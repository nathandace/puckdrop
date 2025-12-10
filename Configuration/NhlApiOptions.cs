namespace PuckDrop.Configuration;

/// <summary>
/// Configuration options for the NHL API client.
/// </summary>
public class NhlApiOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "NhlApi";

    /// <summary>
    /// Gets or sets the base URL for the NHL API.
    /// </summary>
    public string BaseUrl { get; set; } = "https://api-web.nhle.com";

    /// <summary>
    /// Gets or sets the cache duration settings.
    /// </summary>
    public CacheDurationOptions CacheDuration { get; set; } = new();

    /// <summary>
    /// Gets or sets the polling interval settings.
    /// </summary>
    public PollingOptions Polling { get; set; } = new();
}

/// <summary>
/// Cache duration configuration options.
/// </summary>
public class CacheDurationOptions
{
    /// <summary>
    /// Gets or sets the cache duration for static data in minutes.
    /// </summary>
    public int StaticDataMinutes { get; set; } = 360;

    /// <summary>
    /// Gets or sets the cache duration for live data in seconds.
    /// </summary>
    public int LiveDataSeconds { get; set; } = 15;
}

/// <summary>
/// Polling interval configuration options.
/// </summary>
public class PollingOptions
{
    /// <summary>
    /// Gets or sets the polling interval when actively viewing a game in seconds.
    /// </summary>
    public int ActiveIntervalSeconds { get; set; } = 3;

    /// <summary>
    /// Gets or sets the polling interval when idle in seconds.
    /// </summary>
    public int IdleIntervalSeconds { get; set; } = 30;

    /// <summary>
    /// Gets or sets the interval for checking live games in seconds.
    /// </summary>
    public int LiveGameCheckIntervalSeconds { get; set; } = 60;

    /// <summary>
    /// Gets or sets the interval for checking upcoming games for pre-game alerts in seconds.
    /// </summary>
    public int UpcomingGameCheckIntervalSeconds { get; set; } = 300;
}
