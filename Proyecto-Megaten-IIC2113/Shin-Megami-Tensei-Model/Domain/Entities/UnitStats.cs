using System.Text.Json.Serialization;

namespace Shin_Megami_Tensei_Model.Domain.Entities;

public sealed class UnitStats
{
    [JsonPropertyName("HP")]
    public int HealthPoints { get; init; }
    
    [JsonPropertyName("MP")]
    public int ManaPoints { get; init; }
    
    [JsonPropertyName("Str")]
    public int Strength { get; init; }
    
    [JsonPropertyName("Skl")]
    public int Skill { get; init; }
    
    [JsonPropertyName("Mag")]
    public int Magic { get; init; }
    
    [JsonPropertyName("Spd")]
    public int Speed { get; init; }
    
    [JsonPropertyName("Lck")]
    public int Luck { get; init; }
}
