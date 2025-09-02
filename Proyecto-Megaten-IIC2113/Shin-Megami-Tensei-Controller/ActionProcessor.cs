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

        public bool ProcessActionOrder(BattleContext battleContext, List<UnitInstance> actionOrder, TeamState currentTeam)
        {
            while (ShouldContinueProcessingActions(battleContext))
            {
                if (ProcessSingleActionIteration(battleContext, actionOrder, currentTeam))
                    return true;
            }
            return false;
        }

        private void ShowBattleStatus(BattleContext battleContext, List<UnitInstance> actionOrder)
        {
            battleView.ShowBattlefield(battleContext.BattleState, battleContext.Player1Name, battleContext.Player2Name);
            battleView.ShowTurnCounters(battleContext.BattleState);
            battleView.ShowActionOrderBySpeed(actionOrder);
        }

        private const int NO_TURNS_REMAINING = 0;
        private const int EMPTY_LIST_COUNT = 0;
        private const int FIRST_UNIT_INDEX = 0;

        private bool ShouldContinueProcessingActions(BattleContext battleContext)
        {
            return battleContext.BattleState.FullTurns > NO_TURNS_REMAINING && !combatManager.IsBattleOver(battleContext.BattleState);
        }

        private bool ProcessSingleActionIteration(BattleContext battleContext, List<UnitInstance> actionOrder, TeamState currentTeam)
        {
            ShowBattleStatus(battleContext, actionOrder);
            if (IsActionOrderEmpty(actionOrder)) 
                return false;
            
            var currentUnit = GetCurrentUnit(actionOrder);
            if (ProcessSingleUnitAction(currentUnit, battleContext)) 
                return true;
            
            ProcessUnitTurnEnd(actionOrder, currentTeam, currentUnit);
            return false;
        }

        private bool IsActionOrderEmpty(List<UnitInstance> actionOrder)
        {
            return actionOrder.Count == EMPTY_LIST_COUNT;
        }

        private UnitInstance GetCurrentUnit(List<UnitInstance> actionOrder)
        {
            return actionOrder[FIRST_UNIT_INDEX];
        }

        private bool ProcessSingleUnitAction(UnitInstance currentUnit, BattleContext battleContext)
        {
            if (IsUnitActionSuccessful(currentUnit, battleContext))
                return true;
            
            combatManager.ConsumeTurn(battleContext.BattleState);
            
            return CheckAndHandleBattleEnd(battleContext);
        }

        private bool IsUnitActionSuccessful(UnitInstance currentUnit, BattleContext battleContext)
        {
            return combatManager.ProcessUnitAction(currentUnit, battleContext.BattleState, battleContext.Player1Name, battleContext.Player2Name);
        }

        private bool CheckAndHandleBattleEnd(BattleContext battleContext)
        {
            if (combatManager.IsBattleOver(battleContext.BattleState))
            {
                AnnounceWinner(battleContext.BattleState, battleContext.Player1Name, battleContext.Player2Name);
                return true;
            }
            return false;
        }

        private void ProcessUnitTurnEnd(List<UnitInstance> actionOrder, TeamState currentTeam, UnitInstance currentUnit)
        {
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
