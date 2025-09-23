using System;
using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.Domain.States;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class HealProcessor
    {
        private const int INVALID_CHOICE = -1;
        private const int CANCEL_CHOICE_OFFSET = 1;

        private readonly IBattleView battleView;
        private readonly TargetSelector targetSelector;
        private readonly TurnOutcomeProcessor turnOutcomeProcessor;

        public HealProcessor(IBattleView battleView, TargetSelector targetSelector, TurnOutcomeProcessor turnOutcomeProcessor)
        {
            this.battleView = battleView;
            this.targetSelector = targetSelector;
            this.turnOutcomeProcessor = turnOutcomeProcessor;
        }

        public bool CanProcessHeal(UnitInstanceContext healer, BattleState battleState, Skill skill)
        {
            if (battleState.IsBattleFinished)
            {
                return false;
            }

            // Validar que el healer tenga MP suficiente
            if (healer.MP < skill.Cost)
            {
                return false;
            }

            var availableTargets = GetAvailableTargetsForHeal(battleState, skill);
            if (!availableTargets.Any())
            {
                return false;
            }

            battleView.ShowTargetSelection(healer, availableTargets);
            var targetChoice = battleView.GetTargetChoice(availableTargets.Count);

            if (IsInvalidTargetChoice(targetChoice, availableTargets.Count))
            {
                return false;
            }

            var selectedTarget = availableTargets[targetChoice - 1];
            ExecuteHeal(healer, selectedTarget, battleState, skill);
            return true;
        }

        private List<UnitInstanceContext> GetAvailableTargetsForHeal(BattleState battleState, Skill skill)
        {
            var allyTeam = GetAllyTeam(battleState);
            var targets = new List<UnitInstanceContext>();

            foreach (var unit in allyTeam.AliveUnits)
            {
                if (IsValidHealTarget(unit, skill))
                {
                    targets.Add(unit);
                }
            }

            return targets;
        }

        private bool IsValidHealTarget(UnitInstanceContext target, Skill skill)
        {
            // Para habilidades de curación: solo unidades vivas
            if (skill.Name == "Dia" || skill.Name == "Diarama" || skill.Name == "Diarahan")
            {
                return target.HP > 0;
            }

            // Para habilidades de revivir: solo unidades KO
            if (skill.Name == "Recarm" || skill.Name == "Samarecarm" || skill.Name == "Invitation")
            {
                return target.HP <= 0;
            }

            return false;
        }

        private TeamState GetAllyTeam(BattleState battleState)
        {
            return battleState.IsPlayer1Turn ? battleState.Team1 : battleState.Team2;
        }

        private bool IsInvalidTargetChoice(int targetChoice, int targetCount)
        {
            return targetChoice == INVALID_CHOICE || targetChoice == targetCount + CANCEL_CHOICE_OFFSET;
        }

        private void ExecuteHeal(UnitInstanceContext healer, UnitInstanceContext target, BattleState battleState, Skill skill)
        {
            battleView.StartActionBuffer();

            if (IsHealSkill(skill))
            {
                ExecuteHealSkill(healer, target, skill);
            }
            else if (IsReviveSkill(skill))
            {
                ExecuteReviveSkill(healer, target, skill);
            }

            // Descontar MP solo después de ejecutar exitosamente
            healer.MP -= skill.Cost;

            // Aplicar reglas de turnos para habilidades de curación
            turnOutcomeProcessor.ApplyHealTurnOutcome(battleState);
            
            // Incrementar contador de habilidades del jugador después de usar cualquier habilidad
            battleState.IncrementCurrentPlayerSkillCounter();

            battleView.FlushActionBuffer();
        }

        private bool IsHealSkill(Skill skill)
        {
            return skill.Name == "Dia" || skill.Name == "Diarama" || skill.Name == "Diarahan";
        }

        private bool IsReviveSkill(Skill skill)
        {
            return skill.Name == "Recarm" || skill.Name == "Samarecarm" || skill.Name == "Invitation";
        }

        private void ExecuteHealSkill(UnitInstanceContext healer, UnitInstanceContext target, Skill skill)
        {
            var healAmount = CalculateHealAmount(healer, skill, target);
            var maxPossibleHeal = target.MaxHP - target.HP;
            var actualHeal = Math.Min(healAmount, maxPossibleHeal);
            
            target.HP += actualHeal;

            // Para Diarahan, mostrar el HP final como la cantidad curada
            var displayAmount = skill.Name == "Diarahan" ? target.HP : healAmount;
            battleView.ShowHealResult(healer, target, skill.Name, displayAmount);
        }

        private void ExecuteReviveSkill(UnitInstanceContext healer, UnitInstanceContext target, Skill skill)
        {
            var reviveHp = CalculateReviveHp(target, skill);
            target.HP = reviveHp;

            battleView.ShowReviveResult(healer, target, skill.Name);
        }

        private int CalculateHealAmount(UnitInstanceContext healer, Skill skill, UnitInstanceContext target)
        {
            return skill.Name switch
            {
                "Dia" => (target.MaxHP * 25) / 100, // 25% del HP máximo
                "Diarama" => (target.MaxHP * 50) / 100, // 50% del HP máximo
                "Diarahan" => target.MaxHP, // cura al máximo HP completo
                _ => skill.Power
            };
        }

        private int CalculateReviveHp(UnitInstanceContext target, Skill skill)
        {
            return skill.Name switch
            {
                "Recarm" => target.MaxHP / 2, // 50% HP
                "Samarecarm" => target.MaxHP, // 100% HP
                "Invitation" => target.MaxHP, // 100% HP
                _ => target.MaxHP
            };
        }
    }
}
