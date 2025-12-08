using System.Text.Json.Serialization;

namespace PuckDrop.Models.Api;

/// <summary>
/// Response from the player landing page endpoint.
/// </summary>
public class PlayerLandingResponse
{
    [JsonPropertyName("playerId")]
    public int PlayerId { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    [JsonPropertyName("currentTeamId")]
    public int? CurrentTeamId { get; set; }

    [JsonPropertyName("currentTeamAbbrev")]
    public string? CurrentTeamAbbrev { get; set; }

    [JsonPropertyName("fullTeamName")]
    public TeamName? FullTeamName { get; set; }

    [JsonPropertyName("firstName")]
    public TeamName FirstName { get; set; } = new();

    [JsonPropertyName("lastName")]
    public TeamName LastName { get; set; } = new();

    [JsonPropertyName("teamLogo")]
    public string? TeamLogo { get; set; }

    [JsonPropertyName("sweaterNumber")]
    public int? SweaterNumber { get; set; }

    [JsonPropertyName("position")]
    public string Position { get; set; } = string.Empty;

    [JsonPropertyName("headshot")]
    public string? Headshot { get; set; }

    [JsonPropertyName("heroImage")]
    public string? HeroImage { get; set; }

    [JsonPropertyName("heightInInches")]
    public int? HeightInInches { get; set; }

    [JsonPropertyName("heightInCentimeters")]
    public int? HeightInCentimeters { get; set; }

    [JsonPropertyName("weightInPounds")]
    public int? WeightInPounds { get; set; }

    [JsonPropertyName("weightInKilograms")]
    public int? WeightInKilograms { get; set; }

    [JsonPropertyName("birthDate")]
    public string? BirthDate { get; set; }

    [JsonPropertyName("birthCity")]
    public TeamName? BirthCity { get; set; }

    [JsonPropertyName("birthStateProvince")]
    public TeamName? BirthStateProvince { get; set; }

    [JsonPropertyName("birthCountry")]
    public string? BirthCountry { get; set; }

    [JsonPropertyName("shootsCatches")]
    public string? ShootsCatches { get; set; }

    [JsonPropertyName("draftDetails")]
    public DraftDetails? DraftDetails { get; set; }

    [JsonPropertyName("featuredStats")]
    public FeaturedStats? FeaturedStats { get; set; }

    [JsonPropertyName("careerTotals")]
    public CareerTotals? CareerTotals { get; set; }

    [JsonPropertyName("last5Games")]
    public List<PlayerGameLog>? Last5Games { get; set; }

    [JsonPropertyName("seasonTotals")]
    public List<SeasonTotal>? SeasonTotals { get; set; }

    [JsonPropertyName("awards")]
    public List<PlayerAward>? Awards { get; set; }

    [JsonIgnore]
    public string FullName => $"{FirstName.Default} {LastName.Default}";

    [JsonIgnore]
    public int? Age
    {
        get
        {
            if (string.IsNullOrEmpty(BirthDate)) return null;
            if (DateTime.TryParse(BirthDate, out var birth))
            {
                var today = DateTime.Today;
                var age = today.Year - birth.Year;
                if (birth.Date > today.AddYears(-age)) age--;
                return age;
            }
            return null;
        }
    }
}

/// <summary>
/// Draft details for a player.
/// </summary>
public class DraftDetails
{
    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("teamAbbrev")]
    public string? TeamAbbrev { get; set; }

    [JsonPropertyName("round")]
    public int Round { get; set; }

    [JsonPropertyName("pickInRound")]
    public int PickInRound { get; set; }

    [JsonPropertyName("overallPick")]
    public int OverallPick { get; set; }
}

/// <summary>
/// Featured stats for the current season.
/// </summary>
public class FeaturedStats
{
    [JsonPropertyName("season")]
    public int Season { get; set; }

    [JsonPropertyName("regularSeason")]
    public FeaturedSeasonStats? RegularSeason { get; set; }
}

/// <summary>
/// Stats for a featured season.
/// </summary>
public class FeaturedSeasonStats
{
    [JsonPropertyName("subSeason")]
    public PlayerSeasonStats? SubSeason { get; set; }

    [JsonPropertyName("career")]
    public PlayerSeasonStats? Career { get; set; }
}

/// <summary>
/// Player stats for a season or career.
/// </summary>
public class PlayerSeasonStats
{
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

    [JsonPropertyName("pim")]
    public int Pim { get; set; }

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

    [JsonPropertyName("otGoals")]
    public int OtGoals { get; set; }

    [JsonPropertyName("shots")]
    public int Shots { get; set; }

    [JsonPropertyName("shootingPctg")]
    public double ShootingPctg { get; set; }

    [JsonPropertyName("faceoffWinningPctg")]
    public double? FaceoffWinningPctg { get; set; }

    [JsonPropertyName("avgToi")]
    public string? AvgToiString { get; set; }

    [JsonPropertyName("avgTimeOnIcePerGame")]
    public double? AvgTimeOnIcePerGame { get; set; }

    /// <summary>
    /// Gets the average time on ice, handling both string and numeric formats from API.
    /// </summary>
    [JsonIgnore]
    public string? AvgToi
    {
        get
        {
            // First try the string format
            if (!string.IsNullOrEmpty(AvgToiString))
                return AvgToiString;

            // Fall back to numeric format (seconds)
            if (AvgTimeOnIcePerGame.HasValue && AvgTimeOnIcePerGame.Value > 0)
            {
                var totalSeconds = (int)AvgTimeOnIcePerGame.Value;
                return $"{totalSeconds / 60}:{totalSeconds % 60:D2}";
            }

            return null;
        }
    }

    // Goalie stats
    [JsonPropertyName("wins")]
    public int? Wins { get; set; }

    [JsonPropertyName("losses")]
    public int? Losses { get; set; }

    [JsonPropertyName("otLosses")]
    public int? OtLosses { get; set; }

    [JsonPropertyName("goalsAgainstAvg")]
    public double? GoalsAgainstAvg { get; set; }

    [JsonPropertyName("savePctg")]
    public double? SavePctg { get; set; }

    [JsonPropertyName("shutouts")]
    public int? Shutouts { get; set; }
}

/// <summary>
/// Career totals for regular season and playoffs.
/// </summary>
public class CareerTotals
{
    [JsonPropertyName("regularSeason")]
    public PlayerSeasonStats? RegularSeason { get; set; }

    [JsonPropertyName("playoffs")]
    public PlayerSeasonStats? Playoffs { get; set; }
}

/// <summary>
/// A player's game log entry.
/// </summary>
public class PlayerGameLog
{
    [JsonPropertyName("gameId")]
    public int GameId { get; set; }

    [JsonPropertyName("gameDate")]
    public string GameDate { get; set; } = string.Empty;

    [JsonPropertyName("teamAbbrev")]
    public string TeamAbbrev { get; set; } = string.Empty;

    [JsonPropertyName("homeRoadFlag")]
    public string HomeRoadFlag { get; set; } = string.Empty;

    [JsonPropertyName("opponentAbbrev")]
    public string OpponentAbbrev { get; set; } = string.Empty;

    [JsonPropertyName("goals")]
    public int Goals { get; set; }

    [JsonPropertyName("assists")]
    public int Assists { get; set; }

    [JsonPropertyName("points")]
    public int Points { get; set; }

    [JsonPropertyName("plusMinus")]
    public int PlusMinus { get; set; }

    [JsonPropertyName("pim")]
    public int Pim { get; set; }

    [JsonPropertyName("shots")]
    public int Shots { get; set; }

    [JsonPropertyName("toi")]
    public string? Toi { get; set; }
}

/// <summary>
/// Season total stats for a player.
/// </summary>
public class SeasonTotal
{
    [JsonPropertyName("season")]
    public int Season { get; set; }

    [JsonPropertyName("gameTypeId")]
    public int GameTypeId { get; set; }

    [JsonPropertyName("leagueAbbrev")]
    public string LeagueAbbrev { get; set; } = string.Empty;

    [JsonPropertyName("teamName")]
    public TeamName? TeamName { get; set; }

    [JsonPropertyName("sequence")]
    public int? Sequence { get; set; }

    [JsonPropertyName("gamesPlayed")]
    public int GamesPlayed { get; set; }

    [JsonPropertyName("goals")]
    public int Goals { get; set; }

    [JsonPropertyName("assists")]
    public int Assists { get; set; }

    [JsonPropertyName("points")]
    public int Points { get; set; }

    [JsonPropertyName("pim")]
    public int Pim { get; set; }

    [JsonPropertyName("plusMinus")]
    public int? PlusMinus { get; set; }

    [JsonPropertyName("avgToi")]
    public string? AvgToi { get; set; }
}

/// <summary>
/// An award won by a player.
/// </summary>
public class PlayerAward
{
    [JsonPropertyName("trophy")]
    public TeamName Trophy { get; set; } = new();

    [JsonPropertyName("seasons")]
    public List<AwardSeason>? Seasons { get; set; }
}

/// <summary>
/// Season details for an award.
/// </summary>
public class AwardSeason
{
    [JsonPropertyName("seasonId")]
    public int SeasonId { get; set; }

    [JsonPropertyName("gamesPlayed")]
    public int GamesPlayed { get; set; }

    [JsonPropertyName("goals")]
    public int? Goals { get; set; }

    [JsonPropertyName("assists")]
    public int? Assists { get; set; }

    [JsonPropertyName("points")]
    public int? Points { get; set; }
}
