using System.Text.Json.Serialization;

namespace PuckDrop.Models.Api;

/// <summary>
/// Response model for the boxscore endpoint.
/// </summary>
public class BoxscoreResponse
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
    /// Gets or sets the away team boxscore.
    /// </summary>
    [JsonPropertyName("awayTeam")]
    public BoxscoreTeam AwayTeam { get; set; } = new();

    /// <summary>
    /// Gets or sets the home team boxscore.
    /// </summary>
    [JsonPropertyName("homeTeam")]
    public BoxscoreTeam HomeTeam { get; set; } = new();

    /// <summary>
    /// Gets or sets the player by game stats.
    /// </summary>
    [JsonPropertyName("playerByGameStats")]
    public PlayerByGameStats? PlayerByGameStats { get; set; }

    /// <summary>
    /// Gets or sets the game outcome.
    /// </summary>
    [JsonPropertyName("gameOutcome")]
    public GameOutcome? GameOutcome { get; set; }
}

/// <summary>
/// Represents boxscore team information.
/// </summary>
public class BoxscoreTeam
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
    /// Gets or sets the faceoff win percentage.
    /// </summary>
    [JsonPropertyName("faceoffWinningPctg")]
    public double? FaceoffWinningPctg { get; set; }

    /// <summary>
    /// Gets or sets the power play conversion.
    /// </summary>
    [JsonPropertyName("powerPlayConversion")]
    public string? PowerPlayConversion { get; set; }

    /// <summary>
    /// Gets or sets the PIM.
    /// </summary>
    [JsonPropertyName("pim")]
    public int Pim { get; set; }

    /// <summary>
    /// Gets or sets the hits.
    /// </summary>
    [JsonPropertyName("hits")]
    public int Hits { get; set; }

    /// <summary>
    /// Gets or sets the blocks.
    /// </summary>
    [JsonPropertyName("blocks")]
    public int Blocks { get; set; }

    /// <summary>
    /// Gets or sets the logo URL.
    /// </summary>
    [JsonPropertyName("logo")]
    public string Logo { get; set; } = string.Empty;
}

/// <summary>
/// Represents player stats by game.
/// </summary>
public class PlayerByGameStats
{
    /// <summary>
    /// Gets or sets the away team players.
    /// </summary>
    [JsonPropertyName("awayTeam")]
    public TeamPlayerStats AwayTeam { get; set; } = new();

    /// <summary>
    /// Gets or sets the home team players.
    /// </summary>
    [JsonPropertyName("homeTeam")]
    public TeamPlayerStats HomeTeam { get; set; } = new();
}

/// <summary>
/// Represents team player stats.
/// </summary>
public class TeamPlayerStats
{
    /// <summary>
    /// Gets or sets the forwards.
    /// </summary>
    [JsonPropertyName("forwards")]
    public List<SkaterStats> Forwards { get; set; } = new();

    /// <summary>
    /// Gets or sets the defense.
    /// </summary>
    [JsonPropertyName("defense")]
    public List<SkaterStats> Defense { get; set; } = new();

    /// <summary>
    /// Gets or sets the goalies.
    /// </summary>
    [JsonPropertyName("goalies")]
    public List<GoalieStats> Goalies { get; set; } = new();
}

/// <summary>
/// Represents skater statistics.
/// </summary>
public class SkaterStats
{
    /// <summary>
    /// Gets or sets the player ID.
    /// </summary>
    [JsonPropertyName("playerId")]
    public int PlayerId { get; set; }

    /// <summary>
    /// Gets or sets the sweater number.
    /// </summary>
    [JsonPropertyName("sweaterNumber")]
    public int SweaterNumber { get; set; }

    /// <summary>
    /// Gets or sets the player name.
    /// </summary>
    [JsonPropertyName("name")]
    public PlayerName Name { get; set; } = new();

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

    /// <summary>
    /// Gets or sets the plus/minus.
    /// </summary>
    [JsonPropertyName("plusMinus")]
    public int PlusMinus { get; set; }

    /// <summary>
    /// Gets or sets the penalty minutes.
    /// </summary>
    [JsonPropertyName("pim")]
    public int Pim { get; set; }

    /// <summary>
    /// Gets or sets the hits.
    /// </summary>
    [JsonPropertyName("hits")]
    public int Hits { get; set; }

    /// <summary>
    /// Gets or sets the blocked shots.
    /// </summary>
    [JsonPropertyName("blockedShots")]
    public int BlockedShots { get; set; }

    /// <summary>
    /// Gets or sets the power play goals.
    /// </summary>
    [JsonPropertyName("powerPlayGoals")]
    public int PowerPlayGoals { get; set; }

    /// <summary>
    /// Gets or sets the power play points.
    /// </summary>
    [JsonPropertyName("powerPlayPoints")]
    public int PowerPlayPoints { get; set; }

    /// <summary>
    /// Gets or sets the shorthanded goals.
    /// </summary>
    [JsonPropertyName("shorthandedGoals")]
    public int ShorthandedGoals { get; set; }

    /// <summary>
    /// Gets or sets the shorthanded points.
    /// </summary>
    [JsonPropertyName("shorthanded")]
    public int ShorthandedPoints { get; set; }

    /// <summary>
    /// Gets or sets the shots.
    /// </summary>
    [JsonPropertyName("shots")]
    public int Shots { get; set; }

    /// <summary>
    /// Gets or sets the faceoffs.
    /// </summary>
    [JsonPropertyName("faceoffs")]
    public string? Faceoffs { get; set; }

    /// <summary>
    /// Gets or sets the faceoff win percentage.
    /// </summary>
    [JsonPropertyName("faceoffWinningPctg")]
    public double? FaceoffWinningPctg { get; set; }

    /// <summary>
    /// Gets or sets the time on ice.
    /// </summary>
    [JsonPropertyName("toi")]
    public string Toi { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the power play time on ice.
    /// </summary>
    [JsonPropertyName("powerPlayToi")]
    public string? PowerPlayToi { get; set; }

    /// <summary>
    /// Gets or sets the shorthanded time on ice.
    /// </summary>
    [JsonPropertyName("shorthandedToi")]
    public string? ShorthandedToi { get; set; }

    /// <summary>
    /// Gets or sets the giveaways.
    /// </summary>
    [JsonPropertyName("giveaways")]
    public int Giveaways { get; set; }

    /// <summary>
    /// Gets or sets the takeaways.
    /// </summary>
    [JsonPropertyName("takeaways")]
    public int Takeaways { get; set; }

    /// <summary>
    /// Gets or sets the number of faceoffs won.
    /// </summary>
    public int? FaceoffWins => ParseFaceoffWins();

    /// <summary>
    /// Gets or sets the total number of faceoffs taken.
    /// </summary>
    public int? FaceoffsTotal => ParseFaceoffTotal();

    private int? ParseFaceoffWins()
    {
        if (string.IsNullOrEmpty(Faceoffs)) return null;
        var parts = Faceoffs.Split('/');
        if (parts.Length == 2 && int.TryParse(parts[0], out var wins))
            return wins;
        return null;
    }

    private int? ParseFaceoffTotal()
    {
        if (string.IsNullOrEmpty(Faceoffs)) return null;
        var parts = Faceoffs.Split('/');
        if (parts.Length == 2 && int.TryParse(parts[1], out var total))
            return total;
        return null;
    }
}

/// <summary>
/// Represents goalie statistics.
/// </summary>
public class GoalieStats
{
    /// <summary>
    /// Gets or sets the player ID.
    /// </summary>
    [JsonPropertyName("playerId")]
    public int PlayerId { get; set; }

    /// <summary>
    /// Gets or sets the sweater number.
    /// </summary>
    [JsonPropertyName("sweaterNumber")]
    public int SweaterNumber { get; set; }

    /// <summary>
    /// Gets or sets the player name.
    /// </summary>
    [JsonPropertyName("name")]
    public PlayerName Name { get; set; } = new();

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    [JsonPropertyName("position")]
    public string Position { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the even strength shots against.
    /// </summary>
    [JsonPropertyName("evenStrengthShotsAgainst")]
    public string? EvenStrengthShotsAgainst { get; set; }

    /// <summary>
    /// Gets or sets the power play shots against.
    /// </summary>
    [JsonPropertyName("powerPlayShotsAgainst")]
    public string? PowerPlayShotsAgainst { get; set; }

    /// <summary>
    /// Gets or sets the shorthanded shots against.
    /// </summary>
    [JsonPropertyName("shorthandedShotsAgainst")]
    public string? ShorthandedShotsAgainst { get; set; }

    /// <summary>
    /// Gets or sets the saves versus shots.
    /// </summary>
    [JsonPropertyName("saveShotsAgainst")]
    public string? SaveShotsAgainst { get; set; }

    /// <summary>
    /// Gets or sets the even strength goals against.
    /// </summary>
    [JsonPropertyName("evenStrengthGoalsAgainst")]
    public int EvenStrengthGoalsAgainst { get; set; }

    /// <summary>
    /// Gets or sets the power play goals against.
    /// </summary>
    [JsonPropertyName("powerPlayGoalsAgainst")]
    public int PowerPlayGoalsAgainst { get; set; }

    /// <summary>
    /// Gets or sets the shorthanded goals against.
    /// </summary>
    [JsonPropertyName("shorthandedGoalsAgainst")]
    public int ShorthandedGoalsAgainst { get; set; }

    /// <summary>
    /// Gets or sets the penalty minutes.
    /// </summary>
    [JsonPropertyName("pim")]
    public int Pim { get; set; }

    /// <summary>
    /// Gets or sets the goals against.
    /// </summary>
    [JsonPropertyName("goalsAgainst")]
    public int GoalsAgainst { get; set; }

    /// <summary>
    /// Gets or sets the time on ice.
    /// </summary>
    [JsonPropertyName("toi")]
    public string Toi { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the save percentage.
    /// </summary>
    [JsonPropertyName("savePctg")]
    public double? SavePctg { get; set; }

    /// <summary>
    /// Gets or sets the starter indicator.
    /// </summary>
    [JsonPropertyName("starter")]
    public bool? Starter { get; set; }

    /// <summary>
    /// Gets or sets the decision (W, L, OT).
    /// </summary>
    [JsonPropertyName("decision")]
    public string? Decision { get; set; }
}
