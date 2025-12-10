namespace PuckDrop.Services;

/// <summary>
/// Service for sending real-time webhook notifications using Blazor's event pattern.
/// </summary>
public class WebhookNotificationService : IWebhookNotificationService
{
    private readonly ILogger<WebhookNotificationService> _logger;

    /// <summary>
    /// Event raised when a webhook is fired.
    /// </summary>
    public event Action<WebhookLogNotification>? OnWebhookFired;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebhookNotificationService"/> class.
    /// </summary>
    public WebhookNotificationService(ILogger<WebhookNotificationService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task NotifyWebhookFiredAsync(WebhookLogNotification notification, CancellationToken cancellationToken = default)
    {
        try
        {
            OnWebhookFired?.Invoke(notification);
            _logger.LogDebug("Sent webhook notification for rule {RuleId}", notification.RuleId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send webhook notification for rule {RuleId}", notification.RuleId);
        }

        return Task.CompletedTask;
    }
}
