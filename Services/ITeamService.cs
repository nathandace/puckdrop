using PuckDrop.Models.Domain;

namespace PuckDrop.Services;

/// <summary>
/// Interface for team management operations.
/// </summary>
public interface ITeamService
{
    /// <summary>
    /// Gets the user's favorite team.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The favorite team or null if not set.</returns>
    Task<FavoriteTeam?> GetFavoriteTeamAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the user's favorite team.
    /// </summary>
    /// <param name="teamAbbrev">The team abbreviation.</param>
    /// <param name="teamName">The team name.</param>
    /// <param name="logoUrl">The team logo URL.</param>
    /// <param name="primaryColor">The team's primary color.</param>
    /// <param name="secondaryColor">The team's secondary color.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated favorite team.</returns>
    Task<FavoriteTeam> SetFavoriteTeamAsync(
        string teamAbbrev,
        string teamName,
        string? logoUrl,
        string? primaryColor,
        string? secondaryColor,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all available NHL teams.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of all NHL teams.</returns>
    Task<List<NhlTeamInfo>> GetAllTeamsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Event raised when the favorite team changes.
    /// </summary>
    event Action? OnFavoriteTeamChanged;
}

/// <summary>
/// Represents basic NHL team information for selection.
/// </summary>
public class NhlTeamInfo
{
    /// <summary>
    /// Gets or sets the team abbreviation.
    /// </summary>
    public required string Abbrev { get; set; }

    /// <summary>
    /// Gets or sets the team name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the team logo URL.
    /// </summary>
    public string? LogoUrl { get; set; }

    /// <summary>
    /// Gets or sets the team's primary color.
    /// </summary>
    public string? PrimaryColor { get; set; }

    /// <summary>
    /// Gets or sets the team's secondary color.
    /// </summary>
    public string? SecondaryColor { get; set; }

    /// <summary>
    /// Gets or sets the conference name.
    /// </summary>
    public string? Conference { get; set; }

    /// <summary>
    /// Gets or sets the division name.
    /// </summary>
    public string? Division { get; set; }
}
