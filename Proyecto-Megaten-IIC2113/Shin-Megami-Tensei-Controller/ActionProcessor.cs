using System;
using System.Collections.Generic;
using Shin_Megami_Tensei_View.ConsoleLib;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Core;
using Shin_Megami_Tensei_View;

namespace Shin_Megami_Tensei
{
    public class ActionProcessor
    {
        private readonly BattleView battleView;
        private readonly CombatManager combatManager;

        public ActionProcessor(BattleView battleView, CombatManager combatManager)
        {
            this.battleView = battleView;
            this.combatManager = combatManager;
        }

        public bool ProcessActionOrder(BattleContext battleParams, List<UnitInstance> actionOrder, TeamState currentTeam)
        {
            while (ShouldContinueProcessingActions(battleParams))
            {
                if (ProcessSingleActionIteration(battleParams, actionOrder, currentTeam))
                    return true;
            }
            return false;
        }

        private bool ShouldContinueProcessingActions(BattleContext battleParams)
        {
            const int ZERO_TURNS = 0;
            return battleParams.BattleState.FullTurns > ZERO_TURNS && !combatManager.IsBattleOver(battleParams.BattleState);
        }

        private bool ProcessSingleActionIteration(BattleContext battleParams, List<UnitInstance> actionOrder, TeamState currentTeam)
        {
            ShowBattleStatus(battleParams, actionOrder);
            if (IsActionOrderEmpty(actionOrder)) 
                return false;
            
            var currentUnit = GetCurrentUnit(actionOrder);
            if (ProcessSingleUnitAction(currentUnit, battleParams)) 
                return true;
            
            ProcessUnitTurnEnd(actionOrder, currentTeam, currentUnit);
            return false;
        }

        private void ShowBattleStatus(BattleContext battleParams, List<UnitInstance> actionOrder)
        {
            battleView.ShowBattlefield(battleParams.BattleState, battleParams.Player1Name, battleParams.Player2Name);
            battleView.ShowTurnCounters(battleParams.BattleState);
            battleView.ShowActionOrderBySpeed(actionOrder);
        }

        private bool IsActionOrderEmpty(List<UnitInstance> actionOrder)
        {
            const int EMPTY_LIST_COUNT = 0;
            return actionOrder.Count == EMPTY_LIST_COUNT;
        }

        private UnitInstance GetCurrentUnit(List<UnitInstance> actionOrder)
        {
            const int FIRST_UNIT_INDEX = 0;
            return actionOrder[FIRST_UNIT_INDEX];
        }

        private bool ProcessSingleUnitAction(UnitInstance currentUnit, BattleContext battleParams)
        {
            if (IsUnitActionSuccessful(currentUnit, battleParams))
                return true;
            
            combatManager.ConsumeTurn(battleParams.BattleState);
            
            return CheckAndHandleBattleEnd(battleParams);
        }

        private bool IsUnitActionSuccessful(UnitInstance currentUnit, BattleContext battleParams)
        {
            return combatManager.ProcessUnitAction(currentUnit, battleParams.BattleState, battleParams.Player1Name, battleParams.Player2Name);
        }

        private bool CheckAndHandleBattleEnd(BattleContext battleParams)
        {
            if (combatManager.IsBattleOver(battleParams.BattleState))
            {
                AnnounceWinner(battleParams.BattleState, battleParams.Player1Name, battleParams.Player2Name);
                return true;
            }
            return false;
        }

        private void ProcessUnitTurnEnd(List<UnitInstance> actionOrder, TeamState currentTeam, UnitInstance currentUnit)
        {
            const int FIRST_UNIT_INDEX = 0;
            actionOrder.RemoveAt(FIRST_UNIT_INDEX);
            if (currentTeam.AliveUnits.Contains(currentUnit))
            {
                actionOrder.Add(currentUnit);
            }
        }

        private void AnnounceWinner(BattleState battleState, string player1Name, string player2Name)
        {
            var winnerName = combatManager.GetWinner(battleState, player1Name, player2Name);
            var winnerNumber = combatManager.GetWinnerNumber(battleState);
            battleView.ShowWinner(winnerName, winnerNumber);
        }
    }
}
