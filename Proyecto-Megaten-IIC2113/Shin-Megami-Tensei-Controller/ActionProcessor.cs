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
        
        
        public bool ShouldProcessActionOrder(BattleContext battleContext, List<GetUnitInstance> actionOrder, TeamState currentTeam)
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

        private bool ShouldProcessSingleActionIteration(BattleContext battleContext, List<GetUnitInstance> actionOrder, TeamState currentTeam)
        {
            ShowBattleStatus(battleContext, actionOrder);
            
            if (IsActionOrderEmpty(actionOrder)) 
                return false;
            
            return ShouldProcessCurrentUnit(battleContext, actionOrder, currentTeam);
        }

        private void ShowBattleStatus(BattleContext battleContext, List<GetUnitInstance> actionOrder)
        {
            battleView.ShowBattlefield(battleContext.BattleState, battleContext.Player1Name, battleContext.Player2Name);
            battleView.ShowTurnCounters(battleContext.BattleState);
            battleView.ShowActionOrderBySpeed(actionOrder);
        }

        private bool IsActionOrderEmpty(List<GetUnitInstance> actionOrder)
        {
            return actionOrder.Count == EMPTY_LIST_COUNT;
        }

        private bool ShouldProcessCurrentUnit(BattleContext battleContext, List<GetUnitInstance> actionOrder, TeamState currentTeam)
        {
            var currentUnit = GetCurrentUnit(actionOrder);
            
            if (ShouldProcessSingleUnitAction(currentUnit, battleContext)) 
                return true;
            
            ProcessUnitTurnEnd(actionOrder, currentTeam, currentUnit);
            return false;
        }

        private GetUnitInstance GetCurrentUnit(List<GetUnitInstance> actionOrder)
        {
            return actionOrder[FIRST_UNIT_INDEX];
        }

        private bool ShouldProcessSingleUnitAction(GetUnitInstance currentGetUnit, BattleContext battleContext)
        {
            if (IsUnitActionSuccessful(currentGetUnit, battleContext))
                return true;
            
            combatManager.ConsumeTurn(battleContext.BattleState);
            
            return ShouldEndBattle(battleContext);
        }

        private bool IsUnitActionSuccessful(GetUnitInstance currentGetUnit, BattleContext battleContext)
        {
            var unitActionContext = new UnitActionContext(currentGetUnit, battleContext.BattleState, battleContext.Player1Name, battleContext.Player2Name);
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

        private void ProcessUnitTurnEnd(List<GetUnitInstance> actionOrder, TeamState currentTeam, GetUnitInstance currentGetUnit)
        {
            actionOrder.RemoveAt(FIRST_UNIT_INDEX);
            if (currentTeam.AliveUnits.Contains(currentGetUnit))
            {
                actionOrder.Add(currentGetUnit);
            }
        }
    }
}
