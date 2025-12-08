using System.Text.Json.Serialization;

namespace PuckDrop.Models.Api;

/// <summary>
/// Response model for the team roster endpoint.
/// </summary>
public class RosterResponse
{
    [JsonPropertyName("forwards")]
    public List<RosterPlayer> Forwards { get; set; } = new();

    [JsonPropertyName("defensemen")]
    public List<RosterPlayer> Defensemen { get; set; } = new();

    [JsonPropertyName("goalies")]
    public List<RosterPlayer> Goalies { get; set; } = new();
}

/// <summary>
/// Represents a player on the roster.
/// </summary>
public class RosterPlayer
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("headshot")]
    public string? Headshot { get; set; }

    [JsonPropertyName("firstName")]
    public TeamName FirstName { get; set; } = new();

    [JsonPropertyName("lastName")]
    public TeamName LastName { get; set; } = new();

    [JsonPropertyName("sweaterNumber")]
    public int? SweaterNumber { get; set; }

    [JsonPropertyName("positionCode")]
    public string PositionCode { get; set; } = string.Empty;

    [JsonPropertyName("shootsCatches")]
    public string? ShootsCatches { get; set; }

    [JsonPropertyName("heightInInches")]
    public int? HeightInInches { get; set; }

    [JsonPropertyName("weightInPounds")]
    public int? WeightInPounds { get; set; }

    [JsonPropertyName("heightInCentimeters")]
    public int? HeightInCentimeters { get; set; }

    [JsonPropertyName("weightInKilograms")]
    public int? WeightInKilograms { get; set; }

    [JsonPropertyName("birthDate")]
    public string? BirthDate { get; set; }

    [JsonPropertyName("birthCity")]
    public TeamName? BirthCity { get; set; }

    [JsonPropertyName("birthCountry")]
    public string? BirthCountry { get; set; }

    [JsonPropertyName("birthStateProvince")]
    public TeamName? BirthStateProvince { get; set; }

    /// <summary>
    /// Gets the player's full name.
    /// </summary>
    [JsonIgnore]
    public string FullName => $"{FirstName.Default} {LastName.Default}";

    /// <summary>
    /// Gets the player's height in feet and inches format.
    /// </summary>
    [JsonIgnore]
    public string HeightDisplay
    {
        get
        {
            if (!HeightInInches.HasValue) return "-";
            var feet = HeightInInches.Value / 12;
            var inches = HeightInInches.Value % 12;
            return $"{feet}'{inches}\"";
        }
    }

    /// <summary>
    /// Gets the player's age.
    /// </summary>
    [JsonIgnore]
    public int? Age
    {
        get
        {
            if (string.IsNullOrEmpty(BirthDate)) return null;
            if (!DateTime.TryParse(BirthDate, out var birthDate)) return null;
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
