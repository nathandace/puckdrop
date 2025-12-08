using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using PuckDrop.Models.Api;

namespace PuckDrop.Services;

/// <summary>
/// Client for interacting with the NHL API.
/// </summary>
public class NhlApiClient : INhlApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<NhlApiClient> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly TimeSpan StaticDataCacheDuration = TimeSpan.FromHours(6);
    private static readonly TimeSpan LiveDataCacheDuration = TimeSpan.FromSeconds(15);

    /// <summary>
    /// Initializes a new instance of the <see cref="NhlApiClient"/> class.
    /// </summary>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    /// <param name="cache">The memory cache.</param>
    /// <param name="logger">The logger.</param>
    public NhlApiClient(
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache,
        ILogger<NhlApiClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient("NhlApi");
        _cache = cache;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<StandingsResponse?> GetStandingsAsync(CancellationToken cancellationToken = default)
    {
        const string cacheKey = "standings";
        return await GetCachedAsync<StandingsResponse>(
            cacheKey,
            "/v1/standings/now",
            StaticDataCacheDuration,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ScheduleResponse?> GetSeasonScheduleAsync(string teamAbbrev, int season, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"schedule_{teamAbbrev}_{season}";
        var endpoint = $"/v1/club-schedule-season/{teamAbbrev}/{season}";
        return await GetCachedAsync<ScheduleResponse>(
            cacheKey,
            endpoint,
            StaticDataCacheDuration,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<GameLandingResponse?> GetGameLandingAsync(int gameId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"landing_{gameId}";
        var endpoint = $"/v1/gamecenter/{gameId}/landing";
        return await GetCachedAsync<GameLandingResponse>(
            cacheKey,
            endpoint,
            LiveDataCacheDuration,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PlayByPlayResponse?> GetPlayByPlayAsync(int gameId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"playbyplay_{gameId}";
        var endpoint = $"/v1/gamecenter/{gameId}/play-by-play";
        return await GetCachedAsync<PlayByPlayResponse>(
            cacheKey,
            endpoint,
            LiveDataCacheDuration,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<BoxscoreResponse?> GetBoxscoreAsync(int gameId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"boxscore_{gameId}";
        var endpoint = $"/v1/gamecenter/{gameId}/boxscore";
        return await GetCachedAsync<BoxscoreResponse>(
            cacheKey,
            endpoint,
            LiveDataCacheDuration,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ShiftChartResponse?> GetShiftChartAsync(int gameId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"shiftchart_{gameId}";
        var endpoint = $"/v1/gamecenter/{gameId}/shiftchart";
        return await GetCachedAsync<ShiftChartResponse>(
            cacheKey,
            endpoint,
            LiveDataCacheDuration,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<RosterResponse?> GetRosterAsync(string teamAbbrev, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"roster_{teamAbbrev}";
        var endpoint = $"/v1/roster/{teamAbbrev}/current";
        return await GetCachedAsync<RosterResponse>(
            cacheKey,
            endpoint,
            StaticDataCacheDuration,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ClubStatsResponse?> GetClubStatsAsync(string teamAbbrev, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"clubstats_{teamAbbrev}";
        var endpoint = $"/v1/club-stats/{teamAbbrev}/now";
        return await GetCachedAsync<ClubStatsResponse>(
            cacheKey,
            endpoint,
            StaticDataCacheDuration,
            cancellationToken);
    }

    /// <inheritdoc />
    public int GetCurrentSeason()
    {
        var now = DateTime.UtcNow;
        // NHL season typically starts in October and ends in June
        // If we're before September, we're in the previous season year
        var year = now.Month >= 9 ? now.Year : now.Year - 1;
        return year * 10000 + (year + 1);
    }

    /// <inheritdoc />
    public void InvalidateLiveGameCache(int gameId)
    {
        _cache.Remove($"landing_{gameId}");
        _cache.Remove($"playbyplay_{gameId}");
        _cache.Remove($"boxscore_{gameId}");
        _cache.Remove($"shiftchart_{gameId}");
    }

    /// <inheritdoc />
    public async Task<ScoreboardResponse?> GetScoreboardAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = "scoreboard_now";
        var endpoint = "/v1/score/now";
        return await GetCachedAsync<ScoreboardResponse>(
            cacheKey,
            endpoint,
            LiveDataCacheDuration,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PlayerLandingResponse?> GetPlayerLandingAsync(int playerId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"player_{playerId}";
        var endpoint = $"/v1/player/{playerId}/landing";
        return await GetCachedAsync<PlayerLandingResponse>(
            cacheKey,
            endpoint,
            StaticDataCacheDuration,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SkaterLeadersResponse?> GetSkaterLeadersAsync(string? categories = null, int limit = 10, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"skater_leaders_{categories ?? "all"}_{limit}";
        var query = $"?limit={limit}";
        if (!string.IsNullOrEmpty(categories))
        {
            query += $"&categories={categories}";
        }
        var endpoint = $"/v1/skater-stats-leaders/current{query}";
        return await GetCachedAsync<SkaterLeadersResponse>(
            cacheKey,
            endpoint,
            StaticDataCacheDuration,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<GoalieLeadersResponse?> GetGoalieLeadersAsync(string? categories = null, int limit = 10, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"goalie_leaders_{categories ?? "all"}_{limit}";
        var query = $"?limit={limit}";
        if (!string.IsNullOrEmpty(categories))
        {
            query += $"&categories={categories}";
        }
        var endpoint = $"/v1/goalie-stats-leaders/current{query}";
        return await GetCachedAsync<GoalieLeadersResponse>(
            cacheKey,
            endpoint,
            StaticDataCacheDuration,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<RightRailResponse?> GetRightRailAsync(int gameId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"rightrail_{gameId}";
        var endpoint = $"/v1/gamecenter/{gameId}/right-rail";
        return await GetCachedAsync<RightRailResponse>(
            cacheKey,
            endpoint,
            LiveDataCacheDuration,
            cancellationToken);
    }

    /// <summary>
    /// Gets data from cache or fetches from API.
    /// </summary>
    /// <typeparam name="T">The response type.</typeparam>
    /// <param name="cacheKey">The cache key.</param>
    /// <param name="endpoint">The API endpoint.</param>
    /// <param name="cacheDuration">The cache duration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response data or null.</returns>
    private async Task<T?> GetCachedAsync<T>(
        string cacheKey,
        string endpoint,
        TimeSpan cacheDuration,
        CancellationToken cancellationToken) where T : class
    {
        if (_cache.TryGetValue(cacheKey, out T? cached))
        {
            return cached;
        }

        try
        {
            _logger.LogDebug("Fetching from NHL API: {Endpoint}", endpoint);
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("NHL API returned {StatusCode} for {Endpoint}",
                    response.StatusCode, endpoint);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<T>(content, JsonOptions);

            if (result != null)
            {
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(cacheDuration);
                _cache.Set(cacheKey, result, cacheOptions);
            }

            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error fetching from NHL API: {Endpoint}", endpoint);
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing error for NHL API response: {Endpoint}", endpoint);
            return null;
        }
        catch (TaskCanceledException ex) when (ex.CancellationToken == cancellationToken)
        {
            _logger.LogDebug("Request cancelled for {Endpoint}", endpoint);
            return null;
        }
    }
}
