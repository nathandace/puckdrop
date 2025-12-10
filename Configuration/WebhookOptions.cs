namespace PuckDrop.Configuration;

/// <summary>
/// Configuration options for webhook delivery.
/// </summary>
public class WebhookOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "Webhooks";

    /// <summary>
    /// Gets or sets the maximum number of retry attempts.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Gets or sets the timeout for webhook requests in seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 10;

    /// <summary>
    /// Gets or sets the base delay between retries in seconds.
    /// </summary>
    public int RetryDelaySeconds { get; set; } = 2;

    /// <summary>
    /// Gets or sets the default pre-game reminder time in minutes before game start.
    /// </summary>
    public int PreGameReminderMinutes { get; set; } = 30;
}
