using Microsoft.Extensions.Options;
using PuckDrop.Configuration;
using PuckDrop.Services;
using PuckDrop.State;

namespace PuckDrop.BackgroundServices;

/// <summary>
/// Background service that polls NHL API for live game updates.
/// Polls when:
/// - A viewer is watching a game detail page
/// - Any team with webhook rules has a live game
/// </summary>
public class GamePollingBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly GameStateContainer _gameState;
    private readonly ILogger<GamePollingBackgroundService> _logger;
    private readonly NhlApiOptions _options;

    private TimeSpan PollingInterval => TimeSpan.FromSeconds(_options.Polling.ActiveIntervalSeconds);
    private TimeSpan IdleInterval => TimeSpan.FromSeconds(_options.Polling.IdleIntervalSeconds);
    private TimeSpan LiveGameCheckInterval => TimeSpan.FromSeconds(_options.Polling.LiveGameCheckIntervalSeconds);
    private TimeSpan UpcomingGameCheckInterval => TimeSpan.FromSeconds(_options.Polling.UpcomingGameCheckIntervalSeconds);

    private DateTime _lastLiveGameCheck = DateTime.MinValue;
    private DateTime _lastUpcomingGameCheck = DateTime.MinValue;
    private DateTime _lastCleanup = DateTime.MinValue;
    private static readonly TimeSpan CleanupInterval = TimeSpan.FromHours(6);
    // Dictionary of team abbreviation -> live game ID for teams with webhook rules
    private readonly Dictionary<string, int> _webhookTeamLiveGames = new();
    // Dictionary of team abbreviation -> upcoming game info for pre-game notifications
    private readonly Dictionary<string, UpcomingGameInfo> _upcomingGames = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="GamePollingBackgroundService"/> class.
    /// </summary>
    /// <param name="scopeFactory">The service scope factory.</param>
    /// <param name="gameState">The game state container.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="options">The NHL API configuration options.</param>
    public GamePollingBackgroundService(
        IServiceScopeFactory scopeFactory,
        GameStateContainer gameState,
        ILogger<GamePollingBackgroundService> logger,
        IOptions<NhlApiOptions> options)
    {
        _scopeFactory = scopeFactory;
        _gameState = gameState;
        _logger = logger;
        _options = options.Value;
    }

    private class UpcomingGameInfo
    {
        public int GameId { get; set; }
        public DateTime StartTime { get; set; }
        public bool ReminderSent { get; set; }
    }

    /// <summary>
    /// Executes the background polling service.
    /// </summary>
    /// <param name="stoppingToken">Cancellation token.</param>
    /// <returns>Task representing the async operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Game polling service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var hasViewers = _gameState.ActiveViewers > 0;
                var hasGame = _gameState.CurrentGameId.HasValue;

                // Check for webhook teams' live games periodically
                if (DateTime.UtcNow - _lastLiveGameCheck > LiveGameCheckInterval)
                {
                    await CheckWebhookTeamsLiveGamesAsync(stoppingToken);
                    _lastLiveGameCheck = DateTime.UtcNow;
                }

                // Periodic cleanup of old logs (every 6 hours)
                if (DateTime.UtcNow - _lastCleanup > CleanupInterval)
                {
                    await CleanupOldDataAsync(stoppingToken);
                    _lastCleanup = DateTime.UtcNow;
                }

                if (hasGame && hasViewers)
                {
                    // Check if game is finished - don't continuously poll completed games
                    var isFinished = _gameState.Landing?.GameState is "FINAL" or "OFF";

                    if (isFinished)
                    {
                        // Game is finished and we have the data - no need to keep polling
                        await Task.Delay(IdleInterval, stoppingToken);
                        continue;
                    }

                    // Poll for live/in-progress games (or initial load when Landing is null)
                    await PollGameDataAsync(stoppingToken);
                    await Task.Delay(PollingInterval, stoppingToken);
                }
                else if (_webhookTeamLiveGames.Count > 0)
                {
                    // No viewers, but teams with webhooks have live games - poll for webhooks
                    await PollWebhookTeamGamesAsync(stoppingToken);
                    await Task.Delay(PollingInterval, stoppingToken);
                }
                else
                {
                    // No active game, poll less frequently
                    await Task.Delay(IdleInterval, stoppingToken);
                }
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Service is stopping, exit gracefully
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during game polling");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        _logger.LogInformation("Game polling service stopped");
    }

    /// <summary>
    /// Polls the NHL API for current game data.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task PollGameDataAsync(CancellationToken cancellationToken)
    {
        var gameId = _gameState.CurrentGameId;
        if (!gameId.HasValue)
        {
            return;
        }

        // Don't poll finished games - they won't change
        if (!_gameState.IsLive && _gameState.Landing?.GameState is "FINAL" or "OFF")
        {
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var nhlApiClient = scope.ServiceProvider.GetRequiredService<INhlApiClient>();

        // Only invalidate cache for live games
        if (_gameState.IsLive)
        {
            nhlApiClient.InvalidateLiveGameCache(gameId.Value);
        }

        // Fetch all data in parallel
        var landingTask = nhlApiClient.GetGameLandingAsync(gameId.Value, cancellationToken);
        var playByPlayTask = nhlApiClient.GetPlayByPlayAsync(gameId.Value, cancellationToken);
        var boxscoreTask = nhlApiClient.GetBoxscoreAsync(gameId.Value, cancellationToken);
        var shiftChartTask = nhlApiClient.GetShiftChartAsync(gameId.Value, cancellationToken);

        await Task.WhenAll(landingTask, playByPlayTask, boxscoreTask, shiftChartTask);

        // Results are already available after WhenAll - use .Result since tasks are completed
        var landing = landingTask.Result;
        var playByPlay = playByPlayTask.Result;
        var boxscore = boxscoreTask.Result;
        var shiftChart = shiftChartTask.Result;

        // Update state
        _gameState.UpdateAll(landing, playByPlay, boxscore, shiftChart);

        // Process events for webhooks
        if (landing != null && playByPlay != null)
        {
            var eventProcessingService = scope.ServiceProvider.GetRequiredService<IEventProcessingService>();
            var eventsProcessed = await eventProcessingService.ProcessGameEventsAsync(
                gameId.Value, playByPlay, landing, cancellationToken);

            if (eventsProcessed > 0)
            {
                _logger.LogDebug("Processed {Count} webhook events for game {GameId}",
                    eventsProcessed, gameId.Value);
            }
        }

        _logger.LogDebug(
            "Updated game {GameId} - State: {State}, Score: {AwayScore}-{HomeScore}",
            gameId.Value,
            landing?.GameState ?? "Unknown",
            landing?.AwayTeam.Score ?? 0,
            landing?.HomeTeam.Score ?? 0);
    }

    /// <summary>
    /// Checks for live games for all teams that have enabled webhook rules.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task CheckWebhookTeamsLiveGamesAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var webhookRuleService = scope.ServiceProvider.GetRequiredService<IWebhookRuleService>();
        var nhlApiClient = scope.ServiceProvider.GetRequiredService<INhlApiClient>();

        // Get all teams that have enabled webhook rules
        var teamsWithWebhooks = await webhookRuleService.GetTeamsWithEnabledRulesAsync(cancellationToken);
        if (teamsWithWebhooks.Count == 0)
        {
            _webhookTeamLiveGames.Clear();
            return;
        }

        var season = nhlApiClient.GetCurrentSeason();
        var teamsToRemove = new List<string>(_webhookTeamLiveGames.Keys);

        foreach (var teamAbbrev in teamsWithWebhooks)
        {
            try
            {
                var schedule = await nhlApiClient.GetSeasonScheduleAsync(teamAbbrev, season, cancellationToken);

                var liveGame = schedule?.Games?.FirstOrDefault(g =>
                    g.GameState == "LIVE" || g.GameState == "CRIT");

                if (liveGame != null)
                {
                    teamsToRemove.Remove(teamAbbrev);

                    if (!_webhookTeamLiveGames.TryGetValue(teamAbbrev, out var existingGameId) || existingGameId != liveGame.Id)
                    {
                        _logger.LogInformation(
                            "Team {Team} has live game {GameId} - starting webhook polling",
                            teamAbbrev, liveGame.Id);
                        _webhookTeamLiveGames[teamAbbrev] = liveGame.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to check live games for team {Team}", teamAbbrev);
            }
        }

        // Remove teams that no longer have live games
        foreach (var team in teamsToRemove)
        {
            _logger.LogInformation("Team {Team} game ended - stopping webhook polling", team);
            _webhookTeamLiveGames.Remove(team);
        }
    }

    /// <summary>
    /// Polls all webhook team live games for event processing.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task PollWebhookTeamGamesAsync(CancellationToken cancellationToken)
    {
        if (_webhookTeamLiveGames.Count == 0)
        {
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var nhlApiClient = scope.ServiceProvider.GetRequiredService<INhlApiClient>();
        var eventProcessingService = scope.ServiceProvider.GetRequiredService<IEventProcessingService>();

        var teamsToRemove = new List<string>();

        foreach (var (teamAbbrev, gameId) in _webhookTeamLiveGames)
        {
            try
            {
                // Invalidate cache to get fresh data for webhooks
                nhlApiClient.InvalidateLiveGameCache(gameId);

                // We only need landing and play-by-play for webhook processing
                var landingTask = nhlApiClient.GetGameLandingAsync(gameId, cancellationToken);
                var playByPlayTask = nhlApiClient.GetPlayByPlayAsync(gameId, cancellationToken);

                await Task.WhenAll(landingTask, playByPlayTask);

                var landing = landingTask.Result;
                var playByPlay = playByPlayTask.Result;

                // Check if game has ended
                if (landing?.GameState == "OFF" || landing?.GameState == "FINAL")
                {
                    _logger.LogInformation(
                        "Game {GameId} for team {Team} has ended - stopping webhook polling",
                        gameId, teamAbbrev);
                    teamsToRemove.Add(teamAbbrev);
                    continue;
                }

                // Process events for webhooks
                if (landing != null && playByPlay != null)
                {
                    var eventsProcessed = await eventProcessingService.ProcessGameEventsAsync(
                        gameId, playByPlay, landing, cancellationToken);

                    if (eventsProcessed > 0)
                    {
                        _logger.LogDebug(
                            "Webhook polling: Processed {Count} events for game {GameId} (team {Team})",
                            eventsProcessed, gameId, teamAbbrev);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error polling game {GameId} for team {Team}", gameId, teamAbbrev);
            }
        }

        // Remove finished games
        foreach (var team in teamsToRemove)
        {
            _webhookTeamLiveGames.Remove(team);
        }
    }

    /// <summary>
    /// Cleans up old processed events and webhook logs.
    /// </summary>
    private async Task CleanupOldDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var eventProcessingService = scope.ServiceProvider.GetRequiredService<IEventProcessingService>();
            var webhookRuleService = scope.ServiceProvider.GetRequiredService<IWebhookRuleService>();

            // Clean up processed events older than 7 days
            var eventsCutoff = DateTime.UtcNow.AddDays(-7);
            var eventsDeleted = await eventProcessingService.CleanupOldEventsAsync(eventsCutoff, cancellationToken);

            // Clean up webhook logs older than 14 days
            var logsDeleted = await webhookRuleService.CleanupOldLogsAsync(14, cancellationToken);

            if (eventsDeleted > 0 || logsDeleted > 0)
            {
                _logger.LogInformation(
                    "Cleanup complete: {EventsDeleted} processed events, {LogsDeleted} webhook logs removed",
                    eventsDeleted, logsDeleted);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error during cleanup of old data");
        }
    }
}
