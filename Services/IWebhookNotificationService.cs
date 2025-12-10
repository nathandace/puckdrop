namespace PuckDrop.Services;

/// <summary>
/// DTO for webhook log notifications sent to UI components.
/// </summary>
public class WebhookLogNotification
{
    /// <summary>
    /// The webhook rule ID that was triggered.
    /// </summary>
    public int RuleId { get; set; }

    /// <summary>
    /// The log entry ID.
    /// </summary>
    public int LogId { get; set; }

    /// <summary>
    /// Whether the webhook was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Brief description of the event.
    /// </summary>
    public string? EventDescription { get; set; }

    /// <summary>
    /// Error message if failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// When the webhook was triggered.
    /// </summary>
    public DateTime TriggeredAt { get; set; }
}

/// <summary>
/// Service for sending real-time webhook notifications to connected clients.
/// </summary>
public interface IWebhookNotificationService
{
    /// <summary>
    /// Event raised when a webhook is fired.
    /// </summary>
    event Action<WebhookLogNotification>? OnWebhookFired;

    /// <summary>
    /// Notifies all connected clients about a new webhook log entry.
    /// </summary>
    /// <param name="notification">The webhook log notification.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task NotifyWebhookFiredAsync(WebhookLogNotification notification, CancellationToken cancellationToken = default);
}
