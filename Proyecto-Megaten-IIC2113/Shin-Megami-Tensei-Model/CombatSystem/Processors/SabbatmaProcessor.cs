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

            battleView.ShowSummonMenu(availableUnits);

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

        public bool CanProcessInvitation(UnitInstanceContext summoner, BattleState battleState, Skill skill)
        {
            if (battleState.IsBattleFinished)
            {
                return false;
            }

            var currentTeam = battleState.GetCurrentTeam();
            var availableUnits = GetAvailableUnitsForInvitation(currentTeam);

            battleView.ShowSummonMenu(availableUnits);

            var unitChoice = battleView.GetSummonChoice(availableUnits.Count + CANCEL_CHOICE_OFFSET);

            if (IsInvalidChoice(unitChoice) || IsUnitChoiceCancelled(unitChoice, availableUnits.Count))
            {
                return false;
            }

            var selectedUnit = availableUnits[unitChoice - 1];
            var previousHp = selectedUnit.HP;
            var needsRevive = selectedUnit.HP <= 0;

            try
            {
                var executed = ProcessSabbatmaPlacement(selectedUnit, currentTeam, battleState, skipTurnOutcome: needsRevive);

                if (!executed)
                {
                    if (needsRevive)
                    {
                        selectedUnit.HP = previousHp;
                    }

                    return false;
                }

                if (needsRevive)
                {
                    selectedUnit.HP = selectedUnit.MaxHP;
                    battleView.ShowReviveResult(summoner, selectedUnit, skill.Name, selectedUnit.HP, showSeparator: false);
                    ApplySummonOutcome(battleState);
                }

                return true;
            }
            catch (ActionCancelledException)
            {
                if (needsRevive)
                {
                    selectedUnit.HP = previousHp;
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

        private List<UnitInstanceContext> GetAvailableUnitsForInvitation(TeamState team)
        {
            var units = new List<UnitInstanceContext>();

            foreach (var slot in SAMURAI_POSITIONS)
            {
                var unit = team.GetActiveUnitAt(slot);
                if (unit != null && !unit.IsSamurai && unit.HP <= 0 && !units.Contains(unit))
                {
                    units.Add(unit);
                }
            }

            foreach (var reserve in team.Reserves)
            {
                if (!units.Contains(reserve))
                {
                    units.Add(reserve);
                }
            }

            return units;
        }

        private bool IsInvalidChoice(int choice)
        {
            return choice == INVALID_CHOICE;
        }

        private bool IsUnitChoiceCancelled(int choice, int availableUnitsCount)
        {
            return choice == availableUnitsCount + CANCEL_CHOICE_OFFSET;
        }

        private bool ProcessSabbatmaPlacement(UnitInstanceContext selectedUnit, TeamState currentTeam, BattleState battleState, bool skipTurnOutcome = false)
        {
            var positionOptions = GetSamuraiPositionOptions(currentTeam);
            battleView.ShowSummonPositionMenu(positionOptions);
            var positionChoice = battleView.GetSummonPositionChoice(positionOptions.Count + CANCEL_CHOICE_OFFSET);

            if (IsInvalidChoice(positionChoice) || IsPositionChoiceCancelled(positionChoice, positionOptions.Count))
            {
                throw new ActionCancelledException();
            }

            var (slot, existingUnit) = positionOptions[positionChoice - 1];
            ExecuteSabbatmaPlacement(selectedUnit, existingUnit, slot, currentTeam, battleState, skipTurnOutcome);
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

        private void ExecuteSabbatmaPlacement(UnitInstanceContext selectedUnit, UnitInstanceContext? existingUnit, char slot, TeamState currentTeam, BattleState battleState, bool skipTurnOutcome)
        {
            RemoveUnitFromActivePositions(currentTeam, selectedUnit);
            currentTeam.SetActiveUnitAt(slot, selectedUnit);
            currentTeam.RemoveFromReserves(selectedUnit);

            if (existingUnit != null && !existingUnit.IsSamurai)
            {
                currentTeam.AddToReserves(existingUnit);
            }

            battleView.ShowSummonResult(selectedUnit.Name);

            if (!skipTurnOutcome)
            {
                ApplySummonOutcome(battleState);
            }
        }

        private void RemoveUnitFromActivePositions(TeamState team, UnitInstanceContext unit)
        {
            foreach (var slot in SAMURAI_POSITIONS)
            {
                if (team.GetActiveUnitAt(slot) == unit)
                {
                    team.SetActiveUnitAt(slot, null);
                }
            }
        }

        private void ApplySummonOutcome(BattleState battleState)
        {
            turnOutcomeProcessor.ApplySummonTurnOutcome(battleState);
            battleState.IncrementCurrentPlayerSkillCounter();
        }
    }
}
