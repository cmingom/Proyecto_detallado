using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Enums;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class AffinityProcessor
    {
        private const double WeakMultiplier = 1.5;
        private const double ResistMultiplier = 0.5;

        public AffinityReaction DetermineReaction(UnitInstanceContext target, DamageElement element)
        {
            var affinityKey = GetAffinityKey(element);
            if (affinityKey == null)
            {
                return AffinityReaction.Neutral;
            }

            if (target.Affinities.TryGetValue(affinityKey, out var affinityValue))
            {
                return MapAffinityValue(affinityValue);
            }

            return AffinityReaction.Neutral;
        }

        public double ApplyReactionModifier(double baseDamage, AffinityReaction reaction)
        {
            return reaction switch
            {
                AffinityReaction.Resist => baseDamage * ResistMultiplier,
                AffinityReaction.Weak => baseDamage * WeakMultiplier,
                AffinityReaction.Null => 0d,
                AffinityReaction.Miss => 0d,
                _ => baseDamage
            };
        }

        public DamageResolutionResult ProcessDamageWithReaction(
            UnitInstanceContext attacker, 
            UnitInstanceContext target, 
            DamageElement element, 
            double baseDamage, 
            AffinityReaction reaction)
        {
            if (baseDamage <= 0)
            {
                return CreateDamageResult(attacker, target, 0, 0, reaction);
            }

            var modifiedDamage = ApplyReactionModifier(baseDamage, reaction);
            var finalDamage = RoundDownToInteger(modifiedDamage);

            return ExecuteReactionEffect(attacker, target, finalDamage, reaction);
        }

        private DamageResolutionResult ExecuteReactionEffect(
            UnitInstanceContext attacker, 
            UnitInstanceContext target, 
            int damage, 
            AffinityReaction reaction)
        {
            return reaction switch
            {
                AffinityReaction.Null => CreateDamageResult(attacker, target, 0, 0, reaction),
                AffinityReaction.Miss => CreateDamageResult(attacker, target, 0, 0, reaction),
                AffinityReaction.Repel => ProcessRepelEffect(attacker, target, damage, reaction),
                AffinityReaction.Drain => ProcessDrainEffect(attacker, target, damage, reaction),
                _ => ProcessDirectDamage(attacker, target, damage, reaction)
            };
        }

        private DamageResolutionResult ProcessDirectDamage(
            UnitInstanceContext attacker, 
            UnitInstanceContext target, 
            int damage, 
            AffinityReaction reaction)
        {
            ApplyDamageToUnit(target, damage);
            return CreateDamageResult(attacker, target, damage, 0, reaction);
        }

        private DamageResolutionResult ProcessRepelEffect(
            UnitInstanceContext attacker, 
            UnitInstanceContext target, 
            int reflectedDamage, 
            AffinityReaction reaction)
        {
            ApplyDamageToUnit(attacker, reflectedDamage);
            return CreateDamageResult(attacker, target, 0, reflectedDamage, reaction);
        }

        private DamageResolutionResult ProcessDrainEffect(
            UnitInstanceContext attacker, 
            UnitInstanceContext target, 
            int healAmount, 
            AffinityReaction reaction)
        {
            ApplyDamageToUnit(target, -healAmount);
            return CreateDamageResult(attacker, target, healAmount, 0, reaction);
        }

        private DamageResolutionResult CreateDamageResult(
            UnitInstanceContext attacker, 
            UnitInstanceContext target, 
            int damageToTarget, 
            int damageToAttacker, 
            AffinityReaction reaction)
        {
            return new DamageResolutionResult(damageToTarget, damageToAttacker, target.HP, attacker.HP, reaction, false);
        }

        private static string? GetAffinityKey(DamageElement element)
        {
            return element switch
            {
                DamageElement.Phys => "Phys",
                DamageElement.Gun => "Gun",
                DamageElement.Fire => "Fire",
                DamageElement.Ice => "Ice",
                DamageElement.Elec => "Elec",
                DamageElement.Force => "Force",
                DamageElement.Almighty => "Almighty",
                _ => null
            };
        }

        private static AffinityReaction MapAffinityValue(string value)
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

        private static int RoundDownToInteger(double value)
        {
            if (value <= 0)
            {
                return 0;
            }

            return (int)Math.Floor(value);
        }

        private static void ApplyDamageToUnit(UnitInstanceContext unit, int damage)
        {
            if (damage == 0)
            {
                return;
            }

            if (damage > 0)
            {
                unit.HP = Math.Max(0, unit.HP - damage);
            }
            else
            {
                unit.HP = Math.Min(unit.MaxHP, unit.HP - damage);
            }
        }
    }
}