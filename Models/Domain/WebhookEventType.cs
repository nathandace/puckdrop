namespace PuckDrop.Models.Domain;

/// <summary>
/// Defines the types of game events that can trigger webhooks.
/// </summary>
public enum WebhookEventType
{
    /// <summary>
    /// A goal was scored by the selected team.
    /// </summary>
    GoalScored = 0,

    /// <summary>
    /// A penalty was committed by the selected team.
    /// </summary>
    PenaltyCommitted = 1,

    /// <summary>
    /// A power play has started.
    /// </summary>
    PowerPlayStart = 2,

    /// <summary>
    /// A power play has ended.
    /// </summary>
    PowerPlayEnd = 3,

    /// <summary>
    /// A goalie has been pulled for an extra attacker.
    /// </summary>
    GoaliePulled = 4,

    /// <summary>
    /// A goalie has returned to the net.
    /// </summary>
    GoalieReturned = 5,

    /// <summary>
    /// A period has started.
    /// </summary>
    PeriodStart = 6,

    /// <summary>
    /// A period has ended.
    /// </summary>
    PeriodEnd = 7,

    /// <summary>
    /// The game has ended.
    /// </summary>
    GameEnd = 8,

    /// <summary>
    /// The game has started (first period begins).
    /// </summary>
    GameStart = 9,

    /// <summary>
    /// Your favorite team won the game.
    /// </summary>
    TeamWin = 10,

    /// <summary>
    /// Your favorite team lost the game.
    /// </summary>
    TeamLoss = 11,

    /// <summary>
    /// The game is going to overtime.
    /// </summary>
    OvertimeStart = 12,

    /// <summary>
    /// The game is going to a shootout.
    /// </summary>
    ShootoutStart = 13,

    /// <summary>
    /// Game starts in the configured time (pre-game reminder).
    /// </summary>
    GameStartingSoon = 14
}
