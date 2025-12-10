namespace PuckDrop.Models.Domain;

/// <summary>
/// Represents a log entry for a webhook that was triggered.
/// </summary>
public class WebhookLog
{
    /// <summary>
    /// Gets or sets the unique identifier for this log entry.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the ID of the webhook rule that was triggered.
    /// </summary>
    public int WebhookRuleId { get; set; }

    /// <summary>
    /// Gets or sets the webhook rule that was triggered.
    /// </summary>
    public WebhookRule? WebhookRule { get; set; }

    /// <summary>
    /// Gets or sets the event type that triggered the webhook.
    /// </summary>
    public WebhookEventType EventType { get; set; }

    /// <summary>
    /// Gets or sets the game ID associated with this webhook trigger.
    /// </summary>
    public int? GameId { get; set; }

    /// <summary>
    /// Gets or sets whether the webhook was sent successfully.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the HTTP status code returned by the target URL.
    /// </summary>
    public int? HttpStatusCode { get; set; }

    /// <summary>
    /// Gets or sets an error message if the webhook failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the webhook was triggered.
    /// </summary>
    public DateTime TriggeredAt { get; set; }

    /// <summary>
    /// Gets or sets a brief description of the event (e.g., "Goal by Player Name").
    /// </summary>
    public string? EventDescription { get; set; }
}
