using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_Model.CombatSystem.Exceptions;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.Domain.States;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class SabbatmaProcessor
    {
        private const int InvalidChoice = -1;
        private const int CancelChoiceOffset = 1;
        private static readonly char[] SamuraiSlots = { 'B', 'C', 'D' };

        private readonly IBattleView battleView;
        private readonly TurnOutcomeProcessor turnOutcomeProcessor;

        public SabbatmaProcessor(IBattleView battleView, TurnOutcomeProcessor turnOutcomeProcessor)
        {
            this.battleView = battleView;
            this.turnOutcomeProcessor = turnOutcomeProcessor;
        }

        public bool ProcessSabbatma(UnitInstanceContext summoner, BattleState battleState, Skill skill)
        {
            if (battleState.IsBattleFinished)
            {
                return false;
            }

            var currentTeam = battleState.GetCurrentTeam();
            var availableUnits = GetAliveReserves(currentTeam);

            battleView.ShowSummonMenu(availableUnits);
            var unitChoice = battleView.GetSummonChoice(availableUnits.Count + CancelChoiceOffset);

            if (IsInvalidSelection(unitChoice) || IsCancelledSelection(unitChoice, availableUnits.Count))
            {
                return false;
            }

            var selectedUnit = availableUnits[unitChoice - 1];

            try
            {
                return PlaceUnit(selectedUnit, currentTeam, battleState);
            }
            catch (ActionCancelledException)
            {
                return false;
            }
        }

        public bool ProcessInvitation(UnitInstanceContext summoner, BattleState battleState, Skill skill)
        {
            if (battleState.IsBattleFinished)
            {
                return false;
            }

            var currentTeam = battleState.GetCurrentTeam();
            var availableUnits = GetInvitationCandidates(currentTeam);

            battleView.ShowSummonMenu(availableUnits);
            var unitChoice = battleView.GetSummonChoice(availableUnits.Count + CancelChoiceOffset);

            if (IsInvalidSelection(unitChoice) || IsCancelledSelection(unitChoice, availableUnits.Count))
            {
                return false;
            }

            var selectedUnit = availableUnits[unitChoice - 1];
            var previousHp = selectedUnit.HP;
            var needsRevive = selectedUnit.HP <= 0;

            try
            {
                if (!PlaceUnit(selectedUnit, currentTeam, battleState, skipTurnOutcome: needsRevive))
                {
                    RestorePreviousHpIfNeeded(selectedUnit, previousHp, needsRevive);
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
                RestorePreviousHpIfNeeded(selectedUnit, previousHp, needsRevive);
                return false;
            }
        }

        private static void RestorePreviousHpIfNeeded(UnitInstanceContext unit, int previousHp, bool needsRevive)
        {
            if (needsRevive)
            {
                unit.HP = previousHp;
            }
        }

        private List<UnitInstanceContext> GetAliveReserves(TeamState team)
        {
            return team.AllUnits
                .Where(unit => !unit.IsSamurai && unit.HP > 0 && !team.IsUnitActive(unit))
                .ToList();
        }

        private List<UnitInstanceContext> GetInvitationCandidates(TeamState team)
        {
            return team.AllUnits
                .Where(unit => !unit.IsSamurai && (!team.IsUnitActive(unit) || unit.HP <= 0))
                .ToList();
        }

        private static bool IsInvalidSelection(int choice)
        {
            return choice == InvalidChoice;
        }

        private static bool IsCancelledSelection(int choice, int optionCount)
        {
            return choice == optionCount + CancelChoiceOffset;
        }

        private bool PlaceUnit(UnitInstanceContext selectedUnit, TeamState currentTeam, BattleState battleState, bool skipTurnOutcome = false)
        {
            var positionOptions = BuildSamuraiPositionOptions(currentTeam);
            battleView.ShowSummonPositionMenu(positionOptions);
            var positionChoice = battleView.GetSummonPositionChoice(positionOptions.Count + CancelChoiceOffset);

            if (IsInvalidSelection(positionChoice) || IsCancelledSelection(positionChoice, positionOptions.Count))
            {
                throw new ActionCancelledException();
            }

            var (slot, existingUnit) = positionOptions[positionChoice - 1];
            ExecutePlacement(selectedUnit, existingUnit, slot, currentTeam, battleState, skipTurnOutcome);
            return true;
        }

        private List<(char Slot, UnitInstanceContext? Unit)> BuildSamuraiPositionOptions(TeamState currentTeam)
        {
            var options = new List<(char Slot, UnitInstanceContext? Unit)>();
            foreach (var slot in SamuraiSlots)
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

        private void ExecutePlacement(
            UnitInstanceContext selectedUnit,
            UnitInstanceContext? existingUnit,
            char slot,
            TeamState currentTeam,
            BattleState battleState,
            bool skipTurnOutcome)
        {
            RemoveUnitFromActiveSlots(currentTeam, selectedUnit);
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

        private void RemoveUnitFromActiveSlots(TeamState team, UnitInstanceContext unit)
        {
            foreach (var slot in SamuraiSlots)
            {
                if (team.GetActiveUnitAt(slot) == unit)
                {
                    team.SetActiveUnitAt(slot, null);
                }
            }
        }

        private void ApplySummonOutcome(BattleState battleState)
        {
            turnOutcomeProcessor.ProcessSummonOutcome(battleState);
            battleState.IncrementCurrentPlayerSkillCounter();
        }
    }
}
