using System.Text.Json.Serialization;

namespace PuckDrop.Models.Api;

/// <summary>
/// Response model for the shift chart endpoint.
/// </summary>
public class ShiftChartResponse
{
    /// <summary>
    /// Gets or sets the game ID.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the away team shifts.
    /// </summary>
    [JsonPropertyName("awayTeam")]
    public ShiftChartTeam AwayTeam { get; set; } = new();

    /// <summary>
    /// Gets or sets the home team shifts.
    /// </summary>
    [JsonPropertyName("homeTeam")]
    public ShiftChartTeam HomeTeam { get; set; } = new();
}

/// <summary>
/// Represents shift chart team data.
/// </summary>
public class ShiftChartTeam
{
    /// <summary>
    /// Gets or sets the team ID.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the team abbreviation.
    /// </summary>
    [JsonPropertyName("abbrev")]
    public string Abbrev { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the team logo.
    /// </summary>
    [JsonPropertyName("logo")]
    public string Logo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of player shifts.
    /// </summary>
    [JsonPropertyName("players")]
    public List<PlayerShiftData>? Players { get; set; }
}

/// <summary>
/// Represents player shift data.
/// </summary>
public class PlayerShiftData
{
    /// <summary>
    /// Gets or sets the player ID.
    /// </summary>
    [JsonPropertyName("playerId")]
    public int PlayerId { get; set; }

    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    [JsonPropertyName("firstName")]
    public PlayerName FirstName { get; set; } = new();

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    [JsonPropertyName("lastName")]
    public PlayerName LastName { get; set; } = new();

    /// <summary>
    /// Gets or sets the sweater number.
    /// </summary>
    [JsonPropertyName("sweaterNumber")]
    public int SweaterNumber { get; set; }

    /// <summary>
    /// Gets or sets the position code.
    /// </summary>
    [JsonPropertyName("positionCode")]
    public string PositionCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of shifts.
    /// </summary>
    [JsonPropertyName("shifts")]
    public List<Shift> Shifts { get; set; } = new();
}

/// <summary>
/// Represents a single shift.
/// </summary>
public class Shift
{
    /// <summary>
    /// Gets or sets the shift number.
    /// </summary>
    [JsonPropertyName("shiftNumber")]
    public int ShiftNumber { get; set; }

    /// <summary>
    /// Gets or sets the period.
    /// </summary>
    [JsonPropertyName("period")]
    public int Period { get; set; }

    /// <summary>
    /// Gets or sets the start time.
    /// </summary>
    [JsonPropertyName("startTime")]
    public string StartTime { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the end time.
    /// </summary>
    [JsonPropertyName("endTime")]
    public string? EndTime { get; set; }

    /// <summary>
    /// Gets or sets the duration.
    /// </summary>
    [JsonPropertyName("duration")]
    public string? Duration { get; set; }

    /// <summary>
    /// Gets or sets the type code (e.g., "G" for goalie).
    /// </summary>
    [JsonPropertyName("typeCode")]
    public string? TypeCode { get; set; }

    /// <summary>
    /// Gets or sets the event description.
    /// </summary>
    [JsonPropertyName("eventDescription")]
    public string? EventDescription { get; set; }

    /// <summary>
    /// Gets or sets the event details.
    /// </summary>
    [JsonPropertyName("eventDetails")]
    public string? EventDetails { get; set; }

    /// <summary>
    /// Gets or sets the event time in game.
    /// </summary>
    [JsonPropertyName("eventTimeOnIce")]
    public string? EventTimeOnIce { get; set; }
}
