using Microsoft.EntityFrameworkCore;
using PuckDrop.Data;
using PuckDrop.Models.Domain;

namespace PuckDrop.Services;

/// <summary>
/// Service for managing team data and favorite team selection.
/// </summary>
public class TeamService : ITeamService
{
    private readonly IDbContextFactory<NhlDbContext> _dbContextFactory;
    private readonly INhlApiClient _nhlApiClient;
    private readonly ILogger<TeamService> _logger;

    /// <inheritdoc />
    public event Action? OnFavoriteTeamChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="TeamService"/> class.
    /// </summary>
    /// <param name="dbContextFactory">The database context factory.</param>
    /// <param name="nhlApiClient">The NHL API client.</param>
    /// <param name="logger">The logger.</param>
    public TeamService(
        IDbContextFactory<NhlDbContext> dbContextFactory,
        INhlApiClient nhlApiClient,
        ILogger<TeamService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _nhlApiClient = nhlApiClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<FavoriteTeam?> GetFavoriteTeamAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.FavoriteTeams.FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<FavoriteTeam> SetFavoriteTeamAsync(
        string teamAbbrev,
        string teamName,
        string? logoUrl,
        string? primaryColor,
        string? secondaryColor,
        CancellationToken cancellationToken = default)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var existing = await context.FavoriteTeams.FirstOrDefaultAsync(cancellationToken);

        if (existing != null)
        {
            existing.TeamAbbrev = teamAbbrev;
            existing.TeamName = teamName;
            existing.LogoUrl = logoUrl;
            existing.PrimaryColor = primaryColor;
            existing.SecondaryColor = secondaryColor;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            existing = new FavoriteTeam
            {
                TeamAbbrev = teamAbbrev,
                TeamName = teamName,
                LogoUrl = logoUrl,
                PrimaryColor = primaryColor,
                SecondaryColor = secondaryColor,
                UpdatedAt = DateTime.UtcNow
            };
            context.FavoriteTeams.Add(existing);
        }

        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Favorite team set to {TeamName} ({TeamAbbrev})", teamName, teamAbbrev);
        OnFavoriteTeamChanged?.Invoke();

        return existing;
    }

    /// <inheritdoc />
    public async Task<List<NhlTeamInfo>> GetAllTeamsAsync(CancellationToken cancellationToken = default)
    {
        var standings = await _nhlApiClient.GetStandingsAsync(cancellationToken);

        if (standings?.Standings == null || standings.Standings.Count == 0)
        {
            _logger.LogWarning("Could not fetch teams from NHL API, using default list");
            return GetDefaultTeamList();
        }

        return standings.Standings.Select(s => new NhlTeamInfo
        {
            Abbrev = s.TeamAbbrev.Default,
            Name = $"{s.PlaceName.Default} {s.TeamCommonName.Default}",
            LogoUrl = s.TeamLogo,
            Conference = s.ConferenceName,
            Division = s.DivisionName
        }).OrderBy(t => t.Name).ToList();
    }

    /// <summary>
    /// Returns a default list of NHL teams if the API is unavailable.
    /// </summary>
    /// <returns>List of default NHL teams.</returns>
    private static List<NhlTeamInfo> GetDefaultTeamList()
    {
        return new List<NhlTeamInfo>
        {
            new() { Abbrev = "ANA", Name = "Anaheim Ducks", PrimaryColor = "#F47A38" },
            new() { Abbrev = "ARI", Name = "Arizona Coyotes", PrimaryColor = "#8C2633" },
            new() { Abbrev = "BOS", Name = "Boston Bruins", PrimaryColor = "#FFB81C" },
            new() { Abbrev = "BUF", Name = "Buffalo Sabres", PrimaryColor = "#002654" },
            new() { Abbrev = "CGY", Name = "Calgary Flames", PrimaryColor = "#D2001C" },
            new() { Abbrev = "CAR", Name = "Carolina Hurricanes", PrimaryColor = "#CC0000" },
            new() { Abbrev = "CHI", Name = "Chicago Blackhawks", PrimaryColor = "#CF0A2C" },
            new() { Abbrev = "COL", Name = "Colorado Avalanche", PrimaryColor = "#6F263D" },
            new() { Abbrev = "CBJ", Name = "Columbus Blue Jackets", PrimaryColor = "#002654" },
            new() { Abbrev = "DAL", Name = "Dallas Stars", PrimaryColor = "#006847" },
            new() { Abbrev = "DET", Name = "Detroit Red Wings", PrimaryColor = "#CE1126" },
            new() { Abbrev = "EDM", Name = "Edmonton Oilers", PrimaryColor = "#041E42" },
            new() { Abbrev = "FLA", Name = "Florida Panthers", PrimaryColor = "#041E42" },
            new() { Abbrev = "LAK", Name = "Los Angeles Kings", PrimaryColor = "#111111" },
            new() { Abbrev = "MIN", Name = "Minnesota Wild", PrimaryColor = "#154734" },
            new() { Abbrev = "MTL", Name = "Montreal Canadiens", PrimaryColor = "#AF1E2D" },
            new() { Abbrev = "NSH", Name = "Nashville Predators", PrimaryColor = "#FFB81C" },
            new() { Abbrev = "NJD", Name = "New Jersey Devils", PrimaryColor = "#CE1126" },
            new() { Abbrev = "NYI", Name = "New York Islanders", PrimaryColor = "#00539B" },
            new() { Abbrev = "NYR", Name = "New York Rangers", PrimaryColor = "#0038A8" },
            new() { Abbrev = "OTT", Name = "Ottawa Senators", PrimaryColor = "#C52032" },
            new() { Abbrev = "PHI", Name = "Philadelphia Flyers", PrimaryColor = "#F74902" },
            new() { Abbrev = "PIT", Name = "Pittsburgh Penguins", PrimaryColor = "#FCB514" },
            new() { Abbrev = "SJS", Name = "San Jose Sharks", PrimaryColor = "#006D75" },
            new() { Abbrev = "SEA", Name = "Seattle Kraken", PrimaryColor = "#001628" },
            new() { Abbrev = "STL", Name = "St. Louis Blues", PrimaryColor = "#002F87" },
            new() { Abbrev = "TBL", Name = "Tampa Bay Lightning", PrimaryColor = "#002868" },
            new() { Abbrev = "TOR", Name = "Toronto Maple Leafs", PrimaryColor = "#00205B" },
            new() { Abbrev = "UTA", Name = "Utah Hockey Club", PrimaryColor = "#69B3E7" },
            new() { Abbrev = "VAN", Name = "Vancouver Canucks", PrimaryColor = "#00205B" },
            new() { Abbrev = "VGK", Name = "Vegas Golden Knights", PrimaryColor = "#B4975A" },
            new() { Abbrev = "WSH", Name = "Washington Capitals", PrimaryColor = "#041E42" },
            new() { Abbrev = "WPG", Name = "Winnipeg Jets", PrimaryColor = "#041E42" }
        };
    }
}
