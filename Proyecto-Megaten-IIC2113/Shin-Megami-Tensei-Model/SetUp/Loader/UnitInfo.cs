using Shin_Megami_Tensei_Model.Data.Modifiers;
using Shin_Megami_Tensei_Model.Data.Skills;

namespace Shin_Megami_Tensei_Model.SetUp.Loader;

public class UnitInfo
{
    public string Name { get; set; }
    public bool IsSamurai { get; set; }
    public Stats Stats { get; set; }
    public Dictionary<string, string> Affinity { get; set; }
    public List<string> Skills { get; set; }

    public UnitAllModifiers GetAllModifiers()
    {
        var affinities = new Affinities(Affinity);
        return new UnitAllModifiers(
            Stats.HP,
            Stats.MP,
            Stats.Str,
            Stats.Skl,
            Stats.Mag,
            Stats.Spd,
            Stats.Lck,
            affinities
        );
    }
}

public class Stats
{
    public int HP { get; set; }
    public int MP { get; set; }
    public int Str { get; set; }
    public int Skl { get; set; }
    public int Mag { get; set; }
    public int Spd { get; set; }
    public int Lck { get; set; }
}
