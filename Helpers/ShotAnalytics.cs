using PuckDrop.Models.Api;

namespace PuckDrop.Helpers;

/// <summary>
/// Provides shot analytics calculations including danger zones and expected goals.
/// </summary>
public static class ShotAnalytics
{
    /// <summary>
    /// Shot danger zone classifications.
    /// </summary>
    public enum DangerZone
    {
        /// <summary>High danger - close to the net (0-15 feet).</summary>
        High,
        /// <summary>Medium danger - slot area (15-30 feet).</summary>
        Medium,
        /// <summary>Low danger - perimeter shots (30+ feet).</summary>
        Low
    }

    /// <summary>
    /// Goal coordinates for xG calculations. NHL rink is 200x85 feet.
    /// Goals are at x = -89 (left) and x = 89 (right), y = 0 (center).
    /// </summary>
    private const double LeftGoalX = -89.0;
    private const double RightGoalX = 89.0;
    private const double GoalY = 0.0;

    /// <summary>
    /// Distance thresholds in feet for danger zones.
    /// </summary>
    private const double HighDangerDistance = 15.0;
    private const double MediumDangerDistance = 30.0;

    /// <summary>
    /// Base xG values by danger zone.
    /// </summary>
    private const double HighDangerBaseXg = 0.25;
    private const double MediumDangerBaseXg = 0.08;
    private const double LowDangerBaseXg = 0.02;

    /// <summary>
    /// Shot type multipliers for xG calculation.
    /// </summary>
    private static readonly Dictionary<string, double> ShotTypeMultipliers = new(StringComparer.OrdinalIgnoreCase)
    {
        { "wrist", 1.0 },
        { "snap", 1.1 },
        { "slap", 0.9 },
        { "backhand", 0.85 },
        { "deflected", 1.3 },
        { "tip-in", 1.4 },
        { "wrap-around", 0.7 },
        { "bat", 1.2 },
        { "poke", 0.6 },
        { "cradle", 0.8 }
    };

    /// <summary>
    /// Calculates the distance from shot coordinates to the nearest goal.
    /// </summary>
    /// <param name="xCoord">X coordinate of the shot.</param>
    /// <param name="yCoord">Y coordinate of the shot.</param>
    /// <returns>Distance in feet to the nearest goal.</returns>
    public static double CalculateDistanceToGoal(double xCoord, double yCoord)
    {
        // Determine which goal the shot is aimed at (based on x coordinate)
        var goalX = xCoord < 0 ? LeftGoalX : RightGoalX;

        var dx = Math.Abs(xCoord) - Math.Abs(goalX);
        var dy = yCoord - GoalY;

        return Math.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    /// Calculates the distance from shot coordinates to the nearest goal.
    /// </summary>
    /// <param name="play">The play containing shot coordinates.</param>
    /// <returns>Distance in feet to the nearest goal, or null if coordinates unavailable.</returns>
    public static double? CalculateDistanceToGoal(Play play)
    {
        if (play.Details?.XCoord == null || play.Details?.YCoord == null)
            return null;

        return CalculateDistanceToGoal(play.Details.XCoord.Value, play.Details.YCoord.Value);
    }

    /// <summary>
    /// Determines the danger zone for a shot based on distance to goal.
    /// </summary>
    /// <param name="distanceToGoal">Distance in feet from the shot to the goal.</param>
    /// <returns>The danger zone classification.</returns>
    public static DangerZone GetDangerZone(double distanceToGoal)
    {
        if (distanceToGoal <= HighDangerDistance)
            return DangerZone.High;
        if (distanceToGoal <= MediumDangerDistance)
            return DangerZone.Medium;
        return DangerZone.Low;
    }

    /// <summary>
    /// Determines the danger zone for a play.
    /// </summary>
    /// <param name="play">The play containing shot coordinates.</param>
    /// <returns>The danger zone classification, or Low if coordinates unavailable.</returns>
    public static DangerZone GetDangerZone(Play play)
    {
        var distance = CalculateDistanceToGoal(play);
        return distance.HasValue ? GetDangerZone(distance.Value) : DangerZone.Low;
    }

    /// <summary>
    /// Calculates the expected goals (xG) value for a shot.
    /// </summary>
    /// <param name="distanceToGoal">Distance in feet from the shot to the goal.</param>
    /// <param name="shotType">The type of shot (wrist, slap, etc.).</param>
    /// <param name="angle">Optional angle to goal in degrees (0 = straight on).</param>
    /// <returns>Expected goals value (0.0 to 1.0).</returns>
    public static double CalculateXg(double distanceToGoal, string? shotType = null, double? angle = null)
    {
        // Base xG from danger zone
        var dangerZone = GetDangerZone(distanceToGoal);
        var baseXg = dangerZone switch
        {
            DangerZone.High => HighDangerBaseXg,
            DangerZone.Medium => MediumDangerBaseXg,
            _ => LowDangerBaseXg
        };

        // Apply distance decay within zone (closer = higher xG)
        var distanceMultiplier = dangerZone switch
        {
            DangerZone.High => 1.0 + (HighDangerDistance - distanceToGoal) / HighDangerDistance * 0.5,
            DangerZone.Medium => 1.0 - (distanceToGoal - HighDangerDistance) / (MediumDangerDistance - HighDangerDistance) * 0.3,
            _ => Math.Max(0.5, 1.0 - (distanceToGoal - MediumDangerDistance) / 50.0 * 0.5)
        };

        var xg = baseXg * distanceMultiplier;

        // Apply shot type multiplier
        if (!string.IsNullOrEmpty(shotType) && ShotTypeMultipliers.TryGetValue(shotType, out var shotMultiplier))
        {
            xg *= shotMultiplier;
        }

        // Apply angle penalty (shots from extreme angles are harder to score)
        if (angle.HasValue)
        {
            var angleMultiplier = Math.Cos(Math.Abs(angle.Value) * Math.PI / 180.0);
            angleMultiplier = Math.Max(0.3, angleMultiplier); // Don't penalize too harshly
            xg *= angleMultiplier;
        }

        // Cap xG at reasonable maximum
        return Math.Min(0.95, Math.Max(0.01, xg));
    }

    /// <summary>
    /// Calculates the expected goals (xG) value for a play.
    /// </summary>
    /// <param name="play">The play containing shot information.</param>
    /// <returns>Expected goals value, or null if coordinates unavailable.</returns>
    public static double? CalculateXg(Play play)
    {
        var distance = CalculateDistanceToGoal(play);
        if (!distance.HasValue)
            return null;

        var angle = CalculateAngleToGoal(play);
        return CalculateXg(distance.Value, play.Details?.ShotType, angle);
    }

    /// <summary>
    /// Calculates the angle to goal from shot coordinates.
    /// </summary>
    /// <param name="play">The play containing shot coordinates.</param>
    /// <returns>Angle in degrees (0 = straight on, 90 = from the side), or null if unavailable.</returns>
    public static double? CalculateAngleToGoal(Play play)
    {
        if (play.Details?.XCoord == null || play.Details?.YCoord == null)
            return null;

        var x = play.Details.XCoord.Value;
        var y = play.Details.YCoord.Value;

        // Determine which goal
        var goalX = x < 0 ? LeftGoalX : RightGoalX;

        var dx = Math.Abs(goalX) - Math.Abs(x);
        var dy = Math.Abs(y);

        if (dx <= 0)
            return 90.0; // Behind the goal line

        return Math.Atan2(dy, dx) * 180.0 / Math.PI;
    }

    /// <summary>
    /// Gets the color for a danger zone (for UI display).
    /// </summary>
    /// <param name="zone">The danger zone.</param>
    /// <returns>CSS color string.</returns>
    public static string GetDangerZoneColor(DangerZone zone) => zone switch
    {
        DangerZone.High => "#ef4444",   // Red
        DangerZone.Medium => "#f59e0b", // Orange/Amber
        DangerZone.Low => "#3b82f6",    // Blue
        _ => "#6b7280"                   // Gray
    };

    /// <summary>
    /// Gets the display name for a danger zone.
    /// </summary>
    /// <param name="zone">The danger zone.</param>
    /// <returns>Human-readable zone name.</returns>
    public static string GetDangerZoneName(DangerZone zone) => zone switch
    {
        DangerZone.High => "High Danger",
        DangerZone.Medium => "Medium Danger",
        DangerZone.Low => "Low Danger",
        _ => "Unknown"
    };

    /// <summary>
    /// Calculates aggregate shot analytics for a collection of plays.
    /// </summary>
    /// <param name="plays">The plays to analyze.</param>
    /// <param name="teamId">Optional team ID to filter by.</param>
    /// <returns>Aggregate analytics for the shots.</returns>
    public static ShotAnalyticsResult CalculateAggregateAnalytics(IEnumerable<Play> plays, int? teamId = null)
    {
        var shotPlays = plays
            .Where(p => p.TypeDescKey is "shot-on-goal" or "goal" or "missed-shot" or "blocked-shot")
            .Where(p => !teamId.HasValue || p.Details?.EventOwnerTeamId == teamId)
            .ToList();

        var result = new ShotAnalyticsResult
        {
            TotalShots = shotPlays.Count,
            Goals = shotPlays.Count(p => p.TypeDescKey == "goal"),
            ShotsOnGoal = shotPlays.Count(p => p.TypeDescKey is "shot-on-goal" or "goal"),
            MissedShots = shotPlays.Count(p => p.TypeDescKey == "missed-shot"),
            BlockedShots = shotPlays.Count(p => p.TypeDescKey == "blocked-shot")
        };

        foreach (var play in shotPlays)
        {
            var zone = GetDangerZone(play);
            var xg = CalculateXg(play);

            switch (zone)
            {
                case DangerZone.High:
                    result.HighDangerChances++;
                    if (play.TypeDescKey == "goal") result.HighDangerGoals++;
                    break;
                case DangerZone.Medium:
                    result.MediumDangerChances++;
                    if (play.TypeDescKey == "goal") result.MediumDangerGoals++;
                    break;
                case DangerZone.Low:
                    result.LowDangerChances++;
                    if (play.TypeDescKey == "goal") result.LowDangerGoals++;
                    break;
            }

            if (xg.HasValue)
            {
                result.TotalXg += xg.Value;
            }
        }

        return result;
    }
}

/// <summary>
/// Aggregated shot analytics result.
/// </summary>
public class ShotAnalyticsResult
{
    /// <summary>Total shot attempts (all types).</summary>
    public int TotalShots { get; set; }

    /// <summary>Goals scored.</summary>
    public int Goals { get; set; }

    /// <summary>Shots on goal (including goals).</summary>
    public int ShotsOnGoal { get; set; }

    /// <summary>Missed shots.</summary>
    public int MissedShots { get; set; }

    /// <summary>Blocked shots.</summary>
    public int BlockedShots { get; set; }

    /// <summary>High danger scoring chances.</summary>
    public int HighDangerChances { get; set; }

    /// <summary>Goals from high danger area.</summary>
    public int HighDangerGoals { get; set; }

    /// <summary>Medium danger scoring chances.</summary>
    public int MediumDangerChances { get; set; }

    /// <summary>Goals from medium danger area.</summary>
    public int MediumDangerGoals { get; set; }

    /// <summary>Low danger scoring chances.</summary>
    public int LowDangerChances { get; set; }

    /// <summary>Goals from low danger area.</summary>
    public int LowDangerGoals { get; set; }

    /// <summary>Total expected goals.</summary>
    public double TotalXg { get; set; }

    /// <summary>Shooting percentage (goals / shots on goal).</summary>
    public double ShootingPercentage => ShotsOnGoal > 0 ? (double)Goals / ShotsOnGoal * 100.0 : 0.0;

    /// <summary>Goals above expected (actual goals - xG).</summary>
    public double GoalsAboveExpected => Goals - TotalXg;

    /// <summary>Corsi (all shot attempts).</summary>
    public int Corsi => TotalShots;

    /// <summary>Fenwick (unblocked shot attempts).</summary>
    public int Fenwick => ShotsOnGoal + MissedShots;
}
