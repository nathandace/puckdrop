using System.Text.Json.Serialization;

namespace PuckDrop.Models.Api;

/// <summary>
/// Response model for the play-by-play endpoint.
/// </summary>
public class PlayByPlayResponse
{
    /// <summary>
    /// Gets or sets the game ID.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the season.
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
    /// Gets or sets the venue.
    /// </summary>
    [JsonPropertyName("venue")]
    public VenueName Venue { get; set; } = new();

    /// <summary>
    /// Gets or sets the start time in UTC.
    /// </summary>
    [JsonPropertyName("startTimeUTC")]
    public DateTime StartTimeUtc { get; set; }

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
    /// Gets or sets the away team.
    /// </summary>
    [JsonPropertyName("awayTeam")]
    public PlayByPlayTeam AwayTeam { get; set; } = new();

    /// <summary>
    /// Gets or sets the home team.
    /// </summary>
    [JsonPropertyName("homeTeam")]
    public PlayByPlayTeam HomeTeam { get; set; } = new();

    /// <summary>
    /// Gets or sets the clock.
    /// </summary>
    [JsonPropertyName("clock")]
    public GameClock? Clock { get; set; }

    /// <summary>
    /// Gets or sets the plays.
    /// </summary>
    [JsonPropertyName("plays")]
    public List<Play> Plays { get; set; } = new();

    /// <summary>
    /// Gets or sets the roster spots.
    /// </summary>
    [JsonPropertyName("rosterSpots")]
    public List<RosterSpot>? RosterSpots { get; set; }
}

/// <summary>
/// Represents team information in play-by-play.
/// </summary>
public class PlayByPlayTeam
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
    /// Gets or sets the score.
    /// </summary>
    [JsonPropertyName("score")]
    public int Score { get; set; }

    /// <summary>
    /// Gets or sets the shots on goal.
    /// </summary>
    [JsonPropertyName("sog")]
    public int Sog { get; set; }

    /// <summary>
    /// Gets or sets the logo URL.
    /// </summary>
    [JsonPropertyName("logo")]
    public string Logo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the dark logo URL.
    /// </summary>
    [JsonPropertyName("darkLogo")]
    public string? DarkLogo { get; set; }

    /// <summary>
    /// Gets or sets the on-ice players.
    /// </summary>
    [JsonPropertyName("onIce")]
    public List<int>? OnIce { get; set; }
}

/// <summary>
/// Represents a play event.
/// </summary>
public class Play
{
    /// <summary>
    /// Gets or sets the event ID.
    /// </summary>
    [JsonPropertyName("eventId")]
    public int EventId { get; set; }

    /// <summary>
    /// Gets or sets the period descriptor.
    /// </summary>
    [JsonPropertyName("periodDescriptor")]
    public PeriodDescriptor PeriodDescriptor { get; set; } = new();

    /// <summary>
    /// Gets or sets the time in period.
    /// </summary>
    [JsonPropertyName("timeInPeriod")]
    public string TimeInPeriod { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the time remaining.
    /// </summary>
    [JsonPropertyName("timeRemaining")]
    public string TimeRemaining { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the situation code.
    /// </summary>
    [JsonPropertyName("situationCode")]
    public string? SituationCode { get; set; }

    /// <summary>
    /// Gets or sets the home team defending side.
    /// </summary>
    [JsonPropertyName("homeTeamDefendingSide")]
    public string? HomeTeamDefendingSide { get; set; }

    /// <summary>
    /// Gets or sets the type code.
    /// </summary>
    [JsonPropertyName("typeCode")]
    public int TypeCode { get; set; }

    /// <summary>
    /// Gets or sets the type description key.
    /// </summary>
    [JsonPropertyName("typeDescKey")]
    public string TypeDescKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sort order.
    /// </summary>
    [JsonPropertyName("sortOrder")]
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets or sets the details.
    /// </summary>
    [JsonPropertyName("details")]
    public PlayDetails? Details { get; set; }
}

/// <summary>
/// Represents play event details.
/// </summary>
public class PlayDetails
{
    /// <summary>
    /// Gets or sets the event owner team ID.
    /// </summary>
    [JsonPropertyName("eventOwnerTeamId")]
    public int? EventOwnerTeamId { get; set; }

    /// <summary>
    /// Gets or sets the losing player ID (faceoffs).
    /// </summary>
    [JsonPropertyName("losingPlayerId")]
    public int? LosingPlayerId { get; set; }

    /// <summary>
    /// Gets or sets the winning player ID (faceoffs).
    /// </summary>
    [JsonPropertyName("winningPlayerId")]
    public int? WinningPlayerId { get; set; }

    /// <summary>
    /// Gets or sets the x coordinate.
    /// </summary>
    [JsonPropertyName("xCoord")]
    public int? XCoord { get; set; }

    /// <summary>
    /// Gets or sets the y coordinate.
    /// </summary>
    [JsonPropertyName("yCoord")]
    public int? YCoord { get; set; }

    /// <summary>
    /// Gets or sets the zone code.
    /// </summary>
    [JsonPropertyName("zoneCode")]
    public string? ZoneCode { get; set; }

    /// <summary>
    /// Gets or sets the shooting player ID.
    /// </summary>
    [JsonPropertyName("shootingPlayerId")]
    public int? ShootingPlayerId { get; set; }

    /// <summary>
    /// Gets or sets the goalie in net ID.
    /// </summary>
    [JsonPropertyName("goalieInNetId")]
    public int? GoalieInNetId { get; set; }

    /// <summary>
    /// Gets or sets the shot type.
    /// </summary>
    [JsonPropertyName("shotType")]
    public string? ShotType { get; set; }

    /// <summary>
    /// Gets or sets the blocking player ID.
    /// </summary>
    [JsonPropertyName("blockingPlayerId")]
    public int? BlockingPlayerId { get; set; }

    /// <summary>
    /// Gets or sets the hitting player ID.
    /// </summary>
    [JsonPropertyName("hittingPlayerId")]
    public int? HittingPlayerId { get; set; }

    /// <summary>
    /// Gets or sets the hittee player ID.
    /// </summary>
    [JsonPropertyName("hitteePlayerId")]
    public int? HitteePlayerId { get; set; }

    /// <summary>
    /// Gets or sets the scoring player ID.
    /// </summary>
    [JsonPropertyName("scoringPlayerId")]
    public int? ScoringPlayerId { get; set; }

    /// <summary>
    /// Gets or sets the scoring player total goals.
    /// </summary>
    [JsonPropertyName("scoringPlayerTotal")]
    public int? ScoringPlayerTotal { get; set; }

    /// <summary>
    /// Gets or sets the assist 1 player ID.
    /// </summary>
    [JsonPropertyName("assist1PlayerId")]
    public int? Assist1PlayerId { get; set; }

    /// <summary>
    /// Gets or sets the assist 1 player total.
    /// </summary>
    [JsonPropertyName("assist1PlayerTotal")]
    public int? Assist1PlayerTotal { get; set; }

    /// <summary>
    /// Gets or sets the assist 2 player ID.
    /// </summary>
    [JsonPropertyName("assist2PlayerId")]
    public int? Assist2PlayerId { get; set; }

    /// <summary>
    /// Gets or sets the assist 2 player total.
    /// </summary>
    [JsonPropertyName("assist2PlayerTotal")]
    public int? Assist2PlayerTotal { get; set; }

    /// <summary>
    /// Gets or sets the away score.
    /// </summary>
    [JsonPropertyName("awayScore")]
    public int? AwayScore { get; set; }

    /// <summary>
    /// Gets or sets the home score.
    /// </summary>
    [JsonPropertyName("homeScore")]
    public int? HomeScore { get; set; }

    /// <summary>
    /// Gets or sets the away SOG.
    /// </summary>
    [JsonPropertyName("awaySOG")]
    public int? AwaySog { get; set; }

    /// <summary>
    /// Gets or sets the home SOG.
    /// </summary>
    [JsonPropertyName("homeSOG")]
    public int? HomeSog { get; set; }

    /// <summary>
    /// Gets or sets the penalty committed by player ID.
    /// </summary>
    [JsonPropertyName("committedByPlayerId")]
    public int? CommittedByPlayerId { get; set; }

    /// <summary>
    /// Gets or sets the penalty drawn by player ID.
    /// </summary>
    [JsonPropertyName("drawnByPlayerId")]
    public int? DrawnByPlayerId { get; set; }

    /// <summary>
    /// Gets or sets the penalty type code.
    /// </summary>
    [JsonPropertyName("typeCode")]
    public string? PenaltyTypeCode { get; set; }

    /// <summary>
    /// Gets or sets the penalty description key.
    /// </summary>
    [JsonPropertyName("descKey")]
    public string? DescKey { get; set; }

    /// <summary>
    /// Gets or sets the penalty duration in minutes.
    /// </summary>
    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    /// <summary>
    /// Gets or sets the reason for the event.
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>
    /// Gets or sets the secondary reason.
    /// </summary>
    [JsonPropertyName("secondaryReason")]
    public string? SecondaryReason { get; set; }

    /// <summary>
    /// Gets or sets the player ID (general).
    /// </summary>
    [JsonPropertyName("playerId")]
    public int? PlayerId { get; set; }
}

/// <summary>
/// Represents a roster spot.
/// </summary>
public class RosterSpot
{
    /// <summary>
    /// Gets or sets the team ID.
    /// </summary>
    [JsonPropertyName("teamId")]
    public int TeamId { get; set; }

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
    /// Gets or sets the headshot URL.
    /// </summary>
    [JsonPropertyName("headshot")]
    public string? Headshot { get; set; }
}
