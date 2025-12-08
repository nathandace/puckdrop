using System.Text.Json.Serialization;

namespace PuckDrop.Models.Api;

/// <summary>
/// Response model for the club schedule season endpoint.
/// </summary>
public class ScheduleResponse
{
    /// <summary>
    /// Gets or sets the previous season identifier.
    /// </summary>
    [JsonPropertyName("previousSeason")]
    public int? PreviousSeason { get; set; }

    /// <summary>
    /// Gets or sets the current season identifier.
    /// </summary>
    [JsonPropertyName("currentSeason")]
    public int? CurrentSeason { get; set; }

    /// <summary>
    /// Gets or sets the club timezone.
    /// </summary>
    [JsonPropertyName("clubTimezone")]
    public string ClubTimezone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the club UTC offset.
    /// </summary>
    [JsonPropertyName("clubUTCOffset")]
    public string ClubUtcOffset { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of games.
    /// </summary>
    [JsonPropertyName("games")]
    public List<ScheduleGame> Games { get; set; } = new();
}

/// <summary>
/// Represents a game in the schedule.
/// </summary>
public class ScheduleGame
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
    /// Gets or sets the game type (1=preseason, 2=regular, 3=playoffs).
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
    /// Gets or sets whether this is a neutral site game.
    /// </summary>
    [JsonPropertyName("neutralSite")]
    public bool NeutralSite { get; set; }

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
    /// Gets or sets the game state (FUT, LIVE, OFF, FINAL).
    /// </summary>
    [JsonPropertyName("gameState")]
    public string GameState { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the game schedule state.
    /// </summary>
    [JsonPropertyName("gameScheduleState")]
    public string GameScheduleState { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the away team information.
    /// </summary>
    [JsonPropertyName("awayTeam")]
    public ScheduleTeam AwayTeam { get; set; } = new();

    /// <summary>
    /// Gets or sets the home team information.
    /// </summary>
    [JsonPropertyName("homeTeam")]
    public ScheduleTeam HomeTeam { get; set; } = new();

    /// <summary>
    /// Gets or sets the period descriptor.
    /// </summary>
    [JsonPropertyName("periodDescriptor")]
    public PeriodDescriptor? PeriodDescriptor { get; set; }

    /// <summary>
    /// Gets or sets the game outcome.
    /// </summary>
    [JsonPropertyName("gameOutcome")]
    public GameOutcome? GameOutcome { get; set; }

    /// <summary>
    /// Gets or sets the game center link.
    /// </summary>
    [JsonPropertyName("gameCenterLink")]
    public string? GameCenterLink { get; set; }
}

/// <summary>
/// Represents team information in the schedule.
/// </summary>
public class ScheduleTeam
{
    /// <summary>
    /// Gets or sets the team ID.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the place name.
    /// </summary>
    [JsonPropertyName("placeName")]
    public TeamName? PlaceName { get; set; }

    /// <summary>
    /// Gets or sets the common name.
    /// </summary>
    [JsonPropertyName("commonName")]
    public TeamName? CommonName { get; set; }

    /// <summary>
    /// Gets or sets the team abbreviation.
    /// </summary>
    [JsonPropertyName("abbrev")]
    public string Abbrev { get; set; } = string.Empty;

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

    /// <summary>
    /// Gets or sets the team score.
    /// </summary>
    [JsonPropertyName("score")]
    public int? Score { get; set; }
}

/// <summary>
/// Represents the outcome of a game.
/// </summary>
public class GameOutcome
{
    /// <summary>
    /// Gets or sets the last period type.
    /// </summary>
    [JsonPropertyName("lastPeriodType")]
    public string LastPeriodType { get; set; } = string.Empty;
}
