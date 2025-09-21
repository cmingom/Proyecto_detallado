using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.CombatSystem.Exceptions;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class SummonProcessor
    {
        private const int INVALID_CHOICE = -1;
        private const int CANCEL_CHOICE_OFFSET = 1;
        private static readonly char[] SAMURAI_POSITIONS = { 'B', 'C', 'D' };

        private readonly IBattleView battleView;

        public SummonProcessor(IBattleView battleView)
        {
            this.battleView = battleView;
        }

        public bool CanProcessSummon(UnitInstanceContext summoner, BattleState battleState)
        {
            if (battleState.IsBattleFinished)
            {
                return false;
            }

            var currentTeam = battleState.GetCurrentTeam();
            var availableUnits = GetAvailableUnitsFromReserve(currentTeam);

            battleView.ShowSummonMenu(availableUnits);
            var unitChoice = battleView.GetSummonChoice(availableUnits.Count + CANCEL_CHOICE_OFFSET);

            if (availableUnits.Count == 0 || IsInvalidChoice(unitChoice) || IsUnitChoiceCancelled(unitChoice, availableUnits.Count))
            {
                return false;
            }

            var selectedUnit = availableUnits[unitChoice - 1];

            try
            {
                return summoner.IsSamurai
                    ? ProcessSamuraiSummon(selectedUnit, currentTeam, battleState)
                    : ProcessMonsterSummon(summoner, selectedUnit, currentTeam, battleState);
            }
            catch (ActionCancelledException)
            {
                return false;
            }
        }

        private List<UnitInstanceContext> GetAvailableUnitsFromReserve(TeamState team)
        {
            return team.Reserves.Where(unit => unit.HP > 0).ToList();
        }

        private bool IsUnitChoiceCancelled(int choice, int unitCount)
        {
            return choice == unitCount + CANCEL_CHOICE_OFFSET;
        }

        private bool IsInvalidChoice(int choice)
        {
            return choice == INVALID_CHOICE;
        }

        private bool ProcessSamuraiSummon(UnitInstanceContext unitToSummon, TeamState team, BattleState battleState)
        {
            var positionOptions = GetSamuraiPositionOptions(team);
            battleView.ShowSummonPositionMenu(positionOptions);

            var positionChoice = battleView.GetSummonPositionChoice(positionOptions.Count + CANCEL_CHOICE_OFFSET);

            if (IsPositionChoiceCancelled(positionChoice, positionOptions.Count))
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

            HandleSummonSuccess(unitToSummon.Name, battleState);
            return true;
        }

        private bool ProcessMonsterSummon(UnitInstanceContext summoner, UnitInstanceContext unitToSummon, TeamState team, BattleState battleState)
        {
            var summonerIndex = GetUnitPosition(summoner, team);
            if (summonerIndex == INVALID_CHOICE)
            {
                return false;
            }

            var summonerSlot = GetSlotFromIndex(summonerIndex);

            team.RemoveFromReserves(unitToSummon);
            team.SetActiveUnitAt(summonerSlot, unitToSummon);
            team.AddToReserves(summoner);

            HandleSummonSuccess(unitToSummon.Name, battleState);
            return true;
        }

        private List<(char Slot, UnitInstanceContext? Unit)> GetSamuraiPositionOptions(TeamState team)
        {
            var options = new List<(char, UnitInstanceContext?)>();

            foreach (var slot in SAMURAI_POSITIONS)
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

        private bool IsPositionChoiceCancelled(int choice, int optionCount)
        {
            return choice == INVALID_CHOICE || choice == optionCount + CANCEL_CHOICE_OFFSET;
        }

        private char GetSlotFromIndex(int index)
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

        private int GetUnitPosition(UnitInstanceContext unit, TeamState team)
        {
            for (int i = 0; i < team.Units.Count; i++)
            {
                if (team.Units[i] == unit)
                {
                    return i;
                }
            }

            return INVALID_CHOICE;
        }

        private void HandleSummonSuccess(string unitName, BattleState battleState)
        {
            ShowSummonMessage(unitName);
            ConsumeTurnsForSummon(battleState);
        }

        private void ShowSummonMessage(string unitName)
        {
            battleView.ShowSummonResult(unitName);
        }

        private void ConsumeTurnsForSummon(BattleState battleState)
        {
            int fullTurnsConsumed = 0;
            int blinkingTurnsConsumed = 0;
            int blinkingTurnsGranted = 0;

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
            
            // Usar buffering atómico para invocación
            battleView.StartActionBuffer();
            battleView.ShowTurnConsumptionWithBlinking(fullTurnsConsumed, blinkingTurnsConsumed, blinkingTurnsGranted);
            
            // Incrementar contador del jugador después de completar la acción
            battleState.IncrementCurrentPlayerActionCounter();
            
            battleView.FlushActionBuffer();
        }
    }
}
