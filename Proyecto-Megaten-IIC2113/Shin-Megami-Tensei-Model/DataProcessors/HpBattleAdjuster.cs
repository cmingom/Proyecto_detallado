using Shin_Megami_Tensei_Model.Data.Components;

namespace Shin_Megami_Tensei_Model.DataProcessors;

public static class HpBattleAdjuster
{
    public static void AdjustHpAfterDamage(Unit unit, int damage)
    {
        unit.AllModifiers.Hp.TakeDamage(damage);
    }
}