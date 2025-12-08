using System.Text.Json.Serialization;

namespace PuckDrop.Models.Api;

/// <summary>
/// Response model for club stats endpoint.
/// </summary>
public class ClubStatsResponse
{
    [JsonPropertyName("season")]
    public string Season { get; set; } = string.Empty;

    [JsonPropertyName("gameType")]
    public int GameType { get; set; }

    [JsonPropertyName("skaters")]
    public List<SkaterSeasonStats> Skaters { get; set; } = new();

    [JsonPropertyName("goalies")]
    public List<GoalieSeasonStats> Goalies { get; set; } = new();
}

/// <summary>
/// Season stats for a skater.
/// </summary>
public class SkaterSeasonStats
{
    [JsonPropertyName("playerId")]
    public int PlayerId { get; set; }

    [JsonPropertyName("headshot")]
    public string? Headshot { get; set; }

    [JsonPropertyName("firstName")]
    public TeamName FirstName { get; set; } = new();

    [JsonPropertyName("lastName")]
    public TeamName LastName { get; set; } = new();

    [JsonPropertyName("positionCode")]
    public string PositionCode { get; set; } = string.Empty;

    [JsonPropertyName("gamesPlayed")]
    public int GamesPlayed { get; set; }

    [JsonPropertyName("goals")]
    public int Goals { get; set; }

    [JsonPropertyName("assists")]
    public int Assists { get; set; }

    [JsonPropertyName("points")]
    public int Points { get; set; }

    [JsonPropertyName("plusMinus")]
    public int PlusMinus { get; set; }

    [JsonPropertyName("penaltyMinutes")]
    public int PenaltyMinutes { get; set; }

    [JsonPropertyName("powerPlayGoals")]
    public int PowerPlayGoals { get; set; }

    [JsonPropertyName("powerPlayPoints")]
    public int PowerPlayPoints { get; set; }

    [JsonPropertyName("shorthandedGoals")]
    public int ShorthandedGoals { get; set; }

    [JsonPropertyName("shorthandedPoints")]
    public int ShorthandedPoints { get; set; }

    [JsonPropertyName("gameWinningGoals")]
    public int GameWinningGoals { get; set; }

    [JsonPropertyName("overtimeGoals")]
    public int OvertimeGoals { get; set; }

    [JsonPropertyName("shots")]
    public int Shots { get; set; }

    [JsonPropertyName("shootingPctg")]
    public double ShootingPctg { get; set; }

    [JsonPropertyName("avgTimeOnIcePerGame")]
    public double AvgTimeOnIcePerGameSeconds { get; set; }

    [JsonPropertyName("faceoffWinPctg")]
    public double? FaceoffWinPctg { get; set; }

    [JsonIgnore]
    public string FullName => $"{FirstName.Default} {LastName.Default}";

    [JsonIgnore]
    public string AvgToi
    {
        get
        {
            var totalSeconds = (int)AvgTimeOnIcePerGameSeconds;
            return $"{totalSeconds / 60}:{totalSeconds % 60:D2}";
        }
    }
}

/// <summary>
/// Season stats for a goalie.
/// </summary>
public class GoalieSeasonStats
{
    [JsonPropertyName("playerId")]
    public int PlayerId { get; set; }

    [JsonPropertyName("headshot")]
    public string? Headshot { get; set; }

    [JsonPropertyName("firstName")]
    public TeamName FirstName { get; set; } = new();

    [JsonPropertyName("lastName")]
    public TeamName LastName { get; set; } = new();

    [JsonPropertyName("gamesPlayed")]
    public int GamesPlayed { get; set; }

    [JsonPropertyName("gamesStarted")]
    public int GamesStarted { get; set; }

    [JsonPropertyName("wins")]
    public int Wins { get; set; }

    [JsonPropertyName("losses")]
    public int Losses { get; set; }

    [JsonPropertyName("otLosses")]
    public int OtLosses { get; set; }

    [JsonPropertyName("shotsAgainst")]
    public int ShotsAgainst { get; set; }

    [JsonPropertyName("goalsAgainst")]
    public int GoalsAgainst { get; set; }

    [JsonPropertyName("goalsAgainstAvg")]
    public double GoalsAgainstAvg { get; set; }

    [JsonPropertyName("savePctg")]
    public double SavePctg { get; set; }

    [JsonPropertyName("shutouts")]
    public int Shutouts { get; set; }

    [JsonPropertyName("timeOnIce")]
    public int TimeOnIceSeconds { get; set; }

    [JsonIgnore]
    public string FullName => $"{FirstName.Default} {LastName.Default}";

    [JsonIgnore]
    public string TimeOnIce => $"{TimeOnIceSeconds / 60}:{TimeOnIceSeconds % 60:D2}";

    [JsonIgnore]
    public int Saves => ShotsAgainst - GoalsAgainst;

    [JsonIgnore]
    public string Record => $"{Wins}-{Losses}-{OtLosses}";
}
