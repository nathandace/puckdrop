namespace PuckDrop.Models.Domain;

/// <summary>
/// Tracks processed game events to prevent duplicate webhook triggers.
/// </summary>
public class ProcessedEvent
{
    /// <summary>
    /// Gets or sets the unique identifier for this record.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the NHL game identifier.
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the event within the game.
    /// </summary>
    public required string EventId { get; set; }

    /// <summary>
    /// Gets or sets the type of event that was processed.
    /// </summary>
    public WebhookEventType EventType { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this event was processed.
    /// </summary>
    public DateTime ProcessedAt { get; set; }
}
