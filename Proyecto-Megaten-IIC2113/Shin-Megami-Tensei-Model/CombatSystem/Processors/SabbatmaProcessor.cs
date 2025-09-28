using System.Collections.Generic;
using System.Linq;
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
            var selectedUnit = SelectUnit(GetAliveReserves(currentTeam));
            if (selectedUnit == null)
            {
                return false;
            }

            return TryPlaceUnit(selectedUnit, currentTeam, battleState, skipTurnOutcome: false);
        }

        public bool ProcessInvitation(UnitInstanceContext summoner, BattleState battleState, Skill skill)
        {
            if (battleState.IsBattleFinished)
            {
                return false;
            }

            var currentTeam = battleState.GetCurrentTeam();
            var selectedUnit = SelectUnit(GetInvitationCandidates(currentTeam));
            if (selectedUnit == null)
            {
                return false;
            }

            var previousHp = selectedUnit.HP;
            var needsRevive = selectedUnit.HP <= 0;

            if (!TryPlaceUnit(selectedUnit, currentTeam, battleState, needsRevive))
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

        private void RestorePreviousHpIfNeeded(UnitInstanceContext unit, int previousHp, bool needsRevive)
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

        private UnitInstanceContext? SelectUnit(List<UnitInstanceContext> candidates)
        {
            battleView.ShowSummonMenu(candidates);
            var unitChoice = battleView.GetSummonChoice(candidates.Count + CancelChoiceOffset);

            if (candidates.Count == 0 || IsInvalidSelection(unitChoice) || IsCancelledSelection(unitChoice, candidates.Count))
            {
                return null;
            }

            return candidates[unitChoice - 1];
        }

        private bool TryPlaceUnit(UnitInstanceContext selectedUnit, TeamState currentTeam, BattleState battleState, bool skipTurnOutcome)
        {
            var positionOptions = BuildSamuraiPositionOptions(currentTeam);
            battleView.ShowSummonPositionMenu(positionOptions);
            var positionChoice = battleView.GetSummonPositionChoice(positionOptions.Count + CancelChoiceOffset);

            if (IsInvalidSelection(positionChoice) || IsCancelledSelection(positionChoice, positionOptions.Count))
            {
                return false;
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

        private static bool IsInvalidSelection(int choice)
        {
            return choice == InvalidChoice;
        }

        private static bool IsCancelledSelection(int choice, int optionCount)
        {
            return choice == optionCount + CancelChoiceOffset;
        }
    }
}
