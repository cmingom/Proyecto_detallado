using System;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Enums;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class DamageCalculator
    {
        private const double BaseGunDamage = 80.0;
        private const double BasePhysicalDamage = 54.0;
        private const double PerPointScaling = 0.0114;
        private const double WeakMultiplier = 1.5;
        private const double ResistMultiplier = 0.5;

        private static readonly double GunDamageMultiplier = BaseGunDamage * PerPointScaling;
        private static readonly double PhysicalDamageMultiplier = BasePhysicalDamage * PerPointScaling;

        public double GetBasicAttackBaseDamage(UnitInstanceContext attacker, DamageElement element)
        {
            return element switch
            {
                DamageElement.Gun => attacker.Skl * GunDamageMultiplier,
                _ => attacker.Str * PhysicalDamageMultiplier
            };
        }

        public double GetSkillBaseDamage(UnitInstanceContext attacker, Skill skill)
        {
            var element = ParseElement(skill.Type);
            var stat = GetRelevantStat(attacker, element);
            var skillPower = Math.Max(skill.Power, 0);
            return Math.Sqrt(stat * skillPower);
        }

        public DamageResolutionResult ResolveDamage(UnitInstanceContext attacker, UnitInstanceContext target, DamageElement element, double baseDamage)
        {
            var reaction = GetAffinityReaction(target, element);
            if (baseDamage <= 0)
            {
                return BuildResult(attacker, target, 0, 0, reaction);
            }

            var modifiedDamage = ApplyAffinityModifier(baseDamage, reaction);
            modifiedDamage = ApplyAdditionalMultipliers(attacker, target, element, modifiedDamage, reaction);
            var finalDamage = RoundDown(modifiedDamage);

            return reaction switch
            {
                AffinityReaction.Null => BuildResult(attacker, target, 0, 0, reaction),
                AffinityReaction.Miss => BuildResult(attacker, target, 0, 0, reaction),
                AffinityReaction.Repel => ApplyRepel(attacker, target, finalDamage, reaction),
                AffinityReaction.Drain => ApplyDrain(attacker, target, finalDamage, reaction),
                _ => ApplyDirectDamage(attacker, target, finalDamage, reaction)
            };
        }

        public DamageElement ParseElement(string element)
        {
            return element switch
            {
                "Phys" => DamageElement.Phys,
                "Gun" => DamageElement.Gun,
                "Fire" => DamageElement.Fire,
                "Ice" => DamageElement.Ice,
                "Elec" => DamageElement.Elec,
                "Force" => DamageElement.Force,
                "Almighty" => DamageElement.Almighty,
                _ => DamageElement.Phys
            };
        }

        private DamageResolutionResult ApplyDirectDamage(UnitInstanceContext attacker, UnitInstanceContext target, int damage, AffinityReaction reaction)
        {
            ApplyDamage(target, damage);
            return BuildResult(attacker, target, damage, 0, reaction);
        }

        private DamageResolutionResult ApplyRepel(UnitInstanceContext attacker, UnitInstanceContext target, int reflectedDamage, AffinityReaction reaction)
        {
            ApplyDamage(attacker, reflectedDamage);
            return BuildResult(attacker, target, 0, reflectedDamage, reaction);
        }

        private DamageResolutionResult ApplyDrain(UnitInstanceContext attacker, UnitInstanceContext target, int healAmount, AffinityReaction reaction)
        {
            ApplyDamage(target, -healAmount);
            return BuildResult(attacker, target, healAmount, 0, reaction);
        }

        private DamageResolutionResult BuildResult(UnitInstanceContext attacker, UnitInstanceContext target, int damageToTarget, int damageToAttacker, AffinityReaction reaction)
        {
            return new DamageResolutionResult(damageToTarget, damageToAttacker, target.HP, attacker.HP, reaction, false);
        }

        private int GetRelevantStat(UnitInstanceContext attacker, DamageElement element)
        {
            return element switch
            {
                DamageElement.Gun => attacker.Skl,
                DamageElement.Fire => attacker.Mag,
                DamageElement.Ice => attacker.Mag,
                DamageElement.Elec => attacker.Mag,
                DamageElement.Force => attacker.Mag,
                DamageElement.Almighty => attacker.Mag,
                _ => attacker.Str
            };
        }

        private double ApplyAffinityModifier(double baseDamage, AffinityReaction reaction)
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

        private double ApplyAdditionalMultipliers(UnitInstanceContext attacker, UnitInstanceContext target, DamageElement element, double value, AffinityReaction reaction)
        {
            if (reaction == AffinityReaction.Repel || reaction == AffinityReaction.Drain)
            {
                return value;
            }

            value *= GetOffensiveModifier(attacker, element);
            value *= GetDefensiveModifier(target, element);
            value *= GetPleromaModifier(attacker, element);
            value *= GetChargeOrConcentrateMultiplier(attacker, element);
            return value;
        }

        private double GetOffensiveModifier(UnitInstanceContext attacker, DamageElement element)
        {
            return 1.0;
        }

        private double GetDefensiveModifier(UnitInstanceContext target, DamageElement element)
        {
            return 1.0;
        }

        private double GetPleromaModifier(UnitInstanceContext attacker, DamageElement element)
        {
            return 1.0;
        }

        private double GetChargeOrConcentrateMultiplier(UnitInstanceContext attacker, DamageElement element)
        {
            return 1.0;
        }

        private static int RoundDown(double value)
        {
            if (value <= 0)
            {
                return 0;
            }

            return (int)Math.Floor(value);
        }

        private static void ApplyDamage(UnitInstanceContext unit, int damage)
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

        private AffinityReaction GetAffinityReaction(UnitInstanceContext target, DamageElement element)
        {
            var key = GetAffinityKey(element);
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
    }
}
