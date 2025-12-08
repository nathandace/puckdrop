using PuckDrop.Models.Api;

namespace PuckDrop.State;

/// <summary>
/// Container for shared live game state across all connected clients.
/// </summary>
public class GameStateContainer
{
    private readonly object _lock = new();

    /// <summary>
    /// Event raised when the game state changes.
    /// </summary>
    public event Action? OnStateChanged;

    /// <summary>
    /// Gets the currently monitored game ID.
    /// </summary>
    public int? CurrentGameId { get; private set; }

    /// <summary>
    /// Gets the current game landing data.
    /// </summary>
    public GameLandingResponse? Landing { get; private set; }

    /// <summary>
    /// Gets the current play-by-play data.
    /// </summary>
    public PlayByPlayResponse? PlayByPlay { get; private set; }

    /// <summary>
    /// Gets the current boxscore data.
    /// </summary>
    public BoxscoreResponse? Boxscore { get; private set; }

    /// <summary>
    /// Gets the current shift chart data.
    /// </summary>
    public ShiftChartResponse? ShiftChart { get; private set; }

    /// <summary>
    /// Gets the timestamp of the last update.
    /// </summary>
    public DateTime LastUpdated { get; private set; }

    /// <summary>
    /// Gets whether the game is currently live.
    /// </summary>
    public bool IsLive => Landing?.GameState == "LIVE" || Landing?.GameState == "CRIT";

    /// <summary>
    /// Gets the number of active viewers for this game.
    /// </summary>
    public int ActiveViewers { get; private set; }

    /// <summary>
    /// Sets the game being monitored.
    /// </summary>
    /// <param name="gameId">The game ID to monitor.</param>
    public void SetCurrentGame(int gameId)
    {
        lock (_lock)
        {
            if (CurrentGameId != gameId)
            {
                CurrentGameId = gameId;
                Landing = null;
                PlayByPlay = null;
                Boxscore = null;
                ShiftChart = null;
            }
        }
    }

    /// <summary>
    /// Clears the current game.
    /// </summary>
    public void ClearCurrentGame()
    {
        lock (_lock)
        {
            CurrentGameId = null;
            Landing = null;
            PlayByPlay = null;
            Boxscore = null;
            ShiftChart = null;
        }
    }

    /// <summary>
    /// Updates the game landing data.
    /// </summary>
    /// <param name="landing">The new landing data.</param>
    public void UpdateLanding(GameLandingResponse? landing)
    {
        lock (_lock)
        {
            Landing = landing;
            LastUpdated = DateTime.UtcNow;
        }
        NotifyStateChanged();
    }

    /// <summary>
    /// Updates the play-by-play data.
    /// </summary>
    /// <param name="playByPlay">The new play-by-play data.</param>
    public void UpdatePlayByPlay(PlayByPlayResponse? playByPlay)
    {
        lock (_lock)
        {
            PlayByPlay = playByPlay;
            LastUpdated = DateTime.UtcNow;
        }
        NotifyStateChanged();
    }

    /// <summary>
    /// Updates the boxscore data.
    /// </summary>
    /// <param name="boxscore">The new boxscore data.</param>
    public void UpdateBoxscore(BoxscoreResponse? boxscore)
    {
        lock (_lock)
        {
            Boxscore = boxscore;
            LastUpdated = DateTime.UtcNow;
        }
        NotifyStateChanged();
    }

    /// <summary>
    /// Updates the shift chart data.
    /// </summary>
    /// <param name="shiftChart">The new shift chart data.</param>
    public void UpdateShiftChart(ShiftChartResponse? shiftChart)
    {
        lock (_lock)
        {
            ShiftChart = shiftChart;
            LastUpdated = DateTime.UtcNow;
        }
        NotifyStateChanged();
    }

    /// <summary>
    /// Updates all game data at once.
    /// </summary>
    /// <param name="landing">The landing data.</param>
    /// <param name="playByPlay">The play-by-play data.</param>
    /// <param name="boxscore">The boxscore data.</param>
    /// <param name="shiftChart">The shift chart data.</param>
    public void UpdateAll(
        GameLandingResponse? landing,
        PlayByPlayResponse? playByPlay,
        BoxscoreResponse? boxscore,
        ShiftChartResponse? shiftChart)
    {
        lock (_lock)
        {
            Landing = landing;
            PlayByPlay = playByPlay;
            Boxscore = boxscore;
            ShiftChart = shiftChart;
            LastUpdated = DateTime.UtcNow;
        }
        NotifyStateChanged();
    }

    /// <summary>
    /// Registers a viewer for the current game.
    /// </summary>
    public void RegisterViewer()
    {
        lock (_lock)
        {
            ActiveViewers++;
        }
    }

    /// <summary>
    /// Unregisters a viewer for the current game.
    /// </summary>
    public void UnregisterViewer()
    {
        lock (_lock)
        {
            ActiveViewers = Math.Max(0, ActiveViewers - 1);
        }
    }

    /// <summary>
    /// Notifies all listeners that the state has changed.
    /// Thread-safe: captures delegate before invoking to avoid race conditions.
    /// </summary>
    private void NotifyStateChanged()
    {
        // Capture delegate to avoid race condition where subscriber
        // unsubscribes between null check and invocation
        var handler = OnStateChanged;
        handler?.Invoke();
    }
}
