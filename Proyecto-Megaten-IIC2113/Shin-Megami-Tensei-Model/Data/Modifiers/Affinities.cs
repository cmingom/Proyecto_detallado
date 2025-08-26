using Shin_Megami_Tensei_Model.Enum;

namespace Shin_Megami_Tensei_Model.Data.Modifiers;

public class Affinities
{
    private readonly Dictionary<DamageType, AffinityType> _affinities;

    public Affinities(Dictionary<string, string> affinityData)
    {
        _affinities = new Dictionary<DamageType, AffinityType>();
        
        foreach (var kvp in affinityData)
        {
            var damageType = ParseDamageType(kvp.Key);
            var affinityType = ParseAffinityType(kvp.Value);
            _affinities[damageType] = affinityType;
        }
    }

    public AffinityType GetAffinity(DamageType damageType)
    {
        return _affinities.GetValueOrDefault(damageType, AffinityType.Neutral);
    }

    private DamageType ParseDamageType(string type)
    {
        return type switch
        {
            "Phys" => DamageType.Physical,
            "Gun" => DamageType.Gun,
            "Fire" => DamageType.Fire,
            "Ice" => DamageType.Ice,
            "Elec" => DamageType.Electric,
            "Force" => DamageType.Force,
            "Light" => DamageType.Light,
            "Dark" => DamageType.Dark,
            _ => DamageType.Physical
        };
    }

    private AffinityType ParseAffinityType(string affinity)
    {
        return affinity switch
        {
            "-" => AffinityType.Neutral,
            "Wk" => AffinityType.Weak,
            "Rs" => AffinityType.Resist,
            "Nu" => AffinityType.Null,
            "Rp" => AffinityType.Repel,
            "Dr" => AffinityType.Drain,
            _ => AffinityType.Neutral
        };
    }
}
