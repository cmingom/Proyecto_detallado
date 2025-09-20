using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Enums;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class DamageCalculator
    {
        private const double BASE_GUN_DAMAGE = 80.0;
        private const double BASE_PHYSICAL_DAMAGE = 54.0;
        private const double PER_POINT_SCALING = 0.0114;
        private const double GUN_DAMAGE_MULTIPLIER = BASE_GUN_DAMAGE * PER_POINT_SCALING;
        private const double PHYSICAL_DAMAGE_MULTIPLIER = BASE_PHYSICAL_DAMAGE * PER_POINT_SCALING;

        public AttackDamageResult CalculateDamageOutcome(AttackContext attackContext, UnitInstanceContext target)
        {
            var baseDamage = GetBaseDamage(attackContext);
            var reaction = GetAffinityReaction(target, attackContext.AttackType);
            var finalDamage = GetModifiedDamage(baseDamage, reaction);
            ApplyDamageToTarget(target, finalDamage);
            return new AttackDamageResult(finalDamage, reaction);
        }

        private int GetBaseDamage(AttackContext attackContext)
        {
            return IsGunAttack(attackContext.AttackType)
                ? GetCalculatedGunDamage(attackContext.Attacker.Skl)
                : GetCalculatedPhysicalDamage(attackContext.Attacker.Str);
        }

        private bool IsGunAttack(AttackType attackType)
        {
            return attackType == AttackType.Gun;
        }

        private int GetCalculatedGunDamage(int skill)
        {
            return (int)Math.Floor(skill * GUN_DAMAGE_MULTIPLIER);
        }

        private int GetCalculatedPhysicalDamage(int strength)
        {
            return (int)Math.Floor(strength * PHYSICAL_DAMAGE_MULTIPLIER);
        }

        private AffinityReaction GetAffinityReaction(UnitInstanceContext target, AttackType attackType)
        {
            var key = GetAffinityKey(attackType);
            if (key == null)
            {
                return AffinityReaction.Neutral;
            }

            if (target.Affinities.TryGetValue(key, out var value))
            {
                return MapAffinityValue(value);
            }

            return AffinityReaction.Neutral;
        }

        private string? GetAffinityKey(AttackType attackType)
        {
            return attackType switch
            {
                AttackType.Physical => "Phys",
                AttackType.Gun => "Gun",
                _ => null
            };
        }

        private AffinityReaction MapAffinityValue(string value)
        {
            return value switch
            {
                "Rs" => AffinityReaction.Resist,
                "Wk" => AffinityReaction.Weak,
                "Nu" => AffinityReaction.Null,
                "Rp" => AffinityReaction.Repel,
                "Dr" => AffinityReaction.Drain,
                _ => AffinityReaction.Neutral
            };
        }

        private int GetModifiedDamage(int baseDamage, AffinityReaction reaction)
        {
            return reaction switch
            {
                AffinityReaction.Resist => Math.Max(0, (int)Math.Floor(baseDamage * 0.5)),
                _ => baseDamage
            };
        }

        private void ApplyDamageToTarget(UnitInstanceContext target, int damage)
        {
            var newHP = GetCalculatedNewHP(target.HP, damage);
            target.HP = newHP;
        }

        private int GetCalculatedNewHP(int currentHP, int damage)
        {
            return Math.Max(0, currentHP - damage);
        }
    }
}