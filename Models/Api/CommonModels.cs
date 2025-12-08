using System.Text.Json.Serialization;

namespace PuckDrop.Models.Api;

/// <summary>
/// Represents a player's name with translations.
/// </summary>
public class PlayerName
{
    /// <summary>
    /// Gets or sets the default name.
    /// </summary>
    [JsonPropertyName("default")]
    public string Default { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }
}

/// <summary>
/// Represents a team's name with translations.
/// </summary>
public class TeamName
{
    /// <summary>
    /// Gets or sets the default name.
    /// </summary>
    [JsonPropertyName("default")]
    public string Default { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the French name.
    /// </summary>
    [JsonPropertyName("fr")]
    public string? French { get; set; }
}

/// <summary>
/// Represents a venue name.
/// </summary>
public class VenueName
{
    /// <summary>
    /// Gets or sets the default venue name.
    /// </summary>
    [JsonPropertyName("default")]
    public string Default { get; set; } = string.Empty;
}

/// <summary>
/// Represents basic team information in API responses.
/// </summary>
public class TeamInfo
{
    /// <summary>
    /// Gets or sets the team ID.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the team name.
    /// </summary>
    [JsonPropertyName("name")]
    public TeamName Name { get; set; } = new();

    /// <summary>
    /// Gets or sets the team abbreviation.
    /// </summary>
    [JsonPropertyName("abbrev")]
    public string Abbrev { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the team logo URL.
    /// </summary>
    [JsonPropertyName("logo")]
    public string? Logo { get; set; }

    /// <summary>
    /// Gets or sets the dark logo URL.
    /// </summary>
    [JsonPropertyName("darkLogo")]
    public string? DarkLogo { get; set; }

    /// <summary>
    /// Gets or sets the team's primary color.
    /// </summary>
    [JsonPropertyName("primaryColor")]
    public string? PrimaryColor { get; set; }

    /// <summary>
    /// Gets or sets the team's secondary color.
    /// </summary>
    [JsonPropertyName("secondaryColor")]
    public string? SecondaryColor { get; set; }
}

/// <summary>
/// Represents period descriptor information.
/// </summary>
public class PeriodDescriptor
{
    /// <summary>
    /// Gets or sets the period number.
    /// </summary>
    [JsonPropertyName("number")]
    public int Number { get; set; }

    /// <summary>
    /// Gets or sets the period type (REG, OT, SO).
    /// </summary>
    [JsonPropertyName("periodType")]
    public string PeriodType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the maximum regulation periods.
    /// </summary>
    [JsonPropertyName("maxRegulationPeriods")]
    public int? MaxRegulationPeriods { get; set; }
}

/// <summary>
/// Represents time remaining in a period.
/// </summary>
public class TimeRemaining
{
    /// <summary>
    /// Gets or sets the time remaining string (e.g., "12:34").
    /// </summary>
    [JsonPropertyName("timeRemaining")]
    public string Time { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether running time is used.
    /// </summary>
    [JsonPropertyName("running")]
    public bool Running { get; set; }
}

/// <summary>
/// Represents a player's headshot information.
/// </summary>
public class Headshot
{
    /// <summary>
    /// Gets or sets the headshot URL.
    /// </summary>
    [JsonPropertyName("default")]
    public string? Default { get; set; }
}

/// <summary>
/// Represents coordinates on the ice.
/// </summary>
public class Coordinates
{
    /// <summary>
    /// Gets or sets the X coordinate.
    /// </summary>
    [JsonPropertyName("x")]
    public int X { get; set; }

    /// <summary>
    /// Gets or sets the Y coordinate.
    /// </summary>
    [JsonPropertyName("y")]
    public int Y { get; set; }
}
