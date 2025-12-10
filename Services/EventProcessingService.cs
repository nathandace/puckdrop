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

        // Pre-fetch all processed events for this game in a single query
        var allProcessedEventIds = await GetProcessedEventIdsAsync(gameId, cancellationToken);

        // Pre-fetch all enabled rules for both teams (single query per team)
        var homeRulesByType = await _webhookRuleService.GetAllEnabledRulesForTeamAsync(
            landing.HomeTeam.Abbrev, cancellationToken);
        var awayRulesByType = await _webhookRuleService.GetAllEnabledRulesForTeamAsync(
            landing.AwayTeam.Abbrev, cancellationToken);

        var processedCount = 0;
        var eventsToMark = new List<(string EventId, WebhookEventType EventType)>();

        foreach (var play in playByPlay.Plays)
        {
            var webhookEventType = MapPlayToEventType(play);
            if (webhookEventType == null)
            {
                continue;
            }

            var eventId = $"{play.EventId}_{play.TypeCode}";

            // Check against pre-fetched set instead of database query
            if (allProcessedEventIds.Contains(eventId))
            {
                continue;
            }

            // Get rules from pre-fetched dictionaries
            var enabledRules = new List<WebhookRule>();
            if (homeRulesByType.TryGetValue(webhookEventType.Value, out var homeRules))
            {
                enabledRules.AddRange(homeRules);
            }
            if (awayRulesByType.TryGetValue(webhookEventType.Value, out var awayRules))
            {
                enabledRules.AddRange(awayRules);
            }

            // Track event for batch marking
            eventsToMark.Add((eventId, webhookEventType.Value));

            // Send standard webhooks
            if (enabledRules.Count > 0)
            {
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
            }

            // For goals, also check for FavoriteTeamGoal and OpponentGoal rules
            if (webhookEventType.Value == WebhookEventType.GoalScored && play.Details?.EventOwnerTeamId != null)
            {
                var scoringTeamAbbrev = GetTeamAbbrevFromId(play.Details.EventOwnerTeamId, playByPlay);

                // For each team, determine if this is their goal or opponent's goal
                foreach (var (teamAbbrev, rulesByType) in new[] {
                    (landing.HomeTeam.Abbrev, homeRulesByType),
                    (landing.AwayTeam.Abbrev, awayRulesByType) })
                {
                    var isTeamGoal = scoringTeamAbbrev == teamAbbrev;
                    var specialEventType = isTeamGoal
                        ? WebhookEventType.FavoriteTeamGoal
                        : WebhookEventType.OpponentGoal;

                    if (rulesByType.TryGetValue(specialEventType, out var specialRules) && specialRules.Count > 0)
                    {
                        var payload = CreatePayload(specialEventType, play, playByPlay, landing);

                        foreach (var rule in specialRules)
                        {
                            try
                            {
                                var success = await _webhookDispatchService.SendWebhookAsync(rule, payload, cancellationToken);
                                if (success)
                                {
                                    _logger.LogInformation(
                                        "Webhook sent for {EventType} in game {GameId} to {RuleName}",
                                        specialEventType, gameId, rule.Name ?? $"{rule.TeamAbbrev} webhook");
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex,
                                    "Failed to send webhook for {EventType} in game {GameId}",
                                    specialEventType, gameId);
                            }
                        }
                    }
                }
            }

            processedCount++;
        }

        // Batch mark all processed events
        if (eventsToMark.Count > 0)
        {
            await BatchMarkEventsProcessedAsync(gameId, eventsToMark, cancellationToken);
        }

        await ProcessGameStateEventsAsync(gameId, landing, cancellationToken);

        return processedCount;
    }

    /// <summary>
    /// Gets all processed event IDs for a game in a single query.
    /// </summary>
    private async Task<HashSet<string>> GetProcessedEventIdsAsync(int gameId, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var eventIds = await context.ProcessedEvents
            .Where(e => e.GameId == gameId)
            .Select(e => e.EventId)
            .ToListAsync(cancellationToken);
        return new HashSet<string>(eventIds);
    }

    /// <summary>
    /// Batch marks multiple events as processed in a single transaction.
    /// </summary>
    private async Task BatchMarkEventsProcessedAsync(
        int gameId,
        List<(string EventId, WebhookEventType EventType)> events,
        CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        // Get existing event IDs to avoid duplicates
        var eventIds = events.Select(e => e.EventId).ToList();
        var existingEventIds = await context.ProcessedEvents
            .Where(e => e.GameId == gameId && eventIds.Contains(e.EventId))
            .Select(e => e.EventId)
            .ToListAsync(cancellationToken);
        var existingSet = new HashSet<string>(existingEventIds);

        // Add only new events
        var newEvents = events
            .Where(e => !existingSet.Contains(e.EventId))
            .Select(e => new ProcessedEvent
            {
                GameId = gameId,
                EventId = e.EventId,
                EventType = e.EventType,
                ProcessedAt = DateTime.UtcNow
            });

        context.ProcessedEvents.AddRange(newEvents);

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogDebug(ex, "Some events for game {GameId} already processed (race condition)", gameId);
        }
    }

    /// <inheritdoc />
    public async Task<int> CleanupOldEventsAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        // Use ExecuteDeleteAsync for better performance - deletes directly in database
        // without loading entities into memory
        var deletedCount = await context.ProcessedEvents
            .Where(e => e.ProcessedAt < olderThan)
            .ExecuteDeleteAsync(cancellationToken);

        if (deletedCount > 0)
        {
            _logger.LogInformation("Cleaned up {Count} old processed events", deletedCount);
        }

        return deletedCount;
    }

    /// <summary>
    /// Processes game state events like power plays, goalie pulls, overtime/shootout starts, and game results.
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="landing">The game landing response.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task ProcessGameStateEventsAsync(
        int gameId,
        GameLandingResponse landing,
        CancellationToken cancellationToken)
    {
        // Process power play and goalie pull events
        if (landing.Situation != null)
        {
            var situationCode = landing.Situation.SituationCode;
            if (!string.IsNullOrEmpty(situationCode))
            {
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
        }

        // Process overtime start
        if (landing.PeriodDescriptor?.PeriodType == "OT" && landing.GameState is "LIVE" or "CRIT")
        {
            await ProcessOvertimeStartEventAsync(gameId, landing, cancellationToken);
        }

        // Process shootout start
        if (landing.PeriodDescriptor?.PeriodType == "SO" && landing.GameState is "LIVE" or "CRIT")
        {
            await ProcessShootoutStartEventAsync(gameId, landing, cancellationToken);
        }

        // Process game end with win/loss results
        if (landing.GameState is "FINAL" or "OFF")
        {
            await ProcessGameResultEventsAsync(gameId, landing, cancellationToken);
        }
    }

    /// <summary>
    /// Processes overtime start event.
    /// </summary>
    private async Task ProcessOvertimeStartEventAsync(
        int gameId,
        GameLandingResponse landing,
        CancellationToken cancellationToken)
    {
        var periodNumber = landing.PeriodDescriptor?.Number ?? 4;
        var eventId = $"ot_start_{periodNumber}";

        if (await IsEventProcessedAsync(gameId, eventId, cancellationToken))
        {
            return;
        }

        // Send to both teams
        var teams = new[] { landing.HomeTeam.Abbrev, landing.AwayTeam.Abbrev };
        foreach (var teamAbbrev in teams)
        {
            var rules = await _webhookRuleService.GetEnabledRulesForEventAsync(
                WebhookEventType.OvertimeStart, teamAbbrev, cancellationToken);

            if (rules.Count > 0)
            {
                var payload = CreateBasePayload(WebhookEventType.OvertimeStart, landing);
                payload.Details = new WebhookEventDetails
                {
                    Team = teamAbbrev
                };

                foreach (var rule in rules)
                {
                    await _webhookDispatchService.SendWebhookAsync(rule, payload, cancellationToken);
                }
            }
        }

        await MarkEventProcessedAsync(gameId, eventId, WebhookEventType.OvertimeStart, cancellationToken);
    }

    /// <summary>
    /// Processes shootout start event.
    /// </summary>
    private async Task ProcessShootoutStartEventAsync(
        int gameId,
        GameLandingResponse landing,
        CancellationToken cancellationToken)
    {
        var eventId = "so_start";

        if (await IsEventProcessedAsync(gameId, eventId, cancellationToken))
        {
            return;
        }

        // Send to both teams
        var teams = new[] { landing.HomeTeam.Abbrev, landing.AwayTeam.Abbrev };
        foreach (var teamAbbrev in teams)
        {
            var rules = await _webhookRuleService.GetEnabledRulesForEventAsync(
                WebhookEventType.ShootoutStart, teamAbbrev, cancellationToken);

            if (rules.Count > 0)
            {
                var payload = CreateBasePayload(WebhookEventType.ShootoutStart, landing);
                payload.Details = new WebhookEventDetails
                {
                    Team = teamAbbrev
                };

                foreach (var rule in rules)
                {
                    await _webhookDispatchService.SendWebhookAsync(rule, payload, cancellationToken);
                }
            }
        }

        await MarkEventProcessedAsync(gameId, eventId, WebhookEventType.ShootoutStart, cancellationToken);
    }

    /// <summary>
    /// Processes game result events (TeamWin/TeamLoss) for a completed game.
    /// </summary>
    private async Task ProcessGameResultEventsAsync(
        int gameId,
        GameLandingResponse landing,
        CancellationToken cancellationToken)
    {
        var eventId = "game_result";

        if (await IsEventProcessedAsync(gameId, eventId, cancellationToken))
        {
            return;
        }

        var homeWon = landing.HomeTeam.Score > landing.AwayTeam.Score;
        var awayWon = landing.AwayTeam.Score > landing.HomeTeam.Score;

        // Process winner
        var winningTeam = homeWon ? landing.HomeTeam.Abbrev : landing.AwayTeam.Abbrev;
        var losingTeam = homeWon ? landing.AwayTeam.Abbrev : landing.HomeTeam.Abbrev;

        // Send TeamWin webhook
        var winRules = await _webhookRuleService.GetEnabledRulesForEventAsync(
            WebhookEventType.TeamWin, winningTeam, cancellationToken);

        if (winRules.Count > 0)
        {
            var payload = CreateBasePayload(WebhookEventType.TeamWin, landing);
            payload.Details = new WebhookEventDetails
            {
                Team = winningTeam
            };

            foreach (var rule in winRules)
            {
                await _webhookDispatchService.SendWebhookAsync(rule, payload, cancellationToken);
            }
        }

        // Send TeamLoss webhook
        var lossRules = await _webhookRuleService.GetEnabledRulesForEventAsync(
            WebhookEventType.TeamLoss, losingTeam, cancellationToken);

        if (lossRules.Count > 0)
        {
            var payload = CreateBasePayload(WebhookEventType.TeamLoss, landing);
            payload.Details = new WebhookEventDetails
            {
                Team = losingTeam
            };

            foreach (var rule in lossRules)
            {
                await _webhookDispatchService.SendWebhookAsync(rule, payload, cancellationToken);
            }
        }

        await MarkEventProcessedAsync(gameId, eventId, WebhookEventType.GameEnd, cancellationToken);
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
