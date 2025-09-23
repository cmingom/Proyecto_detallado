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
            this.healProcessor = new HealProcessor(battleView, targetSelector, turnOutcomeProcessor);
            this.sabbatmaProcessor = new SabbatmaProcessor(battleView, turnOutcomeProcessor);
            this.basicSkillsProcessor = new BasicSkillsProcessor();
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
            
            // Delegar a procesadores especializados según el tipo de habilidad
            if (IsHealSkill(selectedSkill))
            {
                return healProcessor.CanProcessHeal(unit, battleState, selectedSkill);
            }
            else if (IsSabbatmaSkill(selectedSkill))
            {
                return sabbatmaProcessor.CanProcessSabbatma(unit, battleState);
            }
            else if (IsInvitationSkill(selectedSkill))
            {
                // Para Invitation, necesitamos seleccionar un objetivo primero
                var targets = GetAvailableTargetsForHeal(battleState);
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
                return sabbatmaProcessor.CanProcessInvitation(unit, target, battleState);
            }
            else if (basicSkillsProcessor.IsOffensiveSkillSupported(selectedSkill))
            {
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

            return false;
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

        private bool IsHealSkill(Skill skill)
        {
            return skill.Name == "Dia" || skill.Name == "Diarama" || skill.Name == "Diarahan" ||
                   skill.Name == "Recarm" || skill.Name == "Samarecarm";
        }

        private bool IsSabbatmaSkill(Skill skill)
        {
            return skill.Name == "Sabbatma";
        }

        private bool IsInvitationSkill(Skill skill)
        {
            return skill.Name == "Invitation";
        }

        private List<UnitInstanceContext> GetAvailableTargetsForHeal(BattleState battleState)
        {
            var allyTeam = battleState.IsPlayer1Turn ? battleState.Team1 : battleState.Team2;
            return allyTeam.AliveUnits.ToList();
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

            // Ejecutar exactamente X golpes como especifica la regla
            for (var hitNumber = 1; hitNumber <= intendedHits; hitNumber++)
            {
                // Solo detener si el atacante está muerto ANTES del golpe
                if (unit.HP <= 0)
                {
                    break;
                }

                var resolution = damageCalculator.ResolveDamage(unit, target, element, baseDamage);
                reactions.Add(resolution.Reaction);
                executedHits = hitNumber;

                // Acumular resultado para mostrar después
                var context = BuildAttackResultContext(unit, target, skill.Name, element, resolution, hitNumber, intendedHits);
                hitResults.Add(context);

                // NO interrumpir por Repel - ejecutar exactamente X golpes
                // Solo detener si alguien muere DESPUÉS del golpe
                if (unit.HP <= 0 || target.HP <= 0)
                {
                    break;
                }
            }

            // Ahora mostrar todos los resultados de una vez
            if (executedHits > 0)
            {
                battleView.StartActionBuffer();
                
                // Mostrar todos los golpes
                for (int i = 0; i < hitResults.Count; i++)
                {
                    var hitResult = hitResults[i];
                    // Marcar correctamente cuál es el último golpe ejecutado
                    var finalContext = new AttackResultContext(
                        hitResult.Attacker, hitResult.Target, hitResult.ActionName, hitResult.Element,
                        hitResult.Reaction, hitResult.DamageToTarget, hitResult.DamageToAttacker,
                        hitResult.HitNumber, hitResults.Count, // Usar el número real de golpes ejecutados
                        hitResult.TargetHpAfter, hitResult.AttackerHpAfter, hitResult.IsCritical);
                    
                    battleView.ShowAttackResult(finalContext);
                }

                var netReaction = DetermineNetReaction(reactions);
                turnOutcomeProcessor.ApplyOutcome(battleState, netReaction);
                
                // Incrementar contador del jugador después de completar la acción
                battleState.IncrementCurrentPlayerActionCounter();
                
                // Incrementar contador de habilidades del jugador después de usar cualquier habilidad
                battleState.IncrementCurrentPlayerSkillCounter();
                
                battleView.FlushActionBuffer();
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
