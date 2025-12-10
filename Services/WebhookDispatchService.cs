using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using PuckDrop.Data;
using PuckDrop.Helpers;
using PuckDrop.Models.Domain;
using PuckDrop.Models.Webhooks;

namespace PuckDrop.Services;

/// <summary>
/// Service for dispatching webhooks to configured endpoints.
/// </summary>
public class WebhookDispatchService : IWebhookDispatchService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDbContextFactory<NhlDbContext> _dbContextFactory;
    private readonly IWebhookNotificationService _notificationService;
    private readonly ILogger<WebhookDispatchService> _logger;

    private const int MaxRetries = 3;
    private const int TimeoutSeconds = 10;
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(2);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="WebhookDispatchService"/> class.
    /// </summary>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    /// <param name="dbContextFactory">The database context factory.</param>
    /// <param name="notificationService">The webhook notification service.</param>
    /// <param name="logger">The logger.</param>
    public WebhookDispatchService(
        IHttpClientFactory httpClientFactory,
        IDbContextFactory<NhlDbContext> dbContextFactory,
        IWebhookNotificationService notificationService,
        ILogger<WebhookDispatchService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _dbContextFactory = dbContextFactory;
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<bool> SendWebhookAsync(WebhookRule rule, WebhookPayload payload, CancellationToken cancellationToken = default)
    {
        // Apply configured delay for broadcast/streaming sync
        if (rule.DelaySeconds > 0)
        {
            _logger.LogDebug("Delaying webhook for {Delay}s (rule: {RuleName})",
                rule.DelaySeconds, rule.Name ?? $"#{rule.Id}");
            await Task.Delay(TimeSpan.FromSeconds(rule.DelaySeconds), cancellationToken);
        }

        // Ensure team colors are populated based on the webhook rule's team
        EnsureTeamColors(payload, rule.TeamAbbrev);

        // Use custom template if provided, otherwise use standard format
        var formattedPayload = !string.IsNullOrEmpty(rule.CustomPayloadTemplate)
            ? ApplyCustomTemplate(rule.CustomPayloadTemplate, payload)
            : FormatPayload(rule.PayloadFormat, payload);

        var (success, statusCode, errorMessage) = await SendWithRetryAndStatusAsync(rule.TargetUrl, formattedPayload, cancellationToken);

        // Log the webhook attempt
        await LogWebhookAsync(rule, payload, success, statusCode, errorMessage, cancellationToken);

        return success;
    }

    /// <inheritdoc />
    public async Task<bool> SendTestWebhookAsync(WebhookRule rule, CancellationToken cancellationToken = default)
    {
        // Use the team's actual colors for test
        var teamColors = TeamColors.GetTeamColors(rule.TeamAbbrev);

        var testPayload = new WebhookPayload
        {
            EventType = rule.EventType.ToString(),
            Timestamp = DateTime.UtcNow,
            GameId = 0,
            Period = 1,
            TimeInPeriod = "20:00",
            HomeTeam = rule.TeamAbbrev,
            AwayTeam = "OPP",
            HomeScore = 1,
            AwayScore = 0,
            TeamColors = teamColors,
            Details = new WebhookEventDetails
            {
                Team = rule.TeamAbbrev,
                Player = "Test Player",
                JerseyNumber = 99
            }
        };

        var formattedPayload = !string.IsNullOrEmpty(rule.CustomPayloadTemplate)
            ? ApplyCustomTemplate(rule.CustomPayloadTemplate, testPayload)
            : FormatPayload(rule.PayloadFormat, testPayload);

        var (success, _, _) = await SendWithRetryAndStatusAsync(rule.TargetUrl, formattedPayload, cancellationToken);
        return success;
    }

    /// <summary>
    /// Gets a preview of the payload that would be sent for a webhook rule.
    /// </summary>
    /// <param name="rule">The webhook rule.</param>
    /// <returns>JSON string preview of the payload.</returns>
    public string GetPayloadPreview(WebhookRule rule)
    {
        var teamColors = TeamColors.GetTeamColors(rule.TeamAbbrev);

        var samplePayload = new WebhookPayload
        {
            EventType = rule.EventType.ToString(),
            Timestamp = DateTime.UtcNow,
            GameId = 2024020001,
            Period = 2,
            TimeInPeriod = "15:30",
            HomeTeam = rule.TeamAbbrev,
            AwayTeam = "OPP",
            HomeScore = 2,
            AwayScore = 1,
            TeamColors = teamColors,
            Details = new WebhookEventDetails
            {
                Team = rule.TeamAbbrev,
                Player = "Sample Player",
                JerseyNumber = 91,
                Assists = rule.EventType == WebhookEventType.GoalScored ? new List<string> { "Assist 1", "Assist 2" } : null,
                GoalType = rule.EventType == WebhookEventType.GoalScored ? "ev" : null,
                PenaltyType = rule.EventType == WebhookEventType.PenaltyCommitted ? "Hooking" : null,
                PenaltyMinutes = rule.EventType == WebhookEventType.PenaltyCommitted ? 2 : null
            }
        };

        if (!string.IsNullOrEmpty(rule.CustomPayloadTemplate))
        {
            return ApplyCustomTemplate(rule.CustomPayloadTemplate, samplePayload);
        }

        return FormatPayload(rule.PayloadFormat, samplePayload);
    }

    /// <summary>
    /// Formats the payload based on the target format.
    /// </summary>
    /// <param name="format">The payload format.</param>
    /// <param name="payload">The generic payload.</param>
    /// <returns>The formatted JSON string.</returns>
    private static string FormatPayload(WebhookPayloadFormat format, WebhookPayload payload)
    {
        return format switch
        {
            WebhookPayloadFormat.Discord => FormatDiscordPayload(payload),
            WebhookPayloadFormat.HomeAssistant => FormatHomeAssistantPayload(payload),
            _ => JsonSerializer.Serialize(payload, JsonOptions)
        };
    }

    /// <summary>
    /// Formats a Discord webhook payload.
    /// </summary>
    /// <param name="payload">The generic payload.</param>
    /// <returns>Discord-formatted JSON string.</returns>
    private static string FormatDiscordPayload(WebhookPayload payload)
    {
        var title = GetEventTitle(payload.EventType);
        var color = GetEventColor(payload.EventType);
        var description = GetEventDescription(payload);

        var embed = new DiscordEmbed
        {
            Title = title,
            Description = description,
            Color = color,
            Fields = new List<DiscordEmbedField>
            {
                new() { Name = "Score", Value = $"{payload.AwayTeam} {payload.AwayScore} - {payload.HomeScore} {payload.HomeTeam}", Inline = true },
                new() { Name = "Period", Value = $"P{payload.Period}", Inline = true },
                new() { Name = "Time", Value = payload.TimeInPeriod ?? "00:00", Inline = true }
            },
            Footer = new DiscordEmbedFooter { Text = "NHL Monitor" },
            Timestamp = payload.Timestamp.ToString("o")
        };

        var discordPayload = new DiscordWebhookPayload
        {
            Embeds = new List<DiscordEmbed> { embed }
        };

        return JsonSerializer.Serialize(discordPayload, JsonOptions);
    }

    /// <summary>
    /// Formats a Home Assistant webhook payload.
    /// </summary>
    /// <param name="payload">The generic payload.</param>
    /// <returns>Home Assistant-formatted JSON string.</returns>
    private static string FormatHomeAssistantPayload(WebhookPayload payload)
    {
        var haPayload = new HomeAssistantPayload
        {
            EventType = $"nhl_{payload.EventType.ToLowerInvariant()}",
            Data = new Dictionary<string, object>
            {
                ["game_id"] = payload.GameId,
                ["home_team"] = payload.HomeTeam,
                ["away_team"] = payload.AwayTeam,
                ["home_score"] = payload.HomeScore,
                ["away_score"] = payload.AwayScore,
                ["period"] = payload.Period,
                ["time_in_period"] = payload.TimeInPeriod ?? "00:00"
            }
        };

        // Add team colors for LED/smart home integrations
        if (payload.TeamColors != null)
        {
            haPayload.Data["team_colors"] = payload.TeamColors;
        }

        if (payload.Details != null)
        {
            if (!string.IsNullOrEmpty(payload.Details.Team))
            {
                haPayload.Data["team"] = payload.Details.Team;
            }
            if (!string.IsNullOrEmpty(payload.Details.Player))
            {
                haPayload.Data["player"] = payload.Details.Player;
            }
        }

        return JsonSerializer.Serialize(haPayload, JsonOptions);
    }

    /// <summary>
    /// Gets the event title for Discord embeds.
    /// </summary>
    /// <param name="eventType">The event type.</param>
    /// <returns>Event title string.</returns>
    private static string GetEventTitle(string eventType)
    {
        return eventType switch
        {
            "GoalScored" => "GOAL!",
            "PenaltyCommitted" => "Penalty",
            "PowerPlayStart" => "Power Play",
            "PowerPlayEnd" => "Power Play Over",
            "GoaliePulled" => "Goalie Pulled",
            "GoalieReturned" => "Goalie Returned",
            "PeriodStart" => "Period Started",
            "PeriodEnd" => "Period Ended",
            "GameStart" => "Game Starting",
            "GameEnd" => "Game Over",
            _ => eventType
        };
    }

    /// <summary>
    /// Gets the embed color for Discord webhooks.
    /// </summary>
    /// <param name="eventType">The event type.</param>
    /// <returns>Embed color as integer.</returns>
    private static int GetEventColor(string eventType)
    {
        return eventType switch
        {
            "GoalScored" => 0x00FF00, // Green
            "PenaltyCommitted" => 0xFF0000, // Red
            "PowerPlayStart" => 0xFFFF00, // Yellow
            "PowerPlayEnd" => 0x808080, // Gray
            "GoaliePulled" => 0xFF9800, // Orange
            "GoalieReturned" => 0x2196F3, // Blue
            "PeriodStart" => 0x2196F3, // Blue
            "PeriodEnd" => 0x808080, // Gray
            "GameStart" => 0x4CAF50, // Green
            "GameEnd" => 0x9C27B0, // Purple
            _ => 0x808080 // Default gray
        };
    }

    /// <summary>
    /// Gets the event description for Discord embeds.
    /// </summary>
    /// <param name="payload">The webhook payload.</param>
    /// <returns>Event description string.</returns>
    private static string GetEventDescription(WebhookPayload payload)
    {
        if (payload.Details == null)
        {
            return "";
        }

        return payload.EventType switch
        {
            "GoalScored" => $"#{payload.Details.JerseyNumber} {payload.Details.Player} ({payload.Details.Team})" +
                           (payload.Details.Assists?.Count > 0 ? $"\nAssists: {string.Join(", ", payload.Details.Assists)}" : ""),
            "PenaltyCommitted" => $"{payload.Details.PenaltyType} - {payload.Details.PenaltyMinutes} min\n{payload.Details.Player} ({payload.Details.Team})",
            "PowerPlayStart" => $"{payload.Details.Team} Power Play ({payload.Details.Strength})",
            "GoaliePulled" => $"{payload.Details.Team} has pulled their goalie",
            "GoalieReturned" => $"{payload.Details.Team} goalie has returned",
            _ => ""
        };
    }

    /// <summary>
    /// Logs a webhook attempt to the database and sends a real-time notification.
    /// </summary>
    private async Task LogWebhookAsync(
        WebhookRule rule,
        WebhookPayload payload,
        bool success,
        int? statusCode,
        string? errorMessage,
        CancellationToken cancellationToken)
    {
        var eventDescription = BuildEventDescription(payload);
        var triggeredAt = DateTime.UtcNow;
        int logId = 0;

        try
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var log = new WebhookLog
            {
                WebhookRuleId = rule.Id,
                EventType = rule.EventType,
                GameId = payload.GameId > 0 ? payload.GameId : null,
                Success = success,
                HttpStatusCode = statusCode,
                ErrorMessage = errorMessage,
                TriggeredAt = triggeredAt,
                EventDescription = eventDescription
            };

            context.WebhookLogs.Add(log);
            await context.SaveChangesAsync(cancellationToken);
            logId = log.Id;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to log webhook attempt for rule {RuleId}", rule.Id);
        }

        // Send real-time notification to connected clients
        try
        {
            var notification = new WebhookLogNotification
            {
                RuleId = rule.Id,
                LogId = logId,
                Success = success,
                EventDescription = eventDescription,
                ErrorMessage = errorMessage,
                TriggeredAt = triggeredAt
            };

            await _notificationService.NotifyWebhookFiredAsync(notification, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send webhook notification for rule {RuleId}", rule.Id);
        }
    }

    /// <summary>
    /// Builds a brief event description for logging.
    /// </summary>
    private static string BuildEventDescription(WebhookPayload payload)
    {
        var team = payload.Details?.Team ?? "";
        var player = payload.Details?.Player ?? "";

        return payload.EventType switch
        {
            "GoalScored" or "FavoriteTeamGoal" or "OpponentGoal" =>
                !string.IsNullOrEmpty(player) ? $"Goal by {player}" : $"Goal ({team})",
            "PenaltyCommitted" =>
                !string.IsNullOrEmpty(player) ? $"Penalty: {player}" : $"Penalty ({team})",
            "PowerPlayStart" => $"Power Play: {team}",
            "PowerPlayEnd" => $"PP End: {team}",
            "GoaliePulled" => $"Goalie Pulled: {team}",
            "GoalieReturned" => $"Goalie Returned: {team}",
            "PeriodStart" => $"Period {payload.Period} Start",
            "PeriodEnd" => $"Period {payload.Period} End",
            "GameStart" => $"{payload.AwayTeam} @ {payload.HomeTeam}",
            "GameEnd" => $"Final: {payload.AwayTeam} {payload.AwayScore} - {payload.HomeScore} {payload.HomeTeam}",
            "OvertimeStart" => "Overtime Start",
            "ShootoutStart" => "Shootout Start",
            "TeamWin" => $"{team} Win",
            "TeamLoss" => $"{team} Loss",
            _ => payload.EventType
        };
    }

    /// <summary>
    /// Sends a webhook with retry logic and returns status information.
    /// </summary>
    /// <param name="url">The target URL.</param>
    /// <param name="jsonPayload">The JSON payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Tuple of (success, statusCode, errorMessage).</returns>
    private async Task<(bool Success, int? StatusCode, string? ErrorMessage)> SendWithRetryAndStatusAsync(
        string url, string jsonPayload, CancellationToken cancellationToken)
    {
        if (!ValidateUrl(url))
        {
            _logger.LogError("Invalid webhook URL: {Url}", url);
            return (false, null, "Invalid URL");
        }

        using var httpClient = _httpClientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromSeconds(TimeoutSeconds);

        int? lastStatusCode = null;
        string? lastError = null;

        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(url, content, cancellationToken);

                lastStatusCode = (int)response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogDebug("Webhook sent successfully to {Url}", url);
                    return (true, lastStatusCode, null);
                }

                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                lastError = $"HTTP {(int)response.StatusCode}: {responseBody}";
                _logger.LogWarning(
                    "Webhook failed (attempt {Attempt}/{MaxRetries}): {StatusCode} - {Url} - {Response}",
                    attempt, MaxRetries, response.StatusCode, url, responseBody);
            }
            catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return (false, null, "Cancelled");
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
                _logger.LogWarning(ex,
                    "Webhook error (attempt {Attempt}/{MaxRetries}): {Url}",
                    attempt, MaxRetries, url);
            }

            if (attempt < MaxRetries)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return (false, null, "Cancelled");
                }

                try
                {
                    await Task.Delay(RetryDelay * attempt, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return (false, null, "Cancelled");
                }
            }
        }

        _logger.LogError("Webhook failed after {MaxRetries} attempts: {Url}", MaxRetries, url);
        return (false, lastStatusCode, lastError);
    }

    /// <summary>
    /// Validates that the URL is a valid HTTP or HTTPS URL.
    /// </summary>
    /// <param name="url">The URL to validate.</param>
    /// <returns>True if valid.</returns>
    private static bool ValidateUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return false;
        }

        return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
    }

    /// <summary>
    /// Ensures team colors are populated in the payload.
    /// </summary>
    /// <param name="payload">The payload to update.</param>
    /// <param name="favoriteTeam">The user's favorite team from the webhook rule.</param>
    private static void EnsureTeamColors(WebhookPayload payload, string favoriteTeam)
    {
        // Set team colors based on the user's favorite team (from webhook rule)
        if (payload.TeamColors == null && !string.IsNullOrEmpty(favoriteTeam))
        {
            payload.TeamColors = TeamColors.GetTeamColors(favoriteTeam);
        }
    }

    /// <summary>
    /// Applies a custom template with placeholder substitution.
    /// Supported placeholders: {{eventType}}, {{timestamp}}, {{gameId}}, {{period}}, {{timeInPeriod}},
    /// {{homeTeam}}, {{awayTeam}}, {{homeScore}}, {{awayScore}}, {{team}}, {{player}}, {{jerseyNumber}},
    /// {{team_colors}}, {{payload}} (full JSON)
    /// </summary>
    /// <param name="template">The template string.</param>
    /// <param name="payload">The payload data.</param>
    /// <returns>Processed template string.</returns>
    private static string ApplyCustomTemplate(string template, WebhookPayload payload)
    {
        var result = template;

        // Basic fields
        result = result.Replace("{{eventType}}", payload.EventType);
        result = result.Replace("{{timestamp}}", payload.Timestamp.ToString("o"));
        result = result.Replace("{{gameId}}", payload.GameId.ToString());
        result = result.Replace("{{period}}", payload.Period.ToString());
        result = result.Replace("{{timeInPeriod}}", payload.TimeInPeriod ?? "");
        result = result.Replace("{{homeTeam}}", payload.HomeTeam);
        result = result.Replace("{{awayTeam}}", payload.AwayTeam);
        result = result.Replace("{{homeScore}}", payload.HomeScore.ToString());
        result = result.Replace("{{awayScore}}", payload.AwayScore.ToString());

        // Detail fields
        result = result.Replace("{{team}}", payload.Details?.Team ?? "");
        result = result.Replace("{{player}}", payload.Details?.Player ?? "");
        result = result.Replace("{{jerseyNumber}}", payload.Details?.JerseyNumber?.ToString() ?? "");

        // Team colors as JSON array of objects
        result = result.Replace("{{team_colors}}", JsonSerializer.Serialize(payload.TeamColors ?? new List<RgbColor>()));

        // Full payload as JSON (for nesting in custom structure)
        result = result.Replace("{{payload}}", JsonSerializer.Serialize(payload, JsonOptions));

        return result;
    }
}
