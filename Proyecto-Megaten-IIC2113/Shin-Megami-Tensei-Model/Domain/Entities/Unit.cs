using System.Text.Json.Serialization;

namespace Shin_Megami_Tensei_Model.Domain.Entities;

public abstract class Unit
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("stats")]
    public UnitStats Stats { get; init; } = new();
    
    [JsonPropertyName("skills")]
    public List<string> Skills { get; init; } = new();

    protected Unit() { }

    public abstract List<string> GetAvailableActions();
}


