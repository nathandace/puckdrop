using PuckDrop.Models.Api;

namespace PuckDrop.Services;

/// <summary>
/// Interface for NHL API client operations.
/// </summary>
public interface INhlApiClient
{
    /// <summary>
    /// Gets the current standings for all teams.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The standings response.</returns>
    Task<StandingsResponse?> GetStandingsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the season schedule for a specific team.
    /// </summary>
    /// <param name="teamAbbrev">The team abbreviation (e.g., "TOR").</param>
    /// <param name="season">The season identifier (e.g., 20242025).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The schedule response.</returns>
    Task<ScheduleResponse?> GetSeasonScheduleAsync(string teamAbbrev, int season, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the game landing information.
    /// </summary>
    /// <param name="gameId">The game identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The game landing response.</returns>
    Task<GameLandingResponse?> GetGameLandingAsync(int gameId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the play-by-play data for a game.
    /// </summary>
    /// <param name="gameId">The game identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The play-by-play response.</returns>
    Task<PlayByPlayResponse?> GetPlayByPlayAsync(int gameId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the boxscore data for a game.
    /// </summary>
    /// <param name="gameId">The game identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The boxscore response.</returns>
    Task<BoxscoreResponse?> GetBoxscoreAsync(int gameId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the shift chart data for a game.
    /// </summary>
    /// <param name="gameId">The game identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The shift chart response.</returns>
    Task<ShiftChartResponse?> GetShiftChartAsync(int gameId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the team roster.
    /// </summary>
    /// <param name="teamAbbrev">The team abbreviation (e.g., "TOR").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The roster response.</returns>
    Task<RosterResponse?> GetRosterAsync(string teamAbbrev, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the club season stats for a team.
    /// </summary>
    /// <param name="teamAbbrev">The team abbreviation (e.g., "TOR").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The club stats response.</returns>
    Task<ClubStatsResponse?> GetClubStatsAsync(string teamAbbrev, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current season identifier based on today's date.
    /// </summary>
    /// <returns>The season identifier (e.g., 20242025).</returns>
    int GetCurrentSeason();

    /// <summary>
    /// Invalidates cached data for live game endpoints.
    /// </summary>
    /// <param name="gameId">The game identifier.</param>
    void InvalidateLiveGameCache(int gameId);

    /// <summary>
    /// Gets today's scoreboard with all games and scores.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The scoreboard response.</returns>
    Task<ScoreboardResponse?> GetScoreboardAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the player landing page with comprehensive player info.
    /// </summary>
    /// <param name="playerId">The player identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The player landing response.</returns>
    Task<PlayerLandingResponse?> GetPlayerLandingAsync(int playerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current skater stats leaders.
    /// </summary>
    /// <param name="categories">Categories to retrieve (e.g., "goals,assists,points").</param>
    /// <param name="limit">Maximum number of players per category.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The skater leaders response.</returns>
    Task<SkaterLeadersResponse?> GetSkaterLeadersAsync(string? categories = null, int limit = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current goalie stats leaders.
    /// </summary>
    /// <param name="categories">Categories to retrieve (e.g., "wins,savePctg,gaa").</param>
    /// <param name="limit">Maximum number of goalies per category.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The goalie leaders response.</returns>
    Task<GoalieLeadersResponse?> GetGoalieLeadersAsync(string? categories = null, int limit = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the game right-rail data with team stats.
    /// </summary>
    /// <param name="gameId">The game identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The right-rail response with team stats.</returns>
    Task<RightRailResponse?> GetRightRailAsync(int gameId, CancellationToken cancellationToken = default);
}
