namespace PuckDrop.Services;

/// <summary>
/// Helper class for getting team logo URLs.
/// </summary>
public static class TeamLogoHelper
{
    /// <summary>
    /// Gets the local logo URL for a team.
    /// </summary>
    /// <param name="teamAbbrev">The team abbreviation (e.g., "TOR", "BOS").</param>
    /// <returns>The URL path to the team's SVG logo.</returns>
    public static string GetLogoUrl(string? teamAbbrev)
    {
        if (string.IsNullOrEmpty(teamAbbrev))
            return "/logos/NHL.svg";

        return $"/logos/{teamAbbrev.ToUpperInvariant()}.svg";
    }

    /// <summary>
    /// Gets the local logo URL for a team, with fallback to NHL API if local not found.
    /// </summary>
    /// <param name="teamAbbrev">The team abbreviation.</param>
    /// <param name="nhlApiLogoUrl">The NHL API logo URL as fallback.</param>
    /// <returns>The logo URL.</returns>
    public static string GetLogoUrl(string? teamAbbrev, string? nhlApiLogoUrl)
    {
        if (string.IsNullOrEmpty(teamAbbrev))
            return nhlApiLogoUrl ?? "/logos/NHL.svg";

        return $"/logos/{teamAbbrev.ToUpperInvariant()}.svg";
    }

    /// <summary>
    /// All NHL team abbreviations.
    /// </summary>
    public static readonly string[] AllTeamAbbrevs =
    [
        "ANA", "BOS", "BUF", "CAR", "CBJ", "CGY", "CHI", "COL",
        "DAL", "DET", "EDM", "FLA", "LAK", "MIN", "MTL", "NJD",
        "NSH", "NYI", "NYR", "OTT", "PHI", "PIT", "SEA", "SJS",
        "STL", "TBL", "TOR", "UTA", "VAN", "VGK", "WPG", "WSH"
    ];

    /// <summary>
    /// Team full names by abbreviation.
    /// </summary>
    public static readonly Dictionary<string, string> TeamNames = new()
    {
        ["ANA"] = "Anaheim Ducks",
        ["BOS"] = "Boston Bruins",
        ["BUF"] = "Buffalo Sabres",
        ["CAR"] = "Carolina Hurricanes",
        ["CBJ"] = "Columbus Blue Jackets",
        ["CGY"] = "Calgary Flames",
        ["CHI"] = "Chicago Blackhawks",
        ["COL"] = "Colorado Avalanche",
        ["DAL"] = "Dallas Stars",
        ["DET"] = "Detroit Red Wings",
        ["EDM"] = "Edmonton Oilers",
        ["FLA"] = "Florida Panthers",
        ["LAK"] = "Los Angeles Kings",
        ["MIN"] = "Minnesota Wild",
        ["MTL"] = "Montreal Canadiens",
        ["NJD"] = "New Jersey Devils",
        ["NSH"] = "Nashville Predators",
        ["NYI"] = "New York Islanders",
        ["NYR"] = "New York Rangers",
        ["OTT"] = "Ottawa Senators",
        ["PHI"] = "Philadelphia Flyers",
        ["PIT"] = "Pittsburgh Penguins",
        ["SEA"] = "Seattle Kraken",
        ["SJS"] = "San Jose Sharks",
        ["STL"] = "St. Louis Blues",
        ["TBL"] = "Tampa Bay Lightning",
        ["TOR"] = "Toronto Maple Leafs",
        ["UTA"] = "Utah Hockey Club",
        ["VAN"] = "Vancouver Canucks",
        ["VGK"] = "Vegas Golden Knights",
        ["WPG"] = "Winnipeg Jets",
        ["WSH"] = "Washington Capitals"
    };

    /// <summary>
    /// Gets the team name from abbreviation.
    /// </summary>
    public static string GetTeamName(string? abbrev)
    {
        if (string.IsNullOrEmpty(abbrev))
            return "Unknown";

        return TeamNames.TryGetValue(abbrev.ToUpperInvariant(), out var name) ? name : abbrev;
    }
}
