using System;
using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Core;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class UnitActionProcessor
    {
        private readonly IBattleView battleView;
        private readonly ActionCoordinator actionExecutor;
        private string lastSelectedAction;

        public UnitActionProcessor(IBattleView battleView, ActionCoordinator actionExecutor)
        {
            this.battleView = battleView;
            this.actionExecutor = actionExecutor;
        }

        public bool ProcessUnitAction(UnitInstance actingUnit, BattleState battleState, string player1Name, string player2Name)
        {
            bool actionCompleted = false;

            while (!actionCompleted)
            {
                actionCompleted = ProcessSingleAction(actingUnit, battleState, player1Name, player2Name);
                
                if (ShouldStopProcessing())
                {
                    return true;
                }
            }

            return false;
        }

        private bool ShouldStopProcessing()
        {
            return IsPlayerSurrendering(GetLastSelectedAction());
        }

        private bool ProcessSingleAction(UnitInstance actingUnit, BattleState battleState, string player1Name, string player2Name)
        {
            var availableActions = GetAvailableActions(actingUnit);
            ShowActionMenu(actingUnit, availableActions);

            var actionChoice = GetActionChoice(availableActions.Count);
            if (IsInvalidActionChoice(actionChoice))
                return false;

            return ExecuteSelectedAction(actingUnit, battleState, player1Name, player2Name, actionChoice);
        }

        private bool ExecuteSelectedAction(UnitInstance actingUnit, BattleState battleState, string player1Name, string player2Name, int actionChoice)
        {
            var availableActions = GetAvailableActions(actingUnit);
            var selectedAction = GetSelectedAction(availableActions, actionChoice);
            StoreLastSelectedAction(selectedAction);
            
            return actionExecutor.ExecuteSelectedAction(actingUnit, battleState, selectedAction, player1Name, player2Name);
        }

        private void ShowActionMenu(UnitInstance actingUnit, List<string> availableActions)
        {
            battleView.ShowActionMenu(actingUnit, availableActions);
        }

        private int GetActionChoice(int actionCount)
        {
            return battleView.GetActionChoice(actionCount);
        }

        private bool IsInvalidActionChoice(int actionChoice)
        {
            return actionChoice == -1;
        }

        private string GetSelectedAction(List<string> availableActions, int actionChoice)
        {
            return availableActions[actionChoice - 1];
        }

        private void StoreLastSelectedAction(string action)
        {
            lastSelectedAction = action;
        }

        private string GetLastSelectedAction()
        {
            return lastSelectedAction;
        }

        private bool IsPlayerSurrendering(string selectedAction)
        {
            return selectedAction == "Rendirse";
        }

        public List<string> GetAvailableActions(UnitInstance unit)
        {
            return unit.IsSamurai ? GetSamuraiActions() : GetRegularActions();
        }

        private List<string> GetSamuraiActions()
        {
            return new List<string>
            {
                "Atacar",
                "Disparar",
                "Usar Habilidad",
                "Invocar",
                "Pasar Turno",
                "Rendirse"
            };
        }

        private List<string> GetRegularActions()
        {
            return new List<string>
            {
                "Atacar",
                "Usar Habilidad",
                "Invocar",
                "Pasar Turno"
            };
        }
    }
}
