using System.Text.Json.Serialization;
using PuckDrop.Helpers;
using PuckDrop.Models.Domain;

namespace PuckDrop.Models.Webhooks;

/// <summary>
/// Generic webhook payload for NHL game events.
/// </summary>
public class WebhookPayload
{
    /// <summary>
    /// Gets or sets the event type.
    /// </summary>
    [JsonPropertyName("eventType")]
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the event timestamp.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the game ID.
    /// </summary>
    [JsonPropertyName("gameId")]
    public int GameId { get; set; }

    /// <summary>
    /// Gets or sets the period number.
    /// </summary>
    [JsonPropertyName("period")]
    public int Period { get; set; }

    /// <summary>
    /// Gets or sets the time in period.
    /// </summary>
    [JsonPropertyName("timeInPeriod")]
    public string? TimeInPeriod { get; set; }

    /// <summary>
    /// Gets or sets the home team abbreviation.
    /// </summary>
    [JsonPropertyName("homeTeam")]
    public string HomeTeam { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the away team abbreviation.
    /// </summary>
    [JsonPropertyName("awayTeam")]
    public string AwayTeam { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the home team score.
    /// </summary>
    [JsonPropertyName("homeScore")]
    public int HomeScore { get; set; }

    /// <summary>
    /// Gets or sets the away team score.
    /// </summary>
    [JsonPropertyName("awayScore")]
    public int AwayScore { get; set; }

    /// <summary>
    /// Gets or sets the event details.
    /// </summary>
    [JsonPropertyName("details")]
    public WebhookEventDetails? Details { get; set; }

    /// <summary>
    /// Gets or sets the team colors as RGB objects for LED/smart home integrations.
    /// Each object has r, g, b properties with values 0-255.
    /// </summary>
    [JsonPropertyName("team_colors")]
    public List<RgbColor>? TeamColors { get; set; }
}

/// <summary>
/// Event-specific details for webhook payloads.
/// </summary>
public class WebhookEventDetails
{
    /// <summary>
    /// Gets or sets the team abbreviation involved in the event.
    /// </summary>
    [JsonPropertyName("team")]
    public string? Team { get; set; }

    /// <summary>
    /// Gets or sets the primary player name.
    /// </summary>
    [JsonPropertyName("player")]
    public string? Player { get; set; }

    /// <summary>
    /// Gets or sets the player's jersey number.
    /// </summary>
    [JsonPropertyName("jerseyNumber")]
    public int? JerseyNumber { get; set; }

    /// <summary>
    /// Gets or sets the assist player names.
    /// </summary>
    [JsonPropertyName("assists")]
    public List<string>? Assists { get; set; }

    /// <summary>
    /// Gets or sets the goal type (e.g., "ev", "pp", "sh").
    /// </summary>
    [JsonPropertyName("goalType")]
    public string? GoalType { get; set; }

    /// <summary>
    /// Gets or sets the penalty type.
    /// </summary>
    [JsonPropertyName("penaltyType")]
    public string? PenaltyType { get; set; }

    /// <summary>
    /// Gets or sets the penalty duration in minutes.
    /// </summary>
    [JsonPropertyName("penaltyMinutes")]
    public int? PenaltyMinutes { get; set; }

    /// <summary>
    /// Gets or sets the power play strength (e.g., "5v4", "5v3").
    /// </summary>
    [JsonPropertyName("strength")]
    public string? Strength { get; set; }
}

/// <summary>
/// Discord-formatted webhook payload.
/// </summary>
public class DiscordWebhookPayload
{
    /// <summary>
    /// Gets or sets the content text.
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>
    /// Gets or sets the embeds.
    /// </summary>
    [JsonPropertyName("embeds")]
    public List<DiscordEmbed>? Embeds { get; set; }
}

/// <summary>
/// Discord embed object.
/// </summary>
public class DiscordEmbed
{
    /// <summary>
    /// Gets or sets the embed title.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the embed description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the embed color.
    /// </summary>
    [JsonPropertyName("color")]
    public int? Color { get; set; }

    /// <summary>
    /// Gets or sets the embed fields.
    /// </summary>
    [JsonPropertyName("fields")]
    public List<DiscordEmbedField>? Fields { get; set; }

    /// <summary>
    /// Gets or sets the embed footer.
    /// </summary>
    [JsonPropertyName("footer")]
    public DiscordEmbedFooter? Footer { get; set; }

    /// <summary>
    /// Gets or sets the embed timestamp.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }
}

/// <summary>
/// Discord embed field.
/// </summary>
public class DiscordEmbedField
{
    /// <summary>
    /// Gets or sets the field name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the field is inline.
    /// </summary>
    [JsonPropertyName("inline")]
    public bool Inline { get; set; }
}

/// <summary>
/// Discord embed footer.
/// </summary>
public class DiscordEmbedFooter
{
    /// <summary>
    /// Gets or sets the footer text.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}

/// <summary>
/// Home Assistant webhook payload.
/// </summary>
public class HomeAssistantPayload
{
    /// <summary>
    /// Gets or sets the event type.
    /// </summary>
    [JsonPropertyName("event_type")]
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the event data.
    /// </summary>
    [JsonPropertyName("data")]
    public Dictionary<string, object> Data { get; set; } = new();
}
