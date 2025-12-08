using System.Text.Json.Serialization;

namespace PuckDrop.Models.Api;

/// <summary>
/// Response model for the standings endpoint.
/// </summary>
public class StandingsResponse
{
    /// <summary>
    /// Gets or sets the wild card indicator.
    /// </summary>
    [JsonPropertyName("wildCardIndicator")]
    public bool WildCardIndicator { get; set; }

    /// <summary>
    /// Gets or sets the standings.
    /// </summary>
    [JsonPropertyName("standings")]
    public List<TeamStanding> Standings { get; set; } = new();
}

/// <summary>
/// Represents a team's standing.
/// </summary>
public class TeamStanding
{
    /// <summary>
    /// Gets or sets the conference abbreviation.
    /// </summary>
    [JsonPropertyName("conferenceAbbrev")]
    public string ConferenceAbbrev { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the conference name.
    /// </summary>
    [JsonPropertyName("conferenceName")]
    public string ConferenceName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the division abbreviation.
    /// </summary>
    [JsonPropertyName("divisionAbbrev")]
    public string DivisionAbbrev { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the division name.
    /// </summary>
    [JsonPropertyName("divisionName")]
    public string DivisionName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the place name.
    /// </summary>
    [JsonPropertyName("placeName")]
    public TeamName PlaceName { get; set; } = new();

    /// <summary>
    /// Gets or sets the team name.
    /// </summary>
    [JsonPropertyName("teamName")]
    public TeamName TeamName { get; set; } = new();

    /// <summary>
    /// Gets or sets the team common name.
    /// </summary>
    [JsonPropertyName("teamCommonName")]
    public TeamName TeamCommonName { get; set; } = new();

    /// <summary>
    /// Gets or sets the team abbreviation.
    /// </summary>
    [JsonPropertyName("teamAbbrev")]
    public TeamName TeamAbbrev { get; set; } = new();

    /// <summary>
    /// Gets or sets the team logo URL.
    /// </summary>
    [JsonPropertyName("teamLogo")]
    public string TeamLogo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the conference home sequence.
    /// </summary>
    [JsonPropertyName("conferenceHomeSequence")]
    public int ConferenceHomeSequence { get; set; }

    /// <summary>
    /// Gets or sets the conference L10 sequence.
    /// </summary>
    [JsonPropertyName("conferenceL10Sequence")]
    public int ConferenceL10Sequence { get; set; }

    /// <summary>
    /// Gets or sets the conference road sequence.
    /// </summary>
    [JsonPropertyName("conferenceRoadSequence")]
    public int ConferenceRoadSequence { get; set; }

    /// <summary>
    /// Gets or sets the conference sequence.
    /// </summary>
    [JsonPropertyName("conferenceSequence")]
    public int ConferenceSequence { get; set; }

    /// <summary>
    /// Gets or sets the date.
    /// </summary>
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the division home sequence.
    /// </summary>
    [JsonPropertyName("divisionHomeSequence")]
    public int DivisionHomeSequence { get; set; }

    /// <summary>
    /// Gets or sets the division L10 sequence.
    /// </summary>
    [JsonPropertyName("divisionL10Sequence")]
    public int DivisionL10Sequence { get; set; }

    /// <summary>
    /// Gets or sets the division road sequence.
    /// </summary>
    [JsonPropertyName("divisionRoadSequence")]
    public int DivisionRoadSequence { get; set; }

    /// <summary>
    /// Gets or sets the division sequence.
    /// </summary>
    [JsonPropertyName("divisionSequence")]
    public int DivisionSequence { get; set; }

    /// <summary>
    /// Gets or sets the game type ID.
    /// </summary>
    [JsonPropertyName("gameTypeId")]
    public int GameTypeId { get; set; }

    /// <summary>
    /// Gets or sets the games played.
    /// </summary>
    [JsonPropertyName("gamesPlayed")]
    public int GamesPlayed { get; set; }

    /// <summary>
    /// Gets or sets the goal differential.
    /// </summary>
    [JsonPropertyName("goalDifferential")]
    public int GoalDifferential { get; set; }

    /// <summary>
    /// Gets or sets the goal differential percentage.
    /// </summary>
    [JsonPropertyName("goalDifferentialPctg")]
    public double GoalDifferentialPctg { get; set; }

    /// <summary>
    /// Gets or sets the goals against.
    /// </summary>
    [JsonPropertyName("goalAgainst")]
    public int GoalAgainst { get; set; }

    /// <summary>
    /// Gets or sets the goals for.
    /// </summary>
    [JsonPropertyName("goalFor")]
    public int GoalFor { get; set; }

    /// <summary>
    /// Gets or sets the goals for percentage.
    /// </summary>
    [JsonPropertyName("goalsForPctg")]
    public double GoalsForPctg { get; set; }

    /// <summary>
    /// Gets or sets the home games played.
    /// </summary>
    [JsonPropertyName("homeGamesPlayed")]
    public int HomeGamesPlayed { get; set; }

    /// <summary>
    /// Gets or sets the home goal differential.
    /// </summary>
    [JsonPropertyName("homeGoalDifferential")]
    public int HomeGoalDifferential { get; set; }

    /// <summary>
    /// Gets or sets the home goals against.
    /// </summary>
    [JsonPropertyName("homeGoalsAgainst")]
    public int HomeGoalsAgainst { get; set; }

    /// <summary>
    /// Gets or sets the home goals for.
    /// </summary>
    [JsonPropertyName("homeGoalsFor")]
    public int HomeGoalsFor { get; set; }

    /// <summary>
    /// Gets or sets the home losses.
    /// </summary>
    [JsonPropertyName("homeLosses")]
    public int HomeLosses { get; set; }

    /// <summary>
    /// Gets or sets the home OT losses.
    /// </summary>
    [JsonPropertyName("homeOtLosses")]
    public int HomeOtLosses { get; set; }

    /// <summary>
    /// Gets or sets the home points.
    /// </summary>
    [JsonPropertyName("homePoints")]
    public int HomePoints { get; set; }

    /// <summary>
    /// Gets or sets the home regulation plus OT wins.
    /// </summary>
    [JsonPropertyName("homeRegulationPlusOtWins")]
    public int HomeRegulationPlusOtWins { get; set; }

    /// <summary>
    /// Gets or sets the home regulation wins.
    /// </summary>
    [JsonPropertyName("homeRegulationWins")]
    public int HomeRegulationWins { get; set; }

    /// <summary>
    /// Gets or sets the home ties.
    /// </summary>
    [JsonPropertyName("homeTies")]
    public int HomeTies { get; set; }

    /// <summary>
    /// Gets or sets the home wins.
    /// </summary>
    [JsonPropertyName("homeWins")]
    public int HomeWins { get; set; }

    /// <summary>
    /// Gets or sets the L10 goals against.
    /// </summary>
    [JsonPropertyName("l10GoalsAgainst")]
    public int L10GoalsAgainst { get; set; }

    /// <summary>
    /// Gets or sets the L10 goals for.
    /// </summary>
    [JsonPropertyName("l10GoalsFor")]
    public int L10GoalsFor { get; set; }

    /// <summary>
    /// Gets or sets the L10 losses.
    /// </summary>
    [JsonPropertyName("l10Losses")]
    public int L10Losses { get; set; }

    /// <summary>
    /// Gets or sets the L10 OT losses.
    /// </summary>
    [JsonPropertyName("l10OtLosses")]
    public int L10OtLosses { get; set; }

    /// <summary>
    /// Gets or sets the L10 points.
    /// </summary>
    [JsonPropertyName("l10Points")]
    public int L10Points { get; set; }

    /// <summary>
    /// Gets or sets the L10 regulation plus OT wins.
    /// </summary>
    [JsonPropertyName("l10RegulationPlusOtWins")]
    public int L10RegulationPlusOtWins { get; set; }

    /// <summary>
    /// Gets or sets the L10 regulation wins.
    /// </summary>
    [JsonPropertyName("l10RegulationWins")]
    public int L10RegulationWins { get; set; }

    /// <summary>
    /// Gets or sets the L10 ties.
    /// </summary>
    [JsonPropertyName("l10Ties")]
    public int L10Ties { get; set; }

    /// <summary>
    /// Gets or sets the L10 wins.
    /// </summary>
    [JsonPropertyName("l10Wins")]
    public int L10Wins { get; set; }

    /// <summary>
    /// Gets or sets the league home sequence.
    /// </summary>
    [JsonPropertyName("leagueHomeSequence")]
    public int LeagueHomeSequence { get; set; }

    /// <summary>
    /// Gets or sets the league L10 sequence.
    /// </summary>
    [JsonPropertyName("leagueL10Sequence")]
    public int LeagueL10Sequence { get; set; }

    /// <summary>
    /// Gets or sets the league road sequence.
    /// </summary>
    [JsonPropertyName("leagueRoadSequence")]
    public int LeagueRoadSequence { get; set; }

    /// <summary>
    /// Gets or sets the league sequence.
    /// </summary>
    [JsonPropertyName("leagueSequence")]
    public int LeagueSequence { get; set; }

    /// <summary>
    /// Gets or sets the losses.
    /// </summary>
    [JsonPropertyName("losses")]
    public int Losses { get; set; }

    /// <summary>
    /// Gets or sets the OT losses.
    /// </summary>
    [JsonPropertyName("otLosses")]
    public int OtLosses { get; set; }

    /// <summary>
    /// Gets or sets the point percentage.
    /// </summary>
    [JsonPropertyName("pointPctg")]
    public double PointPctg { get; set; }

    /// <summary>
    /// Gets or sets the points.
    /// </summary>
    [JsonPropertyName("points")]
    public int Points { get; set; }

    /// <summary>
    /// Gets or sets the regulation plus OT wins.
    /// </summary>
    [JsonPropertyName("regulationPlusOtWins")]
    public int RegulationPlusOtWins { get; set; }

    /// <summary>
    /// Gets or sets the regulation wins.
    /// </summary>
    [JsonPropertyName("regulationWins")]
    public int RegulationWins { get; set; }

    /// <summary>
    /// Gets or sets the road games played.
    /// </summary>
    [JsonPropertyName("roadGamesPlayed")]
    public int RoadGamesPlayed { get; set; }

    /// <summary>
    /// Gets or sets the road goal differential.
    /// </summary>
    [JsonPropertyName("roadGoalDifferential")]
    public int RoadGoalDifferential { get; set; }

    /// <summary>
    /// Gets or sets the road goals against.
    /// </summary>
    [JsonPropertyName("roadGoalsAgainst")]
    public int RoadGoalsAgainst { get; set; }

    /// <summary>
    /// Gets or sets the road goals for.
    /// </summary>
    [JsonPropertyName("roadGoalsFor")]
    public int RoadGoalsFor { get; set; }

    /// <summary>
    /// Gets or sets the road losses.
    /// </summary>
    [JsonPropertyName("roadLosses")]
    public int RoadLosses { get; set; }

    /// <summary>
    /// Gets or sets the road OT losses.
    /// </summary>
    [JsonPropertyName("roadOtLosses")]
    public int RoadOtLosses { get; set; }

    /// <summary>
    /// Gets or sets the road points.
    /// </summary>
    [JsonPropertyName("roadPoints")]
    public int RoadPoints { get; set; }

    /// <summary>
    /// Gets or sets the road regulation plus OT wins.
    /// </summary>
    [JsonPropertyName("roadRegulationPlusOtWins")]
    public int RoadRegulationPlusOtWins { get; set; }

    /// <summary>
    /// Gets or sets the road regulation wins.
    /// </summary>
    [JsonPropertyName("roadRegulationWins")]
    public int RoadRegulationWins { get; set; }

    /// <summary>
    /// Gets or sets the road ties.
    /// </summary>
    [JsonPropertyName("roadTies")]
    public int RoadTies { get; set; }

    /// <summary>
    /// Gets or sets the road wins.
    /// </summary>
    [JsonPropertyName("roadWins")]
    public int RoadWins { get; set; }

    /// <summary>
    /// Gets or sets the season ID.
    /// </summary>
    [JsonPropertyName("seasonId")]
    public int SeasonId { get; set; }

    /// <summary>
    /// Gets or sets the shootout losses.
    /// </summary>
    [JsonPropertyName("shootoutLosses")]
    public int ShootoutLosses { get; set; }

    /// <summary>
    /// Gets or sets the shootout wins.
    /// </summary>
    [JsonPropertyName("shootoutWins")]
    public int ShootoutWins { get; set; }

    /// <summary>
    /// Gets or sets the streak code.
    /// </summary>
    [JsonPropertyName("streakCode")]
    public string StreakCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the streak count.
    /// </summary>
    [JsonPropertyName("streakCount")]
    public int StreakCount { get; set; }

    /// <summary>
    /// Gets or sets the ties.
    /// </summary>
    [JsonPropertyName("ties")]
    public int Ties { get; set; }

    /// <summary>
    /// Gets or sets the waiver sequence.
    /// </summary>
    [JsonPropertyName("waiverSequence")]
    public int WaiverSequence { get; set; }

    /// <summary>
    /// Gets or sets the wildcard sequence.
    /// </summary>
    [JsonPropertyName("wildcardSequence")]
    public int WildcardSequence { get; set; }

    /// <summary>
    /// Gets or sets the wins.
    /// </summary>
    [JsonPropertyName("wins")]
    public int Wins { get; set; }
}
