namespace PuckDrop.Models.Domain;

/// <summary>
/// Represents the user's selected favorite NHL team.
/// </summary>
public class FavoriteTeam
{
    /// <summary>
    /// Gets or sets the unique identifier for this record.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the three-letter abbreviation for the team (e.g., "TOR", "BOS").
    /// </summary>
    public required string TeamAbbrev { get; set; }

    /// <summary>
    /// Gets or sets the full name of the team (e.g., "Toronto Maple Leafs").
    /// </summary>
    public required string TeamName { get; set; }

    /// <summary>
    /// Gets or sets the URL to the team's logo image.
    /// </summary>
    public string? LogoUrl { get; set; }

    /// <summary>
    /// Gets or sets the primary team color in hex format (e.g., "#00205B").
    /// </summary>
    public string? PrimaryColor { get; set; }

    /// <summary>
    /// Gets or sets the secondary team color in hex format.
    /// </summary>
    public string? SecondaryColor { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this record was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
