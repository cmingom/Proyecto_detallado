using System;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Core;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class DamageCalculator
    {
        private const double BASE_GUN_DAMAGE = 80.0;
        private const double BASE_PHYSICAL_DAMAGE = 54.0;
        private const double PER_POINT_SCALING = 0.0114;
        private const double GUN_DAMAGE_MULTIPLIER = BASE_GUN_DAMAGE * PER_POINT_SCALING;
        private const double PHYSICAL_DAMAGE_MULTIPLIER = BASE_PHYSICAL_DAMAGE * PER_POINT_SCALING;

        public int CalculateAttackDamage(AttackContext attackContext)
        {
            return IsGunAttack(attackContext.AttackType)
                ? CalculateGunDamage(attackContext.Attacker.Skl)
                : CalculatePhysicalDamage(attackContext.Attacker.Str);
        }

        private bool IsGunAttack(AttackType attackType)
        {
            return attackType == AttackType.Gun;
        }

        private int CalculateGunDamage(int skill)
        {
            return (int)Math.Floor(skill * GUN_DAMAGE_MULTIPLIER);
        }

        private int CalculatePhysicalDamage(int strength)
        {
            return (int)Math.Floor(strength * PHYSICAL_DAMAGE_MULTIPLIER);
        }

        public void ApplyDamageToTarget(UnitInstance target, int damage)
        {
            var newHP = CalculateNewHP(target.HP, damage);
            target.HP = newHP;
        }

        private int CalculateNewHP(int currentHP, int damage)
        {
            return Math.Max(0, currentHP - damage);
        }
    }
}
