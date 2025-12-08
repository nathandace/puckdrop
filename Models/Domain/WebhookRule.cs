namespace PuckDrop.Models.Domain;

/// <summary>
/// Represents a user-configured webhook rule for game event notifications.
/// </summary>
public class WebhookRule
{
    /// <summary>
    /// Gets or sets the unique identifier for this webhook rule.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the team abbreviation to monitor (e.g., "BOS", "TOR").
    /// </summary>
    public required string TeamAbbrev { get; set; }

    /// <summary>
    /// Gets or sets the type of event that triggers this webhook.
    /// </summary>
    public WebhookEventType EventType { get; set; }

    /// <summary>
    /// Gets or sets the target URL where the webhook payload will be sent.
    /// </summary>
    public required string TargetUrl { get; set; }

    /// <summary>
    /// Gets or sets the format of the webhook payload.
    /// </summary>
    public WebhookPayloadFormat PayloadFormat { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this webhook rule is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets an optional display name for this webhook rule.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this rule was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this rule was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets a custom JSON payload template.
    /// If set, this template will be used instead of the default payload format.
    /// Supports placeholders like {{eventType}}, {{team}}, {{player}}, {{team_colors}}, etc.
    /// </summary>
    public string? CustomPayloadTemplate { get; set; }

    /// <summary>
    /// Gets or sets the delay in seconds before sending the webhook.
    /// Useful for syncing with broadcast/streaming delays. Default is 0 (no delay).
    /// </summary>
    public int DelaySeconds { get; set; } = 0;
}
