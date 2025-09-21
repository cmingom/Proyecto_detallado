using System.Text.Json.Serialization;

namespace Shin_Megami_Tensei_Model.Domain.Entities;

public sealed class Skill
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [JsonPropertyName("cost")]
    public int Cost { get; init; }

    [JsonPropertyName("power")]
    public int Power { get; init; }

    [JsonPropertyName("target")]
    public string Target { get; init; } = string.Empty;

    [JsonPropertyName("hits")]
    public string Hits { get; init; } = "1";
}
