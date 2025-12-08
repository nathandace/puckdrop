using PuckDrop.Models.Api;

namespace PuckDrop.Services;

/// <summary>
/// Interface for processing game events and triggering webhooks.
/// </summary>
public interface IEventProcessingService
{
    /// <summary>
    /// Processes play-by-play data and triggers webhooks for new events.
    /// </summary>
    /// <param name="gameId">The game ID.</param>
    /// <param name="playByPlay">The play-by-play response.</param>
    /// <param name="landing">The game landing response for context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of events processed.</returns>
    Task<int> ProcessGameEventsAsync(
        int gameId,
        PlayByPlayResponse playByPlay,
        GameLandingResponse landing,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cleans up old processed events from the database.
    /// </summary>
    /// <param name="olderThan">Remove events older than this date.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of events removed.</returns>
    Task<int> CleanupOldEventsAsync(DateTime olderThan, CancellationToken cancellationToken = default);
}
