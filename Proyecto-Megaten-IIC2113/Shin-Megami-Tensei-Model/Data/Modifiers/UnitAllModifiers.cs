using System.Collections;

namespace Shin_Megami_Tensei_Model.Data.Modifiers;

public class UnitAllModifiers(int hp, int mp, int str, int skl, int mag, int spd, int lck, Affinities affinities)
    : IEnumerable<NormalStat>
{
    public readonly NormalStat Str = new("Str", str);
    public readonly NormalStat Skl = new("Skl", skl);
    public readonly NormalStat Mag = new("Mag", mag);
    public readonly NormalStat Spd = new("Spd", spd);
    public readonly NormalStat Lck = new("Lck", lck);
    public readonly StatHp Hp = new(hp);
    public readonly StatMp Mp = new(mp);
    public readonly Affinities Affinities = affinities;

    public IEnumerator<NormalStat> GetEnumerator()
    {
        yield return Str;
        yield return Skl;
        yield return Mag;
        yield return Spd;
        yield return Lck;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void ResetEffects()
    {
        Str.ResetEffects();
        Skl.ResetEffects();
        Mag.ResetEffects();
        Spd.ResetEffects();
        Lck.ResetEffects();
        Hp.ResetEffects();
        Mp.ResetEffects();
    }
}
