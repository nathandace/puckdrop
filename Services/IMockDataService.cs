using PuckDrop.Models.Api;

namespace PuckDrop.Services;

/// <summary>
/// Interface for generating mock NHL data for development and testing.
/// </summary>
public interface IMockDataService
{
    /// <summary>
    /// Gets mock game landing data for a simulated live game.
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <returns>Mock game landing response.</returns>
    GameLandingResponse GetMockGameLanding(int gameId);

    /// <summary>
    /// Gets mock play-by-play data.
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <returns>Mock play-by-play response.</returns>
    PlayByPlayResponse GetMockPlayByPlay(int gameId);

    /// <summary>
    /// Gets mock boxscore data.
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <returns>Mock boxscore response.</returns>
    BoxscoreResponse GetMockBoxscore(int gameId);

    /// <summary>
    /// Gets mock shift chart data.
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <returns>Mock shift chart response.</returns>
    ShiftChartResponse GetMockShiftChart(int gameId);

    /// <summary>
    /// Gets mock schedule data for a team's season.
    /// </summary>
    /// <param name="teamAbbrev">The team abbreviation.</param>
    /// <returns>Mock schedule response.</returns>
    ScheduleResponse GetMockSchedule(string teamAbbrev);

    /// <summary>
    /// Advances the mock game state by simulating time passing.
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    void AdvanceGameState(int gameId);

    /// <summary>
    /// Resets the mock game to its initial state.
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    void ResetGame(int gameId);

    /// <summary>
    /// Gets whether the mock game is currently active.
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <returns>True if the mock game is active.</returns>
    bool IsGameActive(int gameId);
}
