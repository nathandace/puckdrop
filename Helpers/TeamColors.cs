using System.Text.Json.Serialization;

namespace PuckDrop.Helpers;

/// <summary>
/// Represents an RGB color with individual R, G, B components.
/// </summary>
public class RgbColor
{
    /// <summary>
    /// Gets or sets the red component (0-255).
    /// </summary>
    [JsonPropertyName("r")]
    public int R { get; set; }

    /// <summary>
    /// Gets or sets the green component (0-255).
    /// </summary>
    [JsonPropertyName("g")]
    public int G { get; set; }

    /// <summary>
    /// Gets or sets the blue component (0-255).
    /// </summary>
    [JsonPropertyName("b")]
    public int B { get; set; }

    public RgbColor() { }

    public RgbColor(int r, int g, int b)
    {
        R = r;
        G = g;
        B = b;
    }
}

/// <summary>
/// Provides RGB color values for all NHL teams.
/// Colors are in {r, g, b} format (0-255) suitable for LED lights and smart home integrations.
/// </summary>
public static class TeamColors
{
    /// <summary>
    /// Gets the RGB colors for a team (primary, secondary, and optional tertiary).
    /// </summary>
    /// <param name="teamAbbrev">The team abbreviation.</param>
    /// <returns>List of RGB color objects.</returns>
    public static List<RgbColor> GetTeamColors(string teamAbbrev)
    {
        return teamAbbrev.ToUpperInvariant() switch
        {
            // Anaheim Ducks - Orange, Gold, Black
            "ANA" => [new(252, 76, 2), new(177, 152, 113), new(0, 0, 0)],

            // Arizona Coyotes (Utah Hockey Club) - Brick Red, Sand, Black
            "ARI" or "UTA" => [new(140, 38, 51), new(226, 214, 181), new(0, 0, 0)],

            // Boston Bruins - Gold, Black, White
            "BOS" => [new(252, 181, 20), new(0, 0, 0), new(255, 255, 255)],

            // Buffalo Sabres - Royal Blue, Gold, White
            "BUF" => [new(0, 38, 84), new(252, 181, 20), new(255, 255, 255)],

            // Calgary Flames - Red, Gold, White
            "CGY" => [new(210, 0, 28), new(250, 175, 25), new(255, 255, 255)],

            // Carolina Hurricanes - Red, Black, White
            "CAR" => [new(206, 17, 38), new(0, 0, 0), new(255, 255, 255)],

            // Chicago Blackhawks - Red, Black, White
            "CHI" => [new(207, 10, 44), new(0, 0, 0), new(255, 255, 255)],

            // Colorado Avalanche - Burgundy, Blue, White
            "COL" => [new(111, 38, 61), new(35, 97, 146), new(255, 255, 255)],

            // Columbus Blue Jackets - Blue, Red, White
            "CBJ" => [new(0, 38, 84), new(206, 17, 38), new(255, 255, 255)],

            // Dallas Stars - Victory Green, Black, White
            "DAL" => [new(0, 104, 71), new(0, 0, 0), new(255, 255, 255)],

            // Detroit Red Wings - Red, White
            "DET" => [new(206, 17, 38), new(255, 255, 255), new(206, 17, 38)],

            // Edmonton Oilers - Blue, Orange, White
            "EDM" => [new(4, 30, 66), new(252, 76, 2), new(255, 255, 255)],

            // Florida Panthers - Red, Navy, Gold
            "FLA" => [new(200, 16, 46), new(4, 30, 66), new(185, 151, 91)],

            // Los Angeles Kings - Silver, Black, White
            "LAK" => [new(162, 170, 173), new(0, 0, 0), new(255, 255, 255)],

            // Minnesota Wild - Forest Green, Red, Gold
            "MIN" => [new(2, 73, 48), new(175, 35, 36), new(237, 170, 0)],

            // Montreal Canadiens - Red, Blue, White
            "MTL" => [new(175, 30, 45), new(25, 33, 104), new(255, 255, 255)],

            // Nashville Predators - Gold, Navy, White
            "NSH" => [new(255, 184, 28), new(4, 30, 66), new(255, 255, 255)],

            // New Jersey Devils - Red, Black, White
            "NJD" => [new(206, 17, 38), new(0, 0, 0), new(255, 255, 255)],

            // New York Islanders - Blue, Orange, White
            "NYI" => [new(0, 83, 155), new(244, 125, 48), new(255, 255, 255)],

            // New York Rangers - Blue, Red, White
            "NYR" => [new(0, 56, 168), new(206, 17, 38), new(255, 255, 255)],

            // Ottawa Senators - Red, Gold, Black
            "OTT" => [new(200, 16, 46), new(198, 146, 20), new(0, 0, 0)],

            // Philadelphia Flyers - Orange, Black, White
            "PHI" => [new(247, 73, 2), new(0, 0, 0), new(255, 255, 255)],

            // Pittsburgh Penguins - Gold, Black, White
            "PIT" => [new(252, 181, 20), new(0, 0, 0), new(255, 255, 255)],

            // San Jose Sharks - Teal, Black, White
            "SJS" => [new(0, 109, 117), new(0, 0, 0), new(255, 255, 255)],

            // Seattle Kraken - Deep Sea Blue, Ice Blue, Red
            "SEA" => [new(0, 22, 40), new(153, 217, 217), new(227, 79, 65)],

            // St. Louis Blues - Blue, Gold, White
            "STL" => [new(0, 47, 135), new(252, 181, 20), new(255, 255, 255)],

            // Tampa Bay Lightning - Blue, White
            "TBL" => [new(0, 40, 104), new(255, 255, 255), new(0, 40, 104)],

            // Toronto Maple Leafs - Blue, White
            "TOR" => [new(0, 32, 91), new(255, 255, 255), new(0, 32, 91)],

            // Vancouver Canucks - Blue, Green, White
            "VAN" => [new(0, 32, 91), new(10, 134, 61), new(255, 255, 255)],

            // Vegas Golden Knights - Gold, Steel Gray, Black
            "VGK" => [new(185, 151, 91), new(51, 63, 72), new(0, 0, 0)],

            // Washington Capitals - Red, Navy, White
            "WSH" => [new(200, 16, 46), new(4, 30, 66), new(255, 255, 255)],

            // Winnipeg Jets - Navy, Aviator Blue, White
            "WPG" => [new(4, 30, 66), new(0, 76, 151), new(255, 255, 255)],

            // Default - White flash
            _ => [new(255, 255, 255), new(200, 200, 200), new(255, 255, 255)]
        };
    }

    /// <summary>
    /// Gets the primary team color as a hex string (for Discord embeds, etc).
    /// </summary>
    /// <param name="teamAbbrev">The team abbreviation.</param>
    /// <returns>Hex color string without # prefix.</returns>
    public static string GetPrimaryColorHex(string teamAbbrev)
    {
        var colors = GetTeamColors(teamAbbrev);
        if (colors.Count == 0) return "FFFFFF";
        var rgb = colors[0];
        return $"{rgb.R:X2}{rgb.G:X2}{rgb.B:X2}";
    }

    /// <summary>
    /// Gets the primary team color as a Discord-compatible integer.
    /// </summary>
    /// <param name="teamAbbrev">The team abbreviation.</param>
    /// <returns>Integer color value.</returns>
    public static int GetPrimaryColorInt(string teamAbbrev)
    {
        var colors = GetTeamColors(teamAbbrev);
        if (colors.Count == 0) return 0xFFFFFF;
        var rgb = colors[0];
        return (rgb.R << 16) | (rgb.G << 8) | rgb.B;
    }
}
