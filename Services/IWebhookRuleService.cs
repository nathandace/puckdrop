using PuckDrop.Models.Domain;

namespace PuckDrop.Services;

/// <summary>
/// Interface for webhook rule management operations.
/// </summary>
public interface IWebhookRuleService
{
    /// <summary>
    /// Gets all webhook rules.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of webhook rules.</returns>
    Task<List<WebhookRule>> GetAllRulesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a webhook rule by ID.
    /// </summary>
    /// <param name="id">The rule ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The webhook rule or null.</returns>
    Task<WebhookRule?> GetRuleByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new webhook rule.
    /// </summary>
    /// <param name="rule">The rule to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created rule.</returns>
    Task<WebhookRule> CreateRuleAsync(WebhookRule rule, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing webhook rule.
    /// </summary>
    /// <param name="rule">The rule to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated rule.</returns>
    Task<WebhookRule> UpdateRuleAsync(WebhookRule rule, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a webhook rule.
    /// </summary>
    /// <param name="id">The rule ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted.</returns>
    Task<bool> DeleteRuleAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets enabled rules for a specific event type and team.
    /// </summary>
    /// <param name="eventType">The event type.</param>
    /// <param name="teamAbbrev">The team abbreviation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of matching rules.</returns>
    Task<List<WebhookRule>> GetEnabledRulesForEventAsync(WebhookEventType eventType, string teamAbbrev, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all distinct team abbreviations that have enabled webhook rules.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of team abbreviations.</returns>
    Task<List<string>> GetTeamsWithEnabledRulesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all enabled rules for a team, grouped by event type.
    /// Optimized for batch processing to reduce database round-trips.
    /// </summary>
    /// <param name="teamAbbrev">The team abbreviation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Dictionary mapping event types to their enabled rules.</returns>
    Task<Dictionary<WebhookEventType, List<WebhookRule>>> GetAllEnabledRulesForTeamAsync(string teamAbbrev, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets webhook logs for a specific rule.
    /// </summary>
    /// <param name="ruleId">The rule ID.</param>
    /// <param name="limit">Maximum number of logs to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of webhook logs.</returns>
    Task<List<WebhookLog>> GetLogsForRuleAsync(int ruleId, int limit = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all webhook logs with optional filtering.
    /// </summary>
    /// <param name="ruleId">Optional rule ID to filter by.</param>
    /// <param name="limit">Maximum number of logs to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of webhook logs with rule information.</returns>
    Task<List<WebhookLog>> GetAllLogsAsync(int? ruleId = null, int limit = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cleans up webhook logs older than the specified number of days.
    /// </summary>
    /// <param name="daysToKeep">Number of days of logs to keep.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of logs deleted.</returns>
    Task<int> CleanupOldLogsAsync(int daysToKeep = 14, CancellationToken cancellationToken = default);
}
