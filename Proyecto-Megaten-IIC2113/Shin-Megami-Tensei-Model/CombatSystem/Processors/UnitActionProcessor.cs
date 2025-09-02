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
        private const int INVALID_ACTION_CHOICE = -1;
        private const int ACTION_INDEX_OFFSET = 1;
        private const string ATTACK_ACTION = "Atacar";
        private const string GUN_ACTION = "Disparar";
        private const string SKILL_ACTION = "Usar Habilidad";
        private const string SUMMON_ACTION = "Invocar";
        private const string PASS_TURN_ACTION = "Pasar Turno";
        private const string SURRENDER_ACTION = "Rendirse";
        
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

        private bool ProcessSingleAction(UnitInstance actingUnit, BattleState battleState, string player1Name, string player2Name)
        {
            var availableActions = GetAvailableActions(actingUnit);
            ShowActionMenu(actingUnit, availableActions);

            var actionChoice = GetActionChoice(availableActions.Count);
            if (IsInvalidActionChoice(actionChoice))
                return false;

            return ExecuteSelectedAction(actingUnit, battleState, player1Name, player2Name, actionChoice);
        }

        public List<string> GetAvailableActions(UnitInstance unit)
        {
            return unit.IsSamurai ? GetSamuraiActions() : GetRegularActions();
        }

        private List<string> GetSamuraiActions()
        {
            return new List<string>
            {
                ATTACK_ACTION,
                GUN_ACTION,
                SKILL_ACTION,
                SUMMON_ACTION,
                PASS_TURN_ACTION,
                SURRENDER_ACTION
            };
        }

        private List<string> GetRegularActions()
        {
            return new List<string>
            {
                ATTACK_ACTION,
                SKILL_ACTION,
                SUMMON_ACTION,
                PASS_TURN_ACTION
            };
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
            return actionChoice == INVALID_ACTION_CHOICE;
        }

        private bool ExecuteSelectedAction(UnitInstance actingUnit, BattleState battleState, string player1Name, string player2Name, int actionChoice)
        {
            var availableActions = GetAvailableActions(actingUnit);
            var selectedAction = GetSelectedAction(availableActions, actionChoice);
            StoreLastSelectedAction(selectedAction);
            
            return actionExecutor.ExecuteSelectedAction(actingUnit, battleState, selectedAction, player1Name, player2Name);
        }

        private string GetSelectedAction(List<string> availableActions, int actionChoice)
        {
            return availableActions[actionChoice - ACTION_INDEX_OFFSET];
        }

        private void StoreLastSelectedAction(string action)
        {
            lastSelectedAction = action;
        }

        private bool ShouldStopProcessing()
        {
            return IsPlayerSurrendering(GetLastSelectedAction());
        }

        private string GetLastSelectedAction()
        {
            return lastSelectedAction;
        }

        private bool IsPlayerSurrendering(string selectedAction)
        {
            return selectedAction == SURRENDER_ACTION;
        }
    }
}
