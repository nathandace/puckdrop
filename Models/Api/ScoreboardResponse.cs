using System.Text.Json.Serialization;

namespace PuckDrop.Models.Api;

/// <summary>
/// Response from the NHL scoreboard endpoint (/v1/score/now).
/// </summary>
public class ScoreboardResponse
{
    [JsonPropertyName("prevDate")]
    public string? PrevDate { get; set; }

    [JsonPropertyName("currentDate")]
    public string CurrentDate { get; set; } = string.Empty;

    [JsonPropertyName("nextDate")]
    public string? NextDate { get; set; }

    [JsonPropertyName("gameWeek")]
    public List<GameWeekDay>? GameWeek { get; set; }

    [JsonPropertyName("games")]
    public List<ScoreboardGame> Games { get; set; } = new();
}

/// <summary>
/// Game count per day in the week.
/// </summary>
public class GameWeekDay
{
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    [JsonPropertyName("dayAbbrev")]
    public string DayAbbrev { get; set; } = string.Empty;

    [JsonPropertyName("numberOfGames")]
    public int NumberOfGames { get; set; }
}

/// <summary>
/// A game on the scoreboard.
/// </summary>
public class ScoreboardGame
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("season")]
    public int Season { get; set; }

    [JsonPropertyName("gameType")]
    public int GameType { get; set; }

    [JsonPropertyName("gameDate")]
    public string GameDate { get; set; } = string.Empty;

    [JsonPropertyName("venue")]
    public VenueName? Venue { get; set; }

    [JsonPropertyName("startTimeUTC")]
    public DateTime StartTimeUtc { get; set; }

    [JsonPropertyName("gameState")]
    public string GameState { get; set; } = string.Empty;

    [JsonPropertyName("gameScheduleState")]
    public string? GameScheduleState { get; set; }

    [JsonPropertyName("awayTeam")]
    public ScoreboardTeam AwayTeam { get; set; } = new();

    [JsonPropertyName("homeTeam")]
    public ScoreboardTeam HomeTeam { get; set; } = new();

    [JsonPropertyName("periodDescriptor")]
    public PeriodDescriptor? PeriodDescriptor { get; set; }

    [JsonPropertyName("clock")]
    public GameClock? Clock { get; set; }

    [JsonPropertyName("goals")]
    public List<ScoreboardGoal>? Goals { get; set; }

    [JsonPropertyName("gameCenterLink")]
    public string? GameCenterLink { get; set; }
}

/// <summary>
/// Team info on the scoreboard.
/// </summary>
public class ScoreboardTeam
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public TeamName? Name { get; set; }

    [JsonPropertyName("abbrev")]
    public string Abbrev { get; set; } = string.Empty;

    [JsonPropertyName("score")]
    public int Score { get; set; }

    [JsonPropertyName("sog")]
    public int? Sog { get; set; }

    [JsonPropertyName("logo")]
    public string? Logo { get; set; }
}

/// <summary>
/// Goal info on the scoreboard.
/// </summary>
public class ScoreboardGoal
{
    [JsonPropertyName("period")]
    public int Period { get; set; }

    [JsonPropertyName("periodDescriptor")]
    public PeriodDescriptor? PeriodDescriptor { get; set; }

    [JsonPropertyName("timeInPeriod")]
    public string TimeInPeriod { get; set; } = string.Empty;

    [JsonPropertyName("playerId")]
    public int? PlayerId { get; set; }

    [JsonPropertyName("name")]
    public TeamName? Name { get; set; }

    [JsonPropertyName("firstName")]
    public TeamName? FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public TeamName? LastName { get; set; }

    [JsonPropertyName("teamAbbrev")]
    public string? TeamAbbrev { get; set; }

    [JsonPropertyName("goalsToDate")]
    public int? GoalsToDate { get; set; }

    [JsonPropertyName("awayScore")]
    public int AwayScore { get; set; }

    [JsonPropertyName("homeScore")]
    public int HomeScore { get; set; }

    [JsonPropertyName("strength")]
    public string? Strength { get; set; }

    [JsonPropertyName("headshot")]
    public string? Headshot { get; set; }
}
