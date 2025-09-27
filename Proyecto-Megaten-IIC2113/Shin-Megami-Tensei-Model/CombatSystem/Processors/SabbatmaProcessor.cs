using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.CombatSystem.Exceptions;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class SabbatmaProcessor
    {
        private const int INVALID_CHOICE = -1;
        private const int CANCEL_CHOICE_OFFSET = 1;
        private static readonly char[] SAMURAI_POSITIONS = { 'B', 'C', 'D' };

        private readonly IBattleView battleView;
        private readonly TurnOutcomeProcessor turnOutcomeProcessor;

        public SabbatmaProcessor(IBattleView battleView, TurnOutcomeProcessor turnOutcomeProcessor)
        {
            this.battleView = battleView;
            this.turnOutcomeProcessor = turnOutcomeProcessor;
        }

        public bool CanProcessSabbatma(UnitInstanceContext summoner, BattleState battleState, Skill skill)
        {
            if (battleState.IsBattleFinished)
            {
                return false;
            }

            var currentTeam = battleState.GetCurrentTeam();
            var availableUnits = GetAvailableAliveUnitsFromReserve(currentTeam);

            battleView.ShowSkillUsage(summoner, skill.Name);
            battleView.ShowSummonMenu(availableUnits);

            if (availableUnits.Count == 0)
            {
                return false;
            }

            var unitChoice = battleView.GetSummonChoice(availableUnits.Count + CANCEL_CHOICE_OFFSET);

            if (IsInvalidChoice(unitChoice) || IsUnitChoiceCancelled(unitChoice, availableUnits.Count))
            {
                return false;
            }

            var selectedUnit = availableUnits[unitChoice - 1];

            try
            {
                return ProcessSabbatmaPlacement(selectedUnit, currentTeam, battleState);
            }
            catch (ActionCancelledException)
            {
                return false;
            }
        }

        public bool CanProcessInvitation(UnitInstanceContext summoner, UnitInstanceContext target, BattleState battleState, Skill skill)
        {
            if (battleState.IsBattleFinished)
            {
                return false;
            }

            if (target.HP > 0)
            {
                battleView.StartActionBuffer();
                battleView.ShowHealFailure(summoner, target, skill.Name);
                battleView.FlushActionBuffer();
                return false;
            }

            var currentTeam = battleState.GetCurrentTeam();
            var availableUnits = GetAvailableUnitsFromReserve(currentTeam);

            int previousTargetHp = target.HP;

            battleView.StartActionBuffer();
            target.HP = target.MaxHP;
            battleView.ShowReviveResult(summoner, target, skill.Name, target.HP);
            battleView.FlushActionBuffer();

            battleView.ShowSummonMenu(availableUnits);

            if (availableUnits.Count == 0)
            {
                target.HP = previousTargetHp;
                return false;
            }

            var unitChoice = battleView.GetSummonChoice(availableUnits.Count + CANCEL_CHOICE_OFFSET);

            if (IsInvalidChoice(unitChoice) || IsUnitChoiceCancelled(unitChoice, availableUnits.Count))
            {
                target.HP = previousTargetHp;
                return false;
            }

            var selectedUnit = availableUnits[unitChoice - 1];
            int previousSummonedHp = selectedUnit.HP;
            bool revivedSummonedUnit = false;

            if (selectedUnit.HP <= 0)
            {
                selectedUnit.HP = selectedUnit.MaxHP;
                revivedSummonedUnit = true;
                battleView.ShowReviveResult(summoner, selectedUnit, skill.Name, selectedUnit.HP);
            }

            try
            {
                var executed = ProcessSabbatmaPlacement(selectedUnit, currentTeam, battleState);

                if (!executed)
                {
                    target.HP = previousTargetHp;
                    if (revivedSummonedUnit)
                    {
                        selectedUnit.HP = previousSummonedHp;
                    }
                }

                return executed;
            }
            catch (ActionCancelledException)
            {
                target.HP = previousTargetHp;
                if (revivedSummonedUnit)
                {
                    selectedUnit.HP = previousSummonedHp;
                }

                return false;
            }
        }

        private List<UnitInstanceContext> GetAvailableUnitsFromReserve(TeamState team)
        {
            return team.Reserves.ToList();
        }

        private List<UnitInstanceContext> GetAvailableAliveUnitsFromReserve(TeamState team)
        {
            return team.Reserves.Where(unit => unit.HP > 0).ToList();
        }

        private bool IsInvalidChoice(int choice)
        {
            return choice == INVALID_CHOICE;
        }

        private bool IsUnitChoiceCancelled(int choice, int availableUnitsCount)
        {
            return choice == availableUnitsCount + CANCEL_CHOICE_OFFSET;
        }

        private bool ProcessSabbatmaPlacement(UnitInstanceContext selectedUnit, TeamState currentTeam, BattleState battleState)
        {
            var positionOptions = GetSamuraiPositionOptions(currentTeam);
            battleView.ShowSummonPositionMenu(positionOptions);
            var positionChoice = battleView.GetSummonPositionChoice(positionOptions.Count + CANCEL_CHOICE_OFFSET);

            if (IsInvalidChoice(positionChoice) || IsPositionChoiceCancelled(positionChoice, positionOptions.Count))
            {
                throw new ActionCancelledException();
            }

            var (slot, existingUnit) = positionOptions[positionChoice - 1];
            ExecuteSabbatmaPlacement(selectedUnit, existingUnit, slot, currentTeam, battleState);
            return true;
        }

        private List<(char Slot, UnitInstanceContext? Unit)> GetSamuraiPositionOptions(TeamState currentTeam)
        {
            var options = new List<(char Slot, UnitInstanceContext? Unit)>();
            foreach (var slot in SAMURAI_POSITIONS)
            {
                var unit = currentTeam.GetActiveUnitAt(slot);
                if (unit != null && unit.HP <= 0)
                {
                    unit = null;
                }

                options.Add((slot, unit));
            }
            return options;
        }

        private bool IsPositionChoiceCancelled(int choice, int positionOptionsCount)
        {
            return choice == positionOptionsCount + CANCEL_CHOICE_OFFSET;
        }

        private void ExecuteSabbatmaPlacement(UnitInstanceContext selectedUnit, UnitInstanceContext? existingUnit, char slot, TeamState currentTeam, BattleState battleState)
        {
            currentTeam.SetActiveUnitAt(slot, selectedUnit);
            currentTeam.RemoveFromReserves(selectedUnit);

            if (existingUnit != null && !existingUnit.IsSamurai)
            {
                currentTeam.AddToReserves(existingUnit);
            }

            battleView.ShowSummonResult(selectedUnit.Name);
            turnOutcomeProcessor.ApplySummonTurnOutcome(battleState);
            battleState.IncrementCurrentPlayerSkillCounter();
        }
    }
}
