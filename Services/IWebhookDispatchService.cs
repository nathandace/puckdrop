using PuckDrop.Models.Domain;
using PuckDrop.Models.Webhooks;

namespace PuckDrop.Services;

/// <summary>
/// Interface for webhook dispatch operations.
/// </summary>
public interface IWebhookDispatchService
{
    /// <summary>
    /// Sends a webhook for a game event.
    /// </summary>
    /// <param name="rule">The webhook rule.</param>
    /// <param name="payload">The webhook payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the webhook was sent successfully.</returns>
    Task<bool> SendWebhookAsync(WebhookRule rule, WebhookPayload payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a test webhook to verify configuration.
    /// </summary>
    /// <param name="rule">The webhook rule.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the test was successful.</returns>
    Task<bool> SendTestWebhookAsync(WebhookRule rule, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a preview of the payload that would be sent for a webhook rule.
    /// </summary>
    /// <param name="rule">The webhook rule.</param>
    /// <returns>JSON string preview of the payload.</returns>
    string GetPayloadPreview(WebhookRule rule);
}
