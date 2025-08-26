using Shin_Megami_Tensei_Model.Data.Components;

namespace Shin_Megami_Tensei_Model.DataProcessors;

public static class DamageCalculator
{
    private const int BASE_STRENGTH_BONUS = 20;
    private const int DAMAGE_MULTIPLIER = 6;
    private const int DEFENSE_STRENGTH_MULTIPLIER = 2;
    private const int PERCENTAGE_BASE = 100;
    
    public static int CalculatePhysicalDamage(Unit attacker, Unit defender)
    {
        var attackerStr = attacker.AllModifiers.Str.Value;
        var defenderStr = defender.AllModifiers.Str.Value;
        
        double damage = (attackerStr + BASE_STRENGTH_BONUS) * DAMAGE_MULTIPLIER * 
                       ((PERCENTAGE_BASE - defenderStr * DEFENSE_STRENGTH_MULTIPLIER) / (double)PERCENTAGE_BASE);
        
        return (int)Math.Floor(damage);
    }
}