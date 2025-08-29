using System.Text.Json.Serialization;

namespace Shin_Megami_Tensei_Model.Domain.Entities;

public sealed class UnitStats
{
    [JsonPropertyName("HP")]
    public int HP { get; init; }
    
    [JsonPropertyName("MP")]
    public int MP { get; init; }
    
    [JsonPropertyName("Str")]
    public int Str { get; init; }
    
    [JsonPropertyName("Skl")]
    public int Skl { get; init; }
    
    [JsonPropertyName("Mag")]
    public int Mag { get; init; }
    
    [JsonPropertyName("Spd")]
    public int Spd { get; init; }
    
    [JsonPropertyName("Lck")]
    public int Lck { get; init; }
}
