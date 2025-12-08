using Microsoft.EntityFrameworkCore;
using PuckDrop.Data;
using PuckDrop.Models.Domain;

namespace PuckDrop.Services;

/// <summary>
/// Service for managing webhook rules.
/// </summary>
public class WebhookRuleService : IWebhookRuleService
{
    private readonly IDbContextFactory<NhlDbContext> _dbContextFactory;
    private readonly ILogger<WebhookRuleService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebhookRuleService"/> class.
    /// </summary>
    /// <param name="dbContextFactory">The database context factory.</param>
    /// <param name="logger">The logger.</param>
    public WebhookRuleService(IDbContextFactory<NhlDbContext> dbContextFactory, ILogger<WebhookRuleService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<WebhookRule>> GetAllRulesAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.WebhookRules.OrderBy(r => r.EventType).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WebhookRule?> GetRuleByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.WebhookRules.FindAsync(new object[] { id }, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WebhookRule> CreateRuleAsync(WebhookRule rule, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        rule.CreatedAt = DateTime.UtcNow;
        rule.UpdatedAt = DateTime.UtcNow;

        context.WebhookRules.Add(rule);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created webhook rule {RuleId} for event {EventType}", rule.Id, rule.EventType);
        return rule;
    }

    /// <inheritdoc />
    public async Task<WebhookRule> UpdateRuleAsync(WebhookRule rule, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var existing = await context.WebhookRules.FindAsync(new object[] { rule.Id }, cancellationToken);
        if (existing == null)
        {
            throw new InvalidOperationException($"Webhook rule {rule.Id} not found");
        }

        existing.TeamAbbrev = rule.TeamAbbrev;
        existing.EventType = rule.EventType;
        existing.TargetUrl = rule.TargetUrl;
        existing.PayloadFormat = rule.PayloadFormat;
        existing.IsEnabled = rule.IsEnabled;
        existing.Name = rule.Name;
        existing.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated webhook rule {RuleId}", rule.Id);
        return existing;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteRuleAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var rule = await context.WebhookRules.FindAsync(new object[] { id }, cancellationToken);
        if (rule == null)
        {
            return false;
        }

        context.WebhookRules.Remove(rule);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted webhook rule {RuleId}", id);
        return true;
    }

    /// <inheritdoc />
    public async Task<List<WebhookRule>> GetEnabledRulesForEventAsync(WebhookEventType eventType, string teamAbbrev, CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.WebhookRules
            .Where(r => r.IsEnabled && r.EventType == eventType && r.TeamAbbrev == teamAbbrev)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<string>> GetTeamsWithEnabledRulesAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.WebhookRules
            .Where(r => r.IsEnabled)
            .Select(r => r.TeamAbbrev)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}
