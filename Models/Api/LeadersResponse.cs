using System.Text.Json.Serialization;

namespace PuckDrop.Models.Api;

/// <summary>
/// Response for skater stats leaders.
/// </summary>
public class SkaterLeadersResponse
{
    [JsonPropertyName("goals")]
    public List<SkaterLeader>? Goals { get; set; }

    [JsonPropertyName("assists")]
    public List<SkaterLeader>? Assists { get; set; }

    [JsonPropertyName("points")]
    public List<SkaterLeader>? Points { get; set; }

    [JsonPropertyName("plusMinus")]
    public List<SkaterLeader>? PlusMinus { get; set; }

    [JsonPropertyName("penaltyMins")]
    public List<SkaterLeader>? PenaltyMins { get; set; }

    [JsonPropertyName("powerPlayGoals")]
    public List<SkaterLeader>? PowerPlayGoals { get; set; }

    [JsonPropertyName("shots")]
    public List<SkaterLeader>? Shots { get; set; }

    [JsonPropertyName("faceoffLeaders")]
    public List<SkaterLeader>? FaceoffLeaders { get; set; }

    [JsonPropertyName("timeOnIcePerGame")]
    public List<SkaterLeader>? TimeOnIcePerGame { get; set; }
}

/// <summary>
/// A skater in the leaders list.
/// </summary>
public class SkaterLeader
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("firstName")]
    public TeamName FirstName { get; set; } = new();

    [JsonPropertyName("lastName")]
    public TeamName LastName { get; set; } = new();

    [JsonPropertyName("sweaterNumber")]
    public int? SweaterNumber { get; set; }

    [JsonPropertyName("teamAbbrev")]
    public string TeamAbbrev { get; set; } = string.Empty;

    [JsonPropertyName("teamName")]
    public TeamName? TeamName { get; set; }

    [JsonPropertyName("teamLogo")]
    public string? TeamLogo { get; set; }

    [JsonPropertyName("headshot")]
    public string? Headshot { get; set; }

    [JsonPropertyName("position")]
    public string Position { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public object? Value { get; set; }

    [JsonIgnore]
    public string FullName => $"{FirstName.Default} {LastName.Default}";
}

/// <summary>
/// Response for goalie stats leaders.
/// </summary>
public class GoalieLeadersResponse
{
    [JsonPropertyName("wins")]
    public List<GoalieLeader>? Wins { get; set; }

    [JsonPropertyName("savePctg")]
    public List<GoalieLeader>? SavePctg { get; set; }

    [JsonPropertyName("goalsAgainstAverage")]
    public List<GoalieLeader>? GoalsAgainstAverage { get; set; }

    [JsonPropertyName("shutouts")]
    public List<GoalieLeader>? Shutouts { get; set; }
}

/// <summary>
/// A goalie in the leaders list.
/// </summary>
public class GoalieLeader
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("firstName")]
    public TeamName FirstName { get; set; } = new();

    [JsonPropertyName("lastName")]
    public TeamName LastName { get; set; } = new();

    [JsonPropertyName("sweaterNumber")]
    public int? SweaterNumber { get; set; }

    [JsonPropertyName("teamAbbrev")]
    public string TeamAbbrev { get; set; } = string.Empty;

    [JsonPropertyName("teamName")]
    public TeamName? TeamName { get; set; }

    [JsonPropertyName("teamLogo")]
    public string? TeamLogo { get; set; }

    [JsonPropertyName("headshot")]
    public string? Headshot { get; set; }

    [JsonPropertyName("value")]
    public object? Value { get; set; }

    [JsonIgnore]
    public string FullName => $"{FirstName.Default} {LastName.Default}";
}
