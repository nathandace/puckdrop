namespace PuckDrop.Models.Domain;

/// <summary>
/// Defines the types of game events that can trigger webhooks.
/// </summary>
public enum WebhookEventType
{
    /// <summary>
    /// A goal was scored by either team.
    /// </summary>
    GoalScored = 0,

    /// <summary>
    /// A penalty was committed by either team.
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
    GameStart = 9
}
