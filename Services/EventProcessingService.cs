using Microsoft.EntityFrameworkCore;
using PuckDrop.Data;
using PuckDrop.Models.Api;
using PuckDrop.Models.Domain;
using PuckDrop.Models.Webhooks;

namespace PuckDrop.Services;

/// <summary>
/// Service for processing game events and triggering webhooks.
/// </summary>
public class EventProcessingService : IEventProcessingService
{
    private readonly IDbContextFactory<NhlDbContext> _dbContextFactory;
    private readonly IWebhookRuleService _webhookRuleService;
    private readonly IWebhookDispatchService _webhookDispatchService;
    private readonly ILogger<EventProcessingService> _logger;

    /// <summary>
    /// Event type codes from NHL API.
    /// </summary>
    private static class EventTypeCodes
    {
        public const int Goal = 505;
        public const int Penalty = 509;
        public const int PeriodStart = 520;
        public const int PeriodEnd = 521;
        public const int GameEnd = 524;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventProcessingService"/> class.
    /// </summary>
    /// <param name="dbContextFactory">The database context factory.</param>
    /// <param name="webhookRuleService">The webhook rule service.</param>
    /// <param name="webhookDispatchService">The webhook dispatch service.</param>
    /// <param name="logger">The logger.</param>
    public EventProcessingService(
        IDbContextFactory<NhlDbContext> dbContextFactory,
        IWebhookRuleService webhookRuleService,
        IWebhookDispatchService webhookDispatchService,
        ILogger<EventProcessingService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _webhookRuleService = webhookRuleService;
        _webhookDispatchService = webhookDispatchService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<int> ProcessGameEventsAsync(
        int gameId,
        PlayByPlayResponse playByPlay,
        GameLandingResponse landing,
        CancellationToken cancellationToken = default)
    {
        if (playByPlay?.Plays == null || landing == null)
        {
            return 0;
        }

        var processedCount = 0;

        foreach (var play in playByPlay.Plays)
        {
            var webhookEventType = MapPlayToEventType(play);
            if (webhookEventType == null)
            {
                continue;
            }

            var eventId = $"{play.EventId}_{play.TypeCode}";

            if (await IsEventProcessedAsync(gameId, eventId, cancellationToken))
            {
                continue;
            }

            // Get the team that owns this event
            var eventTeamAbbrev = play.Details != null
                ? GetTeamAbbrevFromId(play.Details.EventOwnerTeamId, playByPlay)
                : null;

            // Get rules for both home and away teams (events may trigger webhooks for either)
            var homeRules = await _webhookRuleService.GetEnabledRulesForEventAsync(
                webhookEventType.Value, landing.HomeTeam.Abbrev, cancellationToken);
            var awayRules = await _webhookRuleService.GetEnabledRulesForEventAsync(
                webhookEventType.Value, landing.AwayTeam.Abbrev, cancellationToken);
            var enabledRules = homeRules.Concat(awayRules).ToList();

            if (enabledRules.Count == 0)
            {
                await MarkEventProcessedAsync(gameId, eventId, webhookEventType.Value, cancellationToken);
                continue;
            }

            var payload = CreatePayload(webhookEventType.Value, play, playByPlay, landing);

            foreach (var rule in enabledRules)
            {
                try
                {
                    var success = await _webhookDispatchService.SendWebhookAsync(rule, payload, cancellationToken);
                    if (success)
                    {
                        _logger.LogInformation(
                            "Webhook sent for {EventType} in game {GameId} to {RuleName}",
                            webhookEventType.Value, gameId, rule.Name ?? $"{rule.TeamAbbrev} webhook");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Failed to send webhook for {EventType} in game {GameId}",
                        webhookEventType.Value, gameId);
                }
            }

            await MarkEventProcessedAsync(gameId, eventId, webhookEventType.Value, cancellationToken);
            processedCount++;
        }

        await ProcessGameStateEventsAsync(gameId, landing, cancellationToken);

        return processedCount;
    }

    /// <inheritdoc />
    public async Task<int> CleanupOldEventsAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var oldEvents = await context.ProcessedEvents
            .Where(e => e.ProcessedAt < olderThan)
            .ToListAsync(cancellationToken);

        if (oldEvents.Count == 0)
        {
            return 0;
        }

        context.ProcessedEvents.RemoveRange(oldEvents);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cleaned up {Count} old processed events", oldEvents.Count);
        return oldEvents.Count;
    }

    /// <summary>
    /// Processes game state events like power plays and goalie pulls.
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="landing">The game landing response.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task ProcessGameStateEventsAsync(
        int gameId,
        GameLandingResponse landing,
        CancellationToken cancellationToken)
    {
        if (landing.Situation == null)
        {
            return;
        }

        var situationCode = landing.Situation.SituationCode;
        if (string.IsNullOrEmpty(situationCode))
        {
            return;
        }

        var homeStrength = landing.Situation.HomeTeam?.Strength ?? 5;
        var awayStrength = landing.Situation.AwayTeam?.Strength ?? 5;

        if (homeStrength > awayStrength)
        {
            await ProcessPowerPlayEventAsync(gameId, landing, landing.HomeTeam.Abbrev,
                $"{homeStrength}v{awayStrength}", cancellationToken);
        }
        else if (awayStrength > homeStrength)
        {
            await ProcessPowerPlayEventAsync(gameId, landing, landing.AwayTeam.Abbrev,
                $"{awayStrength}v{homeStrength}", cancellationToken);
        }

        if (situationCode.Length >= 4)
        {
            var homeGoalie = situationCode[0] == '0';
            var awayGoalie = situationCode[3] == '0';

            if (homeGoalie)
            {
                await ProcessGoaliePulledEventAsync(gameId, landing, landing.HomeTeam.Abbrev, cancellationToken);
            }

            if (awayGoalie)
            {
                await ProcessGoaliePulledEventAsync(gameId, landing, landing.AwayTeam.Abbrev, cancellationToken);
            }
        }
    }

    /// <summary>
    /// Processes a power play event.
    /// </summary>
    private async Task ProcessPowerPlayEventAsync(
        int gameId,
        GameLandingResponse landing,
        string teamAbbrev,
        string strength,
        CancellationToken cancellationToken)
    {
        var eventId = $"pp_{teamAbbrev}_{landing.PeriodDescriptor?.Number}_{landing.Clock?.TimeRemaining}";

        if (await IsEventProcessedAsync(gameId, eventId, cancellationToken))
        {
            return;
        }

        var rules = await _webhookRuleService.GetEnabledRulesForEventAsync(
            WebhookEventType.PowerPlayStart, teamAbbrev, cancellationToken);

        if (rules.Count > 0)
        {
            var payload = CreateBasePayload(WebhookEventType.PowerPlayStart, landing);
            payload.Details = new WebhookEventDetails
            {
                Team = teamAbbrev,
                Strength = strength
            };

            foreach (var rule in rules)
            {
                await _webhookDispatchService.SendWebhookAsync(rule, payload, cancellationToken);
            }
        }

        await MarkEventProcessedAsync(gameId, eventId, WebhookEventType.PowerPlayStart, cancellationToken);
    }

    /// <summary>
    /// Processes a goalie pulled event.
    /// </summary>
    private async Task ProcessGoaliePulledEventAsync(
        int gameId,
        GameLandingResponse landing,
        string teamAbbrev,
        CancellationToken cancellationToken)
    {
        var eventId = $"goalie_pulled_{teamAbbrev}_{landing.PeriodDescriptor?.Number}";

        if (await IsEventProcessedAsync(gameId, eventId, cancellationToken))
        {
            return;
        }

        var rules = await _webhookRuleService.GetEnabledRulesForEventAsync(
            WebhookEventType.GoaliePulled, teamAbbrev, cancellationToken);

        if (rules.Count > 0)
        {
            var payload = CreateBasePayload(WebhookEventType.GoaliePulled, landing);
            payload.Details = new WebhookEventDetails
            {
                Team = teamAbbrev
            };

            foreach (var rule in rules)
            {
                await _webhookDispatchService.SendWebhookAsync(rule, payload, cancellationToken);
            }
        }

        await MarkEventProcessedAsync(gameId, eventId, WebhookEventType.GoaliePulled, cancellationToken);
    }

    /// <summary>
    /// Maps a play to a webhook event type.
    /// </summary>
    /// <param name="play">The play.</param>
    /// <returns>The webhook event type or null if not mapped.</returns>
    private static WebhookEventType? MapPlayToEventType(Play play)
    {
        return play.TypeCode switch
        {
            EventTypeCodes.Goal => WebhookEventType.GoalScored,
            EventTypeCodes.Penalty => WebhookEventType.PenaltyCommitted,
            EventTypeCodes.PeriodStart => play.PeriodDescriptor?.Number == 1
                ? WebhookEventType.GameStart
                : WebhookEventType.PeriodStart,
            EventTypeCodes.PeriodEnd => WebhookEventType.PeriodEnd,
            EventTypeCodes.GameEnd => WebhookEventType.GameEnd,
            _ => null
        };
    }

    /// <summary>
    /// Creates a webhook payload from a play.
    /// </summary>
    private static WebhookPayload CreatePayload(
        WebhookEventType eventType,
        Play play,
        PlayByPlayResponse playByPlay,
        GameLandingResponse landing)
    {
        var payload = CreateBasePayload(eventType, landing);
        payload.Period = play.PeriodDescriptor.Number;
        payload.TimeInPeriod = play.TimeInPeriod;

        if (play.Details != null)
        {
            payload.Details = new WebhookEventDetails
            {
                Team = GetTeamAbbrevFromId(play.Details.EventOwnerTeamId, playByPlay)
            };

            if (eventType == WebhookEventType.GoalScored)
            {
                payload.Details.Player = GetPlayerName(play.Details.ScoringPlayerId, playByPlay);
                if (play.Details.Assist1PlayerId.HasValue)
                {
                    payload.Details.Assists = new List<string>();
                    var assist1 = GetPlayerName(play.Details.Assist1PlayerId, playByPlay);
                    if (!string.IsNullOrEmpty(assist1))
                    {
                        payload.Details.Assists.Add(assist1);
                    }

                    if (play.Details.Assist2PlayerId.HasValue)
                    {
                        var assist2 = GetPlayerName(play.Details.Assist2PlayerId, playByPlay);
                        if (!string.IsNullOrEmpty(assist2))
                        {
                            payload.Details.Assists.Add(assist2);
                        }
                    }
                }

                if (play.Details.HomeScore.HasValue)
                {
                    payload.HomeScore = play.Details.HomeScore.Value;
                }

                if (play.Details.AwayScore.HasValue)
                {
                    payload.AwayScore = play.Details.AwayScore.Value;
                }
            }
            else if (eventType == WebhookEventType.PenaltyCommitted)
            {
                payload.Details.Player = GetPlayerName(play.Details.CommittedByPlayerId, playByPlay);
                payload.Details.PenaltyType = play.Details.DescKey;
                payload.Details.PenaltyMinutes = play.Details.Duration;
            }
        }

        return payload;
    }

    /// <summary>
    /// Creates a base webhook payload from landing data.
    /// </summary>
    private static WebhookPayload CreateBasePayload(WebhookEventType eventType, GameLandingResponse landing)
    {
        return new WebhookPayload
        {
            EventType = eventType.ToString(),
            Timestamp = DateTime.UtcNow,
            GameId = landing.Id,
            Period = landing.PeriodDescriptor?.Number ?? 1,
            TimeInPeriod = landing.Clock?.TimeRemaining ?? "20:00",
            HomeTeam = landing.HomeTeam.Abbrev,
            AwayTeam = landing.AwayTeam.Abbrev,
            HomeScore = landing.HomeTeam.Score,
            AwayScore = landing.AwayTeam.Score
        };
    }

    /// <summary>
    /// Gets the team abbreviation from a team ID.
    /// </summary>
    private static string? GetTeamAbbrevFromId(int? teamId, PlayByPlayResponse playByPlay)
    {
        if (!teamId.HasValue)
        {
            return null;
        }

        if (playByPlay.HomeTeam.Id == teamId.Value)
        {
            return playByPlay.HomeTeam.Abbrev;
        }

        if (playByPlay.AwayTeam.Id == teamId.Value)
        {
            return playByPlay.AwayTeam.Abbrev;
        }

        return null;
    }

    /// <summary>
    /// Gets a player name from a player ID.
    /// </summary>
    private static string? GetPlayerName(int? playerId, PlayByPlayResponse playByPlay)
    {
        if (!playerId.HasValue || playByPlay.RosterSpots == null)
        {
            return null;
        }

        var player = playByPlay.RosterSpots.FirstOrDefault(p => p.PlayerId == playerId.Value);
        if (player == null)
        {
            return null;
        }

        return $"{player.FirstName.Default} {player.LastName.Default}";
    }

    /// <summary>
    /// Checks if an event has already been processed.
    /// </summary>
    private async Task<bool> IsEventProcessedAsync(int gameId, string eventId, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.ProcessedEvents
            .AnyAsync(e => e.GameId == gameId && e.EventId == eventId, cancellationToken);
    }

    /// <summary>
    /// Marks an event as processed.
    /// </summary>
    private async Task MarkEventProcessedAsync(
        int gameId,
        string eventId,
        WebhookEventType eventType,
        CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var exists = await context.ProcessedEvents
            .AnyAsync(e => e.GameId == gameId && e.EventId == eventId, cancellationToken);

        if (exists)
        {
            return;
        }

        context.ProcessedEvents.Add(new ProcessedEvent
        {
            GameId = gameId,
            EventId = eventId,
            EventType = eventType,
            ProcessedAt = DateTime.UtcNow
        });

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            _logger.LogDebug("Event {EventId} for game {GameId} already processed (race condition)", eventId, gameId);
        }
    }
}
