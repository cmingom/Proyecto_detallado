namespace Shin_Megami_Tensei_Model.CombatSystem.Core;

public class DamageCalculator 
{
    private const double BASE_DAMAGE = 54.0;
    private const double BASE_GUN_DAMAGE = 80.0;
    private const double PER_POINT_SCALING = 0.0114;
    private const double PHYSICAL_DAMAGE_MULTIPLIER = BASE_DAMAGE * PER_POINT_SCALING;
    private const double GUN_DAMAGE_MULTIPLIER = BASE_GUN_DAMAGE * PER_POINT_SCALING;

    public int CalculatePhysicalDamage(int strengthStat) 
        => (int)Math.Floor(strengthStat * PHYSICAL_DAMAGE_MULTIPLIER);

    public int CalculateGunDamage(int skillStat) 
        => (int)Math.Floor(skillStat * GUN_DAMAGE_MULTIPLIER);
}