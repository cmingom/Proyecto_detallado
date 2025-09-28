using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_Model.CombatSystem.Exceptions;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.Domain.States;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class SummonProcessor
    {
        private const int InvalidSelectionIndex = -1;
        private const int CancelOptionOffset = 1;
        private static readonly char[] SamuraiSlots = { 'B', 'C', 'D' };

        private readonly IBattleView battleView;

        public SummonProcessor(IBattleView battleView)
        {
            this.battleView = battleView;
        }

        public bool ProcessSummon(UnitInstanceContext summoner, BattleState battleState)
        {
            if (battleState.IsBattleFinished)
            {
                return false;
            }

            var currentTeam = battleState.GetCurrentTeam();
            var availableUnits = GetSummonCandidates(currentTeam);

            battleView.ShowSummonMenu(availableUnits);
            var unitChoice = battleView.GetSummonChoice(availableUnits.Count + CancelOptionOffset);

            if (availableUnits.Count == 0 || IsInvalidSelection(unitChoice) || IsCancelledSelection(unitChoice, availableUnits.Count))
            {
                return false;
            }

            var selectedUnit = availableUnits[unitChoice - 1];

            try
            {
                return summoner.IsSamurai
                    ? SummonForSamurai(selectedUnit, currentTeam, battleState)
                    : SummonForMonster(summoner, selectedUnit, currentTeam, battleState);
            }
            catch (ActionCancelledException)
            {
                return false;
            }
        }

        private List<UnitInstanceContext> GetSummonCandidates(TeamState team)
        {
            return team.Reserves.Where(unit => unit.HP > 0).ToList();
        }

        private static bool IsCancelledSelection(int choice, int optionCount)
        {
            return choice == optionCount + CancelOptionOffset;
        }

        private static bool IsInvalidSelection(int choice)
        {
            return choice == InvalidSelectionIndex;
        }

        private bool SummonForSamurai(UnitInstanceContext unitToSummon, TeamState team, BattleState battleState)
        {
            var positionOptions = BuildSamuraiPositionOptions(team);
            battleView.ShowSummonPositionMenu(positionOptions);

            var positionChoice = battleView.GetSummonPositionChoice(positionOptions.Count + CancelOptionOffset);

            if (IsInvalidSelection(positionChoice) || IsCancelledSelection(positionChoice, positionOptions.Count))
            {
                throw new ActionCancelledException();
            }

            var selectedOption = positionOptions[positionChoice - 1];

            team.RemoveFromReserves(unitToSummon);
            team.SetActiveUnitAt(selectedOption.Slot, unitToSummon);

            if (selectedOption.Unit != null && !selectedOption.Unit.IsSamurai)
            {
                team.AddToReserves(selectedOption.Unit);
            }

            FinalizeSummon(unitToSummon.Name, battleState);
            return true;
        }

        private bool SummonForMonster(UnitInstanceContext summoner, UnitInstanceContext unitToSummon, TeamState team, BattleState battleState)
        {
            var summonerIndex = GetUnitPosition(summoner, team);
            if (summonerIndex == InvalidSelectionIndex)
            {
                return false;
            }

            var summonerSlot = TranslateIndexToSlot(summonerIndex);

            team.RemoveFromReserves(unitToSummon);
            team.SetActiveUnitAt(summonerSlot, unitToSummon);
            team.AddToReserves(summoner);

            FinalizeSummon(unitToSummon.Name, battleState);
            return true;
        }

        private List<(char Slot, UnitInstanceContext? Unit)> BuildSamuraiPositionOptions(TeamState team)
        {
            var options = new List<(char Slot, UnitInstanceContext? Unit)>();

            foreach (var slot in SamuraiSlots)
            {
                var unit = team.GetActiveUnitAt(slot);
                if (unit != null && unit.HP <= 0)
                {
                    unit = null;
                }

                options.Add((slot, unit));
            }

            return options;
        }

        private static char TranslateIndexToSlot(int index)
        {
            return index switch
            {
                0 => 'A',
                1 => 'B',
                2 => 'C',
                3 => 'D',
                _ => 'R'
            };
        }

        private static int GetUnitPosition(UnitInstanceContext unit, TeamState team)
        {
            for (var i = 0; i < team.Units.Count; i++)
            {
                if (team.Units[i] == unit)
                {
                    return i;
                }
            }

            return InvalidSelectionIndex;
        }

        private void FinalizeSummon(string unitName, BattleState battleState)
        {
            battleView.ShowSummonResult(unitName);
            ConsumeTurnsForSummon(battleState);
        }

        private void ConsumeTurnsForSummon(BattleState battleState)
        {
            var fullTurnsConsumed = 0;
            var blinkingTurnsConsumed = 0;
            var blinkingTurnsGranted = 0;

            if (battleState.BlinkingTurns > 0)
            {
                battleState.ConsumeBlinkingTurn();
                blinkingTurnsConsumed = 1;
            }
            else
            {
                battleState.ConsumeTurn();
                battleState.GrantBlinkingTurn();
                fullTurnsConsumed = 1;
                blinkingTurnsGranted = 1;
            }

            battleState.MarkTurnConsumptionMessageShown();

            battleView.StartActionBuffer();
            battleView.ShowTurnConsumptionWithBlinking(fullTurnsConsumed, blinkingTurnsConsumed, blinkingTurnsGranted);
            battleState.IncrementCurrentPlayerActionCounter();
            battleView.FlushActionBuffer();
        }
    }
}
