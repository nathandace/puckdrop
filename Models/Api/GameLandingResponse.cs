using System.Text.Json.Serialization;

namespace PuckDrop.Models.Api;

/// <summary>
/// Response model for the game right-rail endpoint with team stats.
/// </summary>
public class RightRailResponse
{
    /// <summary>
    /// Gets or sets the team game stats.
    /// </summary>
    [JsonPropertyName("teamGameStats")]
    public List<TeamGameStat>? TeamGameStats { get; set; }
}

/// <summary>
/// Response model for the game landing endpoint.
/// </summary>
public class GameLandingResponse
{
    /// <summary>
    /// Gets or sets the game ID.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the season identifier.
    /// </summary>
    [JsonPropertyName("season")]
    public int Season { get; set; }

    /// <summary>
    /// Gets or sets the game type.
    /// </summary>
    [JsonPropertyName("gameType")]
    public int GameType { get; set; }

    /// <summary>
    /// Gets or sets the game date.
    /// </summary>
    [JsonPropertyName("gameDate")]
    public string GameDate { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the venue information.
    /// </summary>
    [JsonPropertyName("venue")]
    public VenueName Venue { get; set; } = new();

    /// <summary>
    /// Gets or sets the start time in UTC.
    /// </summary>
    [JsonPropertyName("startTimeUTC")]
    public DateTime StartTimeUtc { get; set; }

    /// <summary>
    /// Gets or sets the Eastern timezone offset.
    /// </summary>
    [JsonPropertyName("easternUTCOffset")]
    public string EasternUtcOffset { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the venue timezone offset.
    /// </summary>
    [JsonPropertyName("venueUTCOffset")]
    public string VenueUtcOffset { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the game state.
    /// </summary>
    [JsonPropertyName("gameState")]
    public string GameState { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the game schedule state.
    /// </summary>
    [JsonPropertyName("gameScheduleState")]
    public string GameScheduleState { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the period descriptor.
    /// </summary>
    [JsonPropertyName("periodDescriptor")]
    public PeriodDescriptor? PeriodDescriptor { get; set; }

    /// <summary>
    /// Gets or sets the away team information.
    /// </summary>
    [JsonPropertyName("awayTeam")]
    public LandingTeam AwayTeam { get; set; } = new();

    /// <summary>
    /// Gets or sets the home team information.
    /// </summary>
    [JsonPropertyName("homeTeam")]
    public LandingTeam HomeTeam { get; set; } = new();

    /// <summary>
    /// Gets or sets the clock information.
    /// </summary>
    [JsonPropertyName("clock")]
    public GameClock? Clock { get; set; }

    /// <summary>
    /// Gets or sets the game situation.
    /// </summary>
    [JsonPropertyName("situation")]
    public GameSituation? Situation { get; set; }

    /// <summary>
    /// Gets or sets the team game stats.
    /// </summary>
    [JsonPropertyName("teamGameStats")]
    public List<TeamGameStat>? TeamGameStats { get; set; }

    /// <summary>
    /// Gets or sets the game outcome.
    /// </summary>
    [JsonPropertyName("gameOutcome")]
    public GameOutcome? GameOutcome { get; set; }

    /// <summary>
    /// Gets or sets the summary information.
    /// </summary>
    [JsonPropertyName("summary")]
    public GameSummary? Summary { get; set; }
}

/// <summary>
/// Represents team information in the landing response.
/// </summary>
public class LandingTeam
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
    public TeamName? Name { get; set; }

    /// <summary>
    /// Gets or sets the team abbreviation.
    /// </summary>
    [JsonPropertyName("abbrev")]
    public string Abbrev { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the place name.
    /// </summary>
    [JsonPropertyName("placeName")]
    public TeamName? PlaceName { get; set; }

    /// <summary>
    /// Gets or sets the team score.
    /// </summary>
    [JsonPropertyName("score")]
    public int Score { get; set; }

    /// <summary>
    /// Gets or sets the team SOG (shots on goal).
    /// </summary>
    [JsonPropertyName("sog")]
    public int Sog { get; set; }

    /// <summary>
    /// Gets or sets the team logo URL.
    /// </summary>
    [JsonPropertyName("logo")]
    public string Logo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the dark logo URL.
    /// </summary>
    [JsonPropertyName("darkLogo")]
    public string? DarkLogo { get; set; }
}

/// <summary>
/// Represents the game clock information.
/// </summary>
public class GameClock
{
    /// <summary>
    /// Gets or sets the time remaining.
    /// </summary>
    [JsonPropertyName("timeRemaining")]
    public string TimeRemaining { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the seconds remaining.
    /// </summary>
    [JsonPropertyName("secondsRemaining")]
    public int SecondsRemaining { get; set; }

    /// <summary>
    /// Gets or sets whether the clock is running.
    /// </summary>
    [JsonPropertyName("running")]
    public bool Running { get; set; }

    /// <summary>
    /// Gets or sets whether the game is in intermission.
    /// </summary>
    [JsonPropertyName("inIntermission")]
    public bool InIntermission { get; set; }
}

/// <summary>
/// Represents the current game situation.
/// </summary>
public class GameSituation
{
    /// <summary>
    /// Gets or sets the home team's skater count (excluding goalie).
    /// </summary>
    [JsonPropertyName("homeTeam")]
    public SituationTeam? HomeTeam { get; set; }

    /// <summary>
    /// Gets or sets the away team's skater count (excluding goalie).
    /// </summary>
    [JsonPropertyName("awayTeam")]
    public SituationTeam? AwayTeam { get; set; }

    /// <summary>
    /// Gets or sets the situation code.
    /// </summary>
    [JsonPropertyName("situationCode")]
    public string? SituationCode { get; set; }

    /// <summary>
    /// Gets or sets the time remaining.
    /// </summary>
    [JsonPropertyName("timeRemaining")]
    public string? TimeRemaining { get; set; }

    /// <summary>
    /// Gets or sets the seconds remaining.
    /// </summary>
    [JsonPropertyName("secondsRemaining")]
    public int? SecondsRemaining { get; set; }
}

/// <summary>
/// Represents team situation information.
/// </summary>
public class SituationTeam
{
    /// <summary>
    /// Gets or sets the team abbreviation.
    /// </summary>
    [JsonPropertyName("abbrev")]
    public string Abbrev { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the situation descriptor (e.g., "PP", "SH").
    /// </summary>
    [JsonPropertyName("situationDescriptor")]
    public string? SituationDescriptor { get; set; }

    /// <summary>
    /// Gets or sets the strength (e.g., 5, 4, 3).
    /// </summary>
    [JsonPropertyName("strength")]
    public int Strength { get; set; }
}

/// <summary>
/// Represents a team game stat.
/// </summary>
public class TeamGameStat
{
    /// <summary>
    /// Gets or sets the category.
    /// </summary>
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the away team value.
    /// </summary>
    [JsonPropertyName("awayValue")]
    public object? AwayValue { get; set; }

    /// <summary>
    /// Gets or sets the home team value.
    /// </summary>
    [JsonPropertyName("homeValue")]
    public object? HomeValue { get; set; }
}

/// <summary>
/// Represents game summary information.
/// </summary>
public class GameSummary
{
    /// <summary>
    /// Gets or sets the scoring summaries.
    /// </summary>
    [JsonPropertyName("scoring")]
    public List<PeriodScoring>? Scoring { get; set; }

    /// <summary>
    /// Gets or sets the shootout information.
    /// </summary>
    [JsonPropertyName("shootout")]
    public List<ShootoutAttempt>? Shootout { get; set; }

    /// <summary>
    /// Gets or sets the penalties.
    /// </summary>
    [JsonPropertyName("penalties")]
    public List<PeriodPenalties>? Penalties { get; set; }

    /// <summary>
    /// Gets or sets the three stars.
    /// </summary>
    [JsonPropertyName("threeStars")]
    public List<ThreeStar>? ThreeStars { get; set; }

    /// <summary>
    /// Gets or sets the game information.
    /// </summary>
    [JsonPropertyName("gameInfo")]
    public GameInfo? GameInfo { get; set; }
}

/// <summary>
/// Represents scoring for a period.
/// </summary>
public class PeriodScoring
{
    /// <summary>
    /// Gets or sets the period descriptor.
    /// </summary>
    [JsonPropertyName("periodDescriptor")]
    public PeriodDescriptor PeriodDescriptor { get; set; } = new();

    /// <summary>
    /// Gets or sets the goals scored in this period.
    /// </summary>
    [JsonPropertyName("goals")]
    public List<GoalInfo>? Goals { get; set; }
}

/// <summary>
/// Represents information about a goal.
/// </summary>
public class GoalInfo
{
    /// <summary>
    /// Gets or sets the situation code.
    /// </summary>
    [JsonPropertyName("situationCode")]
    public string? SituationCode { get; set; }

    /// <summary>
    /// Gets or sets the strength (e.g., "ev", "pp", "sh").
    /// </summary>
    [JsonPropertyName("strength")]
    public string? Strength { get; set; }

    /// <summary>
    /// Gets or sets the player ID.
    /// </summary>
    [JsonPropertyName("playerId")]
    public int PlayerId { get; set; }

    /// <summary>
    /// Gets or sets the goal time in period.
    /// </summary>
    [JsonPropertyName("timeInPeriod")]
    public string TimeInPeriod { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    [JsonPropertyName("firstName")]
    public PlayerName? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    [JsonPropertyName("lastName")]
    public PlayerName? LastName { get; set; }

    /// <summary>
    /// Gets or sets the player's team abbreviation.
    /// </summary>
    [JsonPropertyName("teamAbbrev")]
    public TeamName? TeamAbbrev { get; set; }

    /// <summary>
    /// Gets or sets the headshot URL.
    /// </summary>
    [JsonPropertyName("headshot")]
    public string? Headshot { get; set; }

    /// <summary>
    /// Gets or sets the goal modifier (e.g., empty-net).
    /// </summary>
    [JsonPropertyName("goalModifier")]
    public string? GoalModifier { get; set; }

    /// <summary>
    /// Gets or sets the assists.
    /// </summary>
    [JsonPropertyName("assists")]
    public List<AssistInfo>? Assists { get; set; }

    /// <summary>
    /// Gets or sets the away score.
    /// </summary>
    [JsonPropertyName("awayScore")]
    public int AwayScore { get; set; }

    /// <summary>
    /// Gets or sets the home score.
    /// </summary>
    [JsonPropertyName("homeScore")]
    public int HomeScore { get; set; }
}

/// <summary>
/// Represents assist information.
/// </summary>
public class AssistInfo
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
    public PlayerName? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    [JsonPropertyName("lastName")]
    public PlayerName? LastName { get; set; }

    /// <summary>
    /// Gets or sets the assist number for the player.
    /// </summary>
    [JsonPropertyName("assistsToDate")]
    public int AssistsToDate { get; set; }
}

/// <summary>
/// Represents a shootout attempt.
/// </summary>
public class ShootoutAttempt
{
    /// <summary>
    /// Gets or sets the sequence number.
    /// </summary>
    [JsonPropertyName("sequence")]
    public int Sequence { get; set; }

    /// <summary>
    /// Gets or sets the player ID.
    /// </summary>
    [JsonPropertyName("playerId")]
    public int PlayerId { get; set; }

    /// <summary>
    /// Gets or sets the team abbreviation.
    /// </summary>
    [JsonPropertyName("teamAbbrev")]
    public TeamName? TeamAbbrev { get; set; }

    /// <summary>
    /// Gets or sets the result.
    /// </summary>
    [JsonPropertyName("result")]
    public string Result { get; set; } = string.Empty;
}

/// <summary>
/// Represents penalties for a period.
/// </summary>
public class PeriodPenalties
{
    /// <summary>
    /// Gets or sets the period descriptor.
    /// </summary>
    [JsonPropertyName("periodDescriptor")]
    public PeriodDescriptor PeriodDescriptor { get; set; } = new();

    /// <summary>
    /// Gets or sets the penalties.
    /// </summary>
    [JsonPropertyName("penalties")]
    public List<PenaltyInfo>? Penalties { get; set; }
}

/// <summary>
/// Represents penalty information.
/// </summary>
public class PenaltyInfo
{
    /// <summary>
    /// Gets or sets the time in period.
    /// </summary>
    [JsonPropertyName("timeInPeriod")]
    public string TimeInPeriod { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the penalty type.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the penalty duration.
    /// </summary>
    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    /// <summary>
    /// Gets or sets the committed by player.
    /// </summary>
    [JsonPropertyName("committedByPlayer")]
    public PenaltyPlayer? CommittedByPlayer { get; set; }

    /// <summary>
    /// Gets or sets the team abbreviation.
    /// </summary>
    [JsonPropertyName("teamAbbrev")]
    public TeamName? TeamAbbrev { get; set; }

    /// <summary>
    /// Gets or sets the drawn by player.
    /// </summary>
    [JsonPropertyName("drawnBy")]
    public PenaltyPlayer? DrawnBy { get; set; }

    /// <summary>
    /// Gets or sets the description key.
    /// </summary>
    [JsonPropertyName("descKey")]
    public string? DescKey { get; set; }
}

/// <summary>
/// Represents a player involved in a penalty.
/// </summary>
public class PenaltyPlayer
{
    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    [JsonPropertyName("firstName")]
    public TeamName? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    [JsonPropertyName("lastName")]
    public TeamName? LastName { get; set; }

    /// <summary>
    /// Gets or sets the sweater number.
    /// </summary>
    [JsonPropertyName("sweaterNumber")]
    public int? SweaterNumber { get; set; }

    /// <summary>
    /// Gets the full name of the player.
    /// </summary>
    [JsonIgnore]
    public string FullName => $"{FirstName?.Default} {LastName?.Default}".Trim();
}

/// <summary>
/// Represents a three star selection.
/// </summary>
public class ThreeStar
{
    /// <summary>
    /// Gets or sets the star number (1, 2, or 3).
    /// </summary>
    [JsonPropertyName("star")]
    public int Star { get; set; }

    /// <summary>
    /// Gets or sets the player ID.
    /// </summary>
    [JsonPropertyName("playerId")]
    public int PlayerId { get; set; }

    /// <summary>
    /// Gets or sets the team abbreviation.
    /// </summary>
    [JsonPropertyName("teamAbbrev")]
    public string TeamAbbrev { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the headshot URL.
    /// </summary>
    [JsonPropertyName("headshot")]
    public string? Headshot { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [JsonPropertyName("name")]
    public PlayerName? Name { get; set; }

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    [JsonPropertyName("position")]
    public string Position { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the goals.
    /// </summary>
    [JsonPropertyName("goals")]
    public int Goals { get; set; }

    /// <summary>
    /// Gets or sets the assists.
    /// </summary>
    [JsonPropertyName("assists")]
    public int Assists { get; set; }

    /// <summary>
    /// Gets or sets the points.
    /// </summary>
    [JsonPropertyName("points")]
    public int Points { get; set; }
}

/// <summary>
/// Represents game information.
/// </summary>
public class GameInfo
{
    /// <summary>
    /// Gets or sets the referees.
    /// </summary>
    [JsonPropertyName("referees")]
    public List<OfficialInfo>? Referees { get; set; }

    /// <summary>
    /// Gets or sets the linesmen.
    /// </summary>
    [JsonPropertyName("linesmen")]
    public List<OfficialInfo>? Linesmen { get; set; }

    /// <summary>
    /// Gets or sets the away team info.
    /// </summary>
    [JsonPropertyName("awayTeam")]
    public TeamGameInfo? AwayTeam { get; set; }

    /// <summary>
    /// Gets or sets the home team info.
    /// </summary>
    [JsonPropertyName("homeTeam")]
    public TeamGameInfo? HomeTeam { get; set; }
}

/// <summary>
/// Represents an official's information.
/// </summary>
public class OfficialInfo
{
    /// <summary>
    /// Gets or sets the default name.
    /// </summary>
    [JsonPropertyName("default")]
    public string Default { get; set; } = string.Empty;
}

/// <summary>
/// Represents team game information.
/// </summary>
public class TeamGameInfo
{
    /// <summary>
    /// Gets or sets the head coach.
    /// </summary>
    [JsonPropertyName("headCoach")]
    public OfficialInfo? HeadCoach { get; set; }

    /// <summary>
    /// Gets or sets the scratches.
    /// </summary>
    [JsonPropertyName("scratches")]
    public List<int>? Scratches { get; set; }
}
