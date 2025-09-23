using System;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Enums;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class DamageCalculator
    {
        private readonly AffinityProcessor affinityProcessor;
        private readonly BasicSkillsProcessor basicSkillsProcessor;

        public DamageCalculator()
        {
            this.affinityProcessor = new AffinityProcessor();
            this.basicSkillsProcessor = new BasicSkillsProcessor();
        }

        public double GetBasicAttackBaseDamage(UnitInstanceContext attacker, DamageElement element)
        {
            return basicSkillsProcessor.CalculateBasicAttackDamage(attacker, element);
        }

        public double GetSkillBaseDamage(UnitInstanceContext attacker, Skill skill)
        {
            return basicSkillsProcessor.CalculateSkillDamage(attacker, skill);
        }

        public DamageResolutionResult ResolveDamage(UnitInstanceContext attacker, UnitInstanceContext target, DamageElement element, double baseDamage)
        {
            var reaction = affinityProcessor.DetermineReaction(target, element);
            if (baseDamage <= 0)
            {
                return affinityProcessor.ProcessDamageWithReaction(attacker, target, element, 0, reaction);
            }

            var modifiedDamage = ApplyAdditionalMultipliers(attacker, target, element, baseDamage, reaction);
            
            return affinityProcessor.ProcessDamageWithReaction(attacker, target, element, modifiedDamage, reaction);
        }

        public DamageElement ParseElement(string element)
        {
            return basicSkillsProcessor.ParseElement(element);
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

    }
}
