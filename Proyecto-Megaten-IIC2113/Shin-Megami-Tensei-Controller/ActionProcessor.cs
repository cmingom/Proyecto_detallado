using Shin_Megami_Tensei_View.ConsoleLib;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Core;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;

namespace Shin_Megami_Tensei
{
    public class ActionProcessor
    {
        private const int NO_TURNS_REMAINING = 0;
        private const int EMPTY_LIST_COUNT = 0;
        private const int FIRST_UNIT_INDEX = 0;
        
        private readonly BattleView battleView;
        private readonly CombatManager combatManager;

        public ActionProcessor(BattleView battleView, CombatManager combatManager)
        {
            this.battleView = battleView;
            this.combatManager = combatManager;
        }
        
        
        public bool ShouldProcessActionOrder(BattleContext battleContext, List<UnitInstanceContext> actionOrder, TeamState currentTeam)
        {
            while (ShouldContinueProcessingActions(battleContext))
            {
                if (ShouldProcessSingleActionIteration(battleContext, actionOrder, currentTeam))
                    return true;
            }
            return false;
        }

        private bool ShouldContinueProcessingActions(BattleContext battleContext)
        {
            return battleContext.BattleState.FullTurns > NO_TURNS_REMAINING && !combatManager.IsBattleOver(battleContext.BattleState);
        }

        private bool ShouldProcessSingleActionIteration(BattleContext battleContext, List<UnitInstanceContext> actionOrder, TeamState currentTeam)
        {
            ShowBattleStatus(battleContext, actionOrder);
            
            if (IsActionOrderEmpty(actionOrder)) 
                return false;
            
            return ShouldProcessCurrentUnit(battleContext, actionOrder, currentTeam);
        }

        private void ShowBattleStatus(BattleContext battleContext, List<UnitInstanceContext> actionOrder)
        {
            battleView.ShowBattlefield(battleContext.BattleState, battleContext.Player1Name, battleContext.Player2Name);
            battleView.ShowTurnCounters(battleContext.BattleState);
            battleView.ShowActionOrderBySpeed(actionOrder);
        }

        private bool IsActionOrderEmpty(List<UnitInstanceContext> actionOrder)
        {
            return actionOrder.Count == EMPTY_LIST_COUNT;
        }

        private bool ShouldProcessCurrentUnit(BattleContext battleContext, List<UnitInstanceContext> actionOrder, TeamState currentTeam)
        {
            var currentUnit = GetCurrentUnit(actionOrder);
            
            if (ShouldProcessSingleUnitAction(currentUnit, battleContext)) 
                return true;
            
            ProcessUnitTurnEnd(actionOrder, currentTeam, currentUnit);
            return false;
        }

        private UnitInstanceContext GetCurrentUnit(List<UnitInstanceContext> actionOrder)
        {
            return actionOrder[FIRST_UNIT_INDEX];
        }

        private bool ShouldProcessSingleUnitAction(UnitInstanceContext currentUnit, BattleContext battleContext)
        {
            if (IsUnitActionSuccessful(currentUnit, battleContext))
                return true;
            
            combatManager.ConsumeTurn(battleContext.BattleState);
            
            return ShouldEndBattle(battleContext);
        }

        private bool IsUnitActionSuccessful(UnitInstanceContext currentUnit, BattleContext battleContext)
        {
            var unitActionContext = new UnitActionContext(currentUnit, battleContext.BattleState, battleContext.Player1Name, battleContext.Player2Name);
            return combatManager.CanProcessUnitAction(unitActionContext);
        }

        private bool ShouldEndBattle(BattleContext battleContext)
        {
            if (IsBattleOver(battleContext.BattleState))
            {
                HandleBattleEnd(battleContext);
                return true;
            }
            return false;
        }

        private bool IsBattleOver(BattleState battleState)
        {
            return combatManager.IsBattleOver(battleState);
        }

        private void HandleBattleEnd(BattleContext battleContext)
        {
            AnnounceWinner(battleContext.BattleState, battleContext.Player1Name, battleContext.Player2Name);
        }

        private void AnnounceWinner(BattleState battleState, string player1Name, string player2Name)
        {
            var winnerName = combatManager.GetWinner(battleState, player1Name, player2Name);
            var winnerNumber = combatManager.GetWinnerNumber(battleState);
            battleView.ShowWinner(winnerName, winnerNumber);
        }

        private void ProcessUnitTurnEnd(List<UnitInstanceContext> actionOrder, TeamState currentTeam, UnitInstanceContext currentUnit)
        {
            actionOrder.RemoveAt(FIRST_UNIT_INDEX);
            if (currentTeam.AliveUnits.Contains(currentUnit))
            {
                actionOrder.Add(currentUnit);
            }
        }
    }
}
