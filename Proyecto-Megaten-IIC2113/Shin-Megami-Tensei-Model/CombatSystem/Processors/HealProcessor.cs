using System;
using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.Domain.States;
namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class HealProcessor
    {
        private const int InvalidChoice = -1;
        private const int CancelChoiceOffset = 1;
        private readonly IBattleView battleView;
        private readonly TurnOutcomeProcessor turnOutcomeProcessor;
        public HealProcessor(IBattleView battleView, TurnOutcomeProcessor turnOutcomeProcessor)
        {
            this.battleView = battleView;
            this.turnOutcomeProcessor = turnOutcomeProcessor;
        }
        public bool CanProcessHeal(UnitInstanceContext healer, BattleState battleState, Skill skill)
        {
            if (battleState.IsBattleFinished)
            {
                return false;
            }
            var availableTargets = GetAvailableTargetsForSkill(battleState, skill);
            battleView.ShowTargetSelection(healer, availableTargets);
            var targetChoice = battleView.GetTargetChoice(availableTargets.Count);
            if (IsInvalidTargetChoice(targetChoice, availableTargets.Count))
            {
                return false;
            }
            var selectedTarget = availableTargets[targetChoice - 1];
            var executed = ExecuteHeal(healer, selectedTarget, battleState, skill);
            if (!executed)
            {
                return false;
            }
            healer.MP -= skill.Cost;
            turnOutcomeProcessor.ApplyHealTurnOutcome(battleState);
            battleState.IncrementCurrentPlayerSkillCounter();
            return true;
        }
        private List<UnitInstanceContext> GetAvailableTargetsForSkill(BattleState battleState, Skill skill)
        {
            var allyTeam = battleState.IsPlayer1Turn ? battleState.Team1 : battleState.Team2;

            if (IsReviveSkill(skill))
            {
                return allyTeam.AllUnits
                    .Where(unit => unit.HP <= 0)
                    .ToList();
            }

            return allyTeam.ActiveUnits
                .Where(unit => unit != null && unit.HP > 0)
                .Cast<UnitInstanceContext>()
                .ToList();
        }
        private bool IsInvalidTargetChoice(int targetChoice, int targetCount)
        {
            return targetChoice == InvalidChoice || targetChoice == targetCount + CancelChoiceOffset;
        }
        private bool ExecuteHeal(UnitInstanceContext healer, UnitInstanceContext target, BattleState battleState, Skill skill)
        {
            battleView.StartActionBuffer();
            bool executed = IsReviveSkill(skill)
                ? ExecuteReviveSkill(healer, target, battleState, skill)
                : ExecuteStandardHeal(healer, target, skill);
            battleView.FlushActionBuffer();
            return executed;
        }
        private bool ExecuteStandardHeal(UnitInstanceContext healer, UnitInstanceContext target, Skill skill)
        {
            if (target.HP <= 0)
            {
                battleView.ShowHealFailure(healer, target, skill.Name);
                return true;
            }
            var healAmount = CalculateHealAmount(target, skill);
            var missingHp = Math.Max(0, target.MaxHP - target.HP);
            var appliedHeal = Math.Min(healAmount, missingHp);
            target.HP += appliedHeal;
            battleView.ShowHealSuccess(healer, target, skill.Name, healAmount);
            return true;
        }
        private bool ExecuteReviveSkill(UnitInstanceContext healer, UnitInstanceContext target, BattleState battleState, Skill skill)
        {
            if (target.HP > 0)
            {
                battleView.ShowHealFailure(healer, target, skill.Name);
                return false;
            }
            var revivedHp = CalculateReviveHp(target, skill);
            var revivedAmount = Math.Min(revivedHp, target.MaxHP);
            target.HP = revivedAmount;
            var currentTeam = battleState.IsPlayer1Turn ? battleState.Team1 : battleState.Team2;
            if (!target.IsSamurai)
            {
                MoveUnitToReservesIfPossible(currentTeam, target);
            }
            battleView.ShowReviveResult(healer, target, skill.Name, revivedAmount);
            return true;
        }
        private bool IsReviveSkill(Skill skill)
        {
            return skill.Name == "Recarm" || skill.Name == "Samarecarm";
        }
        private int CalculateHealAmount(UnitInstanceContext target, Skill skill)
        {
            if (skill.Power <= 0)
            {
                return target.MaxHP;
            }
            var amount = (target.MaxHP * skill.Power) / 100;
            return Math.Max(1, amount);
        }
        private int CalculateReviveHp(UnitInstanceContext target, Skill skill)
        {
            if (skill.Power <= 0)
            {
                return target.MaxHP;
            }
            var amount = (target.MaxHP * skill.Power) / 100;
            return Math.Max(1, amount);
        }
        private void MoveUnitToReservesIfPossible(TeamState team, UnitInstanceContext unit)
        {
            var positions = new[] { 'A', 'B', 'C', 'D' };
            var removedFromActive = false;
            foreach (var slot in positions)
            {
                var current = team.GetActiveUnitAt(slot);
                if (current == unit)
                {
                    team.SetActiveUnitAt(slot, null);
                    removedFromActive = true;
                    break;
                }
            }
            if (!removedFromActive)
            {
                return;
            }
            if (team.Reserves.Contains(unit))
            {
                return;
            }
            if (team.CanAddToReserves())
            {
                team.AddToReserves(unit);
            }
        }
    }
}
