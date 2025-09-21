using System;
using System.Collections.Generic;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;
using Shin_Megami_Tensei_Model.CombatSystem.Enums;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.Domain.States;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class SkillProcessor
    {
        private const int InvalidSkillChoice = -1;
        private const int CancelSkillChoiceOffset = 1;

        private static readonly Random MultiHitRandom = Random.Shared;
        private static readonly HashSet<AffinityReaction> PenaltyReactions = new HashSet<AffinityReaction>
        {
            AffinityReaction.Miss,
            AffinityReaction.Null,
            AffinityReaction.Repel,
            AffinityReaction.Drain
        };

        private readonly IBattleView battleView;
        private readonly Dictionary<string, Skill> skillData;
        private readonly TargetSelector targetSelector;
        private readonly DamageCalculator damageCalculator;
        private readonly TurnOutcomeProcessor turnOutcomeProcessor;

        public SkillProcessor(
            IBattleView battleView,
            Dictionary<string, Skill> skillData,
            TargetSelector targetSelector,
            DamageCalculator damageCalculator,
            TurnOutcomeProcessor turnOutcomeProcessor)
        {
            this.battleView = battleView;
            this.skillData = skillData;
            this.targetSelector = targetSelector;
            this.damageCalculator = damageCalculator;
            this.turnOutcomeProcessor = turnOutcomeProcessor;
        }

        public bool CanProcessUseSkill(UnitInstanceContext unit, BattleState battleState)
        {
            var availableSkills = GetAvailableSkills(unit);
            battleView.ShowSkillSelection(unit, availableSkills);
            var skillChoice = battleView.GetSkillChoice(availableSkills.Count);
            if (!IsValidSkillChoice(skillChoice, availableSkills.Count))
            {
                return false;
            }

            var selectedSkill = availableSkills[skillChoice - 1];
            if (!IsSupportedOffensiveSkill(selectedSkill))
            {
                return false;
            }

            var targets = targetSelector.GetAvailableTargetsForAttack(battleState);
            if (targets.Count == 0)
            {
                return false;
            }

            battleView.ShowTargetSelection(unit, targets);
            var targetChoice = battleView.GetTargetChoice(targets.Count);
            if (IsInvalidTargetChoice(targetChoice, targets.Count))
            {
                return false;
            }

            var target = targets[targetChoice - 1];
            ExecuteSkill(unit, target, battleState, selectedSkill);
            return true;
        }

        public List<Skill> GetAvailableSkills(UnitInstanceContext unit)
        {
            var availableSkills = new List<Skill>();
            foreach (var skillName in unit.Skills)
            {
                if (skillData.TryGetValue(skillName, out var skill) && unit.MP >= skill.Cost)
                {
                    availableSkills.Add(skill);
                }
            }

            return availableSkills;
        }

        private bool IsValidSkillChoice(int skillChoice, int skillCount)
        {
            return !IsInvalidSkillChoice(skillChoice, skillCount);
        }

        private static bool IsInvalidSkillChoice(int skillChoice, int skillCount)
        {
            return skillChoice == InvalidSkillChoice || skillChoice == skillCount + CancelSkillChoiceOffset;
        }

        private static bool IsInvalidTargetChoice(int targetChoice, int targetCount)
        {
            return targetChoice == InvalidSkillChoice || targetChoice == targetCount + CancelSkillChoiceOffset;
        }

        private bool IsSupportedOffensiveSkill(Skill skill)
        {
            if (!string.Equals(skill.Target, "Single", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var element = damageCalculator.ParseElement(skill.Type);
            return element == DamageElement.Phys ||
                   element == DamageElement.Gun ||
                   element == DamageElement.Fire ||
                   element == DamageElement.Ice ||
                   element == DamageElement.Elec ||
                   element == DamageElement.Force;
        }

        private int DetermineHitCount(Skill skill)
        {
            if (string.IsNullOrWhiteSpace(skill.Hits))
            {
                return 1;
            }

            if (skill.Hits.Contains('-'))
            {
                var parts = skill.Hits.Split('-', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2 &&
                    int.TryParse(parts[0], out var minHits) &&
                    int.TryParse(parts[1], out var maxHits))
                {
                    if (maxHits < minHits)
                    {
                        (minHits, maxHits) = (maxHits, minHits);
                    }

                    var lowerBound = Math.Max(1, minHits);
                    var upperBound = Math.Max(lowerBound, maxHits);
                    return MultiHitRandom.Next(lowerBound, upperBound + 1);
                }
            }

            if (int.TryParse(skill.Hits, out var fixedHits) && fixedHits > 0)
            {
                return fixedHits;
            }

            return 1;
        }

        private void ExecuteSkill(UnitInstanceContext unit, UnitInstanceContext target, BattleState battleState, Skill skill)
        {
            unit.MP -= skill.Cost;

            var element = damageCalculator.ParseElement(skill.Type);
            var baseDamage = damageCalculator.GetSkillBaseDamage(unit, skill);
            var intendedHits = DetermineHitCount(skill);
            var executedHits = 0;
            var reactions = new List<AffinityReaction>();

            for (var hitNumber = 1; hitNumber <= intendedHits; hitNumber++)
            {
                if (unit.HP <= 0 || target.HP <= 0)
                {
                    break;
                }

                var resolution = damageCalculator.ResolveDamage(unit, target, element, baseDamage);
                reactions.Add(resolution.Reaction);
                executedHits = hitNumber;

                var attackerDefeated = unit.HP <= 0;
                var targetDefeated = target.HP <= 0;
                var shouldStop = attackerDefeated || targetDefeated || resolution.Reaction == AffinityReaction.Repel;
                var totalHitsForContext = shouldStop ? hitNumber : intendedHits;

                var context = BuildAttackResultContext(unit, target, skill.Name, element, resolution, hitNumber, totalHitsForContext);
                battleView.ShowAttackResult(context);

                if (shouldStop)
                {
                    break;
                }
            }

            if (executedHits > 0)
            {
                var netReaction = DetermineNetReaction(reactions);
                turnOutcomeProcessor.ApplyOutcome(battleState, netReaction);
            }
        }

        private AttackResultContext BuildAttackResultContext(
            UnitInstanceContext attacker,
            UnitInstanceContext target,
            string actionName,
            DamageElement element,
            DamageResolutionResult resolution,
            int hitNumber,
            int totalHits)
        {
            return new AttackResultContext(
                attacker,
                target,
                actionName,
                element,
                resolution.Reaction,
                resolution.DamageToTarget,
                resolution.DamageToAttacker,
                hitNumber,
                totalHits,
                resolution.TargetHpAfter,
                resolution.AttackerHpAfter,
                resolution.IsCritical);
        }

        private static AffinityReaction DetermineNetReaction(List<AffinityReaction> reactions)
        {
            if (reactions.Count == 0)
            {
                return AffinityReaction.Neutral;
            }

            foreach (var reaction in reactions)
            {
                if (PenaltyReactions.Contains(reaction))
                {
                    return reaction;
                }
            }

            if (reactions.Contains(AffinityReaction.Weak))
            {
                return AffinityReaction.Weak;
            }

            if (reactions.Contains(AffinityReaction.Resist))
            {
                return AffinityReaction.Resist;
            }

            return AffinityReaction.Neutral;
        }
    }
}
