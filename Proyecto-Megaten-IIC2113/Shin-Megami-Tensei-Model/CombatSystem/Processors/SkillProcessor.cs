using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;
using Shin_Megami_Tensei_Model.CombatSystem.Enums;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.Domain.States;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class SkillProcessor
    {
        private const int InvalidSkillSelection = -1;
        private const int CancelSkillOffset = 1;

        private static readonly HashSet<AffinityReaction> PenaltyReactions = new()
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
        private readonly HealProcessor healProcessor;
        private readonly SabbatmaProcessor sabbatmaProcessor;
        private readonly BasicSkillsProcessor basicSkillsProcessor;

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
            sabbatmaProcessor = new SabbatmaProcessor(battleView, turnOutcomeProcessor);
            healProcessor = new HealProcessor(battleView, turnOutcomeProcessor);
            basicSkillsProcessor = new BasicSkillsProcessor();
        }

        public bool ProcessSkill(UnitInstanceContext unit, BattleState battleState)
        {
            var selectedSkill = RequestSkillSelection(unit);
            if (selectedSkill == null)
            {
                return false;
            }

            return ExecuteSkillSelection(unit, battleState, selectedSkill);
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

        private Skill? RequestSkillSelection(UnitInstanceContext unit)
        {
            var availableSkills = GetAvailableSkills(unit);
            battleView.ShowSkillSelection(unit, availableSkills);

            var skillChoice = battleView.GetSkillChoice(availableSkills.Count);
            if (IsCancelledSelection(skillChoice, availableSkills.Count))
            {
                return null;
            }

            return availableSkills[skillChoice - 1];
        }

        private bool ExecuteSkillSelection(UnitInstanceContext unit, BattleState battleState, Skill selectedSkill)
        {
            if (IsHealSkill(selectedSkill))
            {
                return healProcessor.ProcessHeal(unit, battleState, selectedSkill);
            }

            if (IsInvitationSkill(selectedSkill))
            {
                return ProcessInvitationSkill(unit, battleState, selectedSkill);
            }

            if (IsSabbatmaSkill(selectedSkill))
            {
                return ProcessSabbatmaSkill(unit, battleState, selectedSkill);
            }

            if (!basicSkillsProcessor.IsOffensiveSkillSupported(selectedSkill))
            {
                return false;
            }

            return ExecuteOffensiveSkill(unit, battleState, selectedSkill);
        }

        private bool ProcessInvitationSkill(UnitInstanceContext unit, BattleState battleState, Skill selectedSkill)
        {
            var executed = sabbatmaProcessor.ProcessInvitation(unit, battleState, selectedSkill);
            if (executed)
            {
                unit.MP -= selectedSkill.Cost;
            }

            return executed;
        }

        private bool ProcessSabbatmaSkill(UnitInstanceContext unit, BattleState battleState, Skill selectedSkill)
        {
            var executed = sabbatmaProcessor.ProcessSabbatma(unit, battleState, selectedSkill);
            if (executed)
            {
                unit.MP -= selectedSkill.Cost;
            }

            return executed;
        }

        private bool ExecuteOffensiveSkill(UnitInstanceContext unit, BattleState battleState, Skill selectedSkill)
        {
            var targets = targetSelector.GetAvailableTargetsForAttack(battleState);
            if (!targets.Any())
            {
                return false;
            }

            var target = targetSelector.RequestTargetForAttack(unit, targets);
            if (target == null)
            {
                return false;
            }

            ExecuteSkill(unit, target, battleState, selectedSkill);
            return true;
        }

        private static bool IsCancelledSelection(int skillChoice, int availableSkillCount)
        {
            return skillChoice == InvalidSkillSelection ||
                   skillChoice == availableSkillCount + CancelSkillOffset;
        }

        private static bool IsHealSkill(Skill skill)
        {
            return skill.Name is "Dia" or "Diarama" or "Diarahan" or "Recarm" or "Samarecarm";
        }

        private static bool IsSabbatmaSkill(Skill skill)
        {
            return skill.Name == "Sabbatma";
        }

        private static bool IsInvitationSkill(Skill skill)
        {
            return skill.Name == "Invitation";
        }

        private void ExecuteSkill(UnitInstanceContext unit, UnitInstanceContext target, BattleState battleState, Skill skill)
        {
            unit.MP -= skill.Cost;

            var element = damageCalculator.ParseElement(skill.Type);
            var baseDamage = damageCalculator.GetSkillBaseDamage(unit, skill);
            var intendedHits = basicSkillsProcessor.CalculateHitCount(skill, battleState);
            var executedHits = 0;
            var reactions = new List<AffinityReaction>();
            var hitResults = new List<AttackResultContext>();

            for (var hitNumber = 1; hitNumber <= intendedHits; hitNumber++)
            {
                if (unit.HP <= 0)
                {
                    break;
                }

                var resolution = damageCalculator.ResolveDamage(unit, target, element, baseDamage);
                reactions.Add(resolution.Reaction);
                executedHits = hitNumber;

                var context = BuildAttackResultContext(unit, target, skill.Name, element, resolution, hitNumber, intendedHits);
                hitResults.Add(context);

                if (unit.HP <= 0)
                {
                    break;
                }
            }

            if (executedHits == 0)
            {
                return;
            }

            battleView.StartActionBuffer();

            foreach (var hitResult in hitResults)
            {
                var context = new AttackResultContext(
                    hitResult.Attacker,
                    hitResult.Target,
                    hitResult.ActionName,
                    hitResult.Element,
                    hitResult.Reaction,
                    hitResult.DamageToTarget,
                    hitResult.DamageToAttacker,
                    hitResult.HitNumber,
                    hitResults.Count,
                    hitResult.TargetHpAfter,
                    hitResult.AttackerHpAfter,
                    hitResult.IsCritical);

                battleView.ShowAttackResult(context);
            }

            var netReaction = DetermineNetReaction(reactions);
            turnOutcomeProcessor.ProcessAffinityOutcome(battleState, netReaction);

            battleState.IncrementCurrentPlayerActionCounter();
            battleState.IncrementCurrentPlayerSkillCounter();

            battleView.FlushActionBuffer();
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

