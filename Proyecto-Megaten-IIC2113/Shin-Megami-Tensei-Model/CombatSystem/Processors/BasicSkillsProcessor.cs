using System;
using System.Collections.Generic;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.CombatSystem.Enums;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class BasicSkillsProcessor
    {
        private const double BaseGunDamage = 80.0;
        private const double BasePhysicalDamage = 54.0;
        private const double PerPointScaling = 0.0114;

        private static readonly double GunDamageMultiplier = BaseGunDamage * PerPointScaling;
        private static readonly double PhysicalDamageMultiplier = BasePhysicalDamage * PerPointScaling;

        public double CalculateBasicAttackDamage(UnitInstanceContext attacker, DamageElement element)
        {
            return element switch
            {
                DamageElement.Gun => attacker.Skl * GunDamageMultiplier,
                _ => attacker.Str * PhysicalDamageMultiplier
            };
        }

        public double CalculateSkillDamage(UnitInstanceContext attacker, Skill skill)
        {
            var element = ParseElement(skill.Type);
            var relevantStat = GetRelevantStat(attacker, element);
            var skillPower = Math.Max(skill.Power, 0);
            return Math.Sqrt(relevantStat * skillPower);
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

        public bool IsOffensiveSkillSupported(Skill skill)
        {
            if (!IsSingleTargetSkill(skill))
            {
                return false;
            }

            var element = ParseElement(skill.Type);
            return IsSupportedElement(element);
        }

        public List<Skill> GetAvailableOffensiveSkills(UnitInstanceContext unit, Dictionary<string, Skill> skillData)
        {
            var availableSkills = new List<Skill>();
            foreach (var skillName in unit.Skills)
            {
                if (skillData.TryGetValue(skillName, out var skill) && 
                    HasEnoughMana(unit, skill) && 
                    IsOffensiveSkillSupported(skill))
                {
                    availableSkills.Add(skill);
                }
            }

            return availableSkills;
        }

        public int CalculateHitCount(Skill skill, BattleState battleState)
        {
            if (string.IsNullOrWhiteSpace(skill.Hits))
            {
                return 1;
            }

            if (IsRangeHitCount(skill.Hits))
            {
                return CalculateRangeHitCount(skill.Hits, battleState);
            }

            if (IsFixedHitCount(skill.Hits))
            {
                return ParseFixedHitCount(skill.Hits);
            }

            return 1;
        }

        private bool IsSingleTargetSkill(Skill skill)
        {
            return string.Equals(skill.Target, "Single", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsSupportedElement(DamageElement element)
        {
            return element == DamageElement.Phys ||
                   element == DamageElement.Gun ||
                   element == DamageElement.Fire ||
                   element == DamageElement.Ice ||
                   element == DamageElement.Elec ||
                   element == DamageElement.Force;
        }

        private bool HasEnoughMana(UnitInstanceContext unit, Skill skill)
        {
            return unit.MP >= skill.Cost;
        }

        private bool IsRangeHitCount(string hits)
        {
            return hits.Contains('-');
        }

        private bool IsFixedHitCount(string hits)
        {
            return int.TryParse(hits, out var fixedHits) && fixedHits > 0;
        }

        private int CalculateRangeHitCount(string hits, BattleState battleState)
        {
            var parts = hits.Split('-', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2 ||
                !int.TryParse(parts[0], out var minHits) ||
                !int.TryParse(parts[1], out var maxHits))
            {
                return 1;
            }

            if (maxHits < minHits)
            {
                (minHits, maxHits) = (maxHits, minHits);
            }

            var skillCounter = battleState.GetCurrentPlayerSkillCounter();
            var range = maxHits - minHits + 1;
            var offset = skillCounter % range;
            var calculatedHits = minHits + offset;
            return Math.Max(1, calculatedHits);
        }

        private int ParseFixedHitCount(string hits)
        {
            return int.TryParse(hits, out var fixedHits) ? fixedHits : 1;
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
    }
}