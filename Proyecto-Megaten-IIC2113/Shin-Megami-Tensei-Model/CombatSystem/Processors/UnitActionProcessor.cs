using System.Collections.Generic;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class UnitActionProcessor
    {
        private const int InvalidActionChoice = -1;
        private const int ActionIndexOffset = 1;
        private const string AttackActionName = "Atacar";
        private const string GunActionName = "Disparar";
        private const string SkillActionName = "Usar Habilidad";
        private const string SummonActionName = "Invocar";
        private const string PassTurnActionName = "Pasar Turno";
        private const string SurrenderActionName = "Rendirse";

        private readonly IBattleView battleView;
        private readonly ActionCoordinator actionCoordinator;

        public UnitActionProcessor(IBattleView battleView, ActionCoordinator actionCoordinator)
        {
            this.battleView = battleView;
            this.actionCoordinator = actionCoordinator;
        }

        public bool ExecuteUnitTurn(UnitActionContext context)
        {
            if (context.BattleState.IsBattleFinished)
            {
                return true;
            }

            var actionCompleted = false;

            while (!actionCompleted)
            {
                if (context.BattleState.IsBattleFinished)
                {
                    return true;
                }

                actionCompleted = ExecuteSingleAction(context);

                if (context.BattleState.IsBattleFinished)
                {
                    return true;
                }
            }

            return false;
        }

        private bool ExecuteSingleAction(UnitActionContext context)
        {
            var actionChoice = GetUserActionChoice(context.ActingUnit);
            if (IsInvalidActionChoice(actionChoice))
            {
                return false;
            }

            return ExecuteSelectedAction(context, actionChoice);
        }

        private int GetUserActionChoice(UnitInstanceContext actingUnit)
        {
            var availableActions = GetAvailableActions(actingUnit);
            battleView.ShowActionMenu(actingUnit, availableActions);
            return battleView.GetActionChoice(availableActions.Count);
        }

        public List<string> GetAvailableActions(UnitInstanceContext unit)
        {
            return unit.IsSamurai ? GetSamuraiActions() : GetMonsterActions();
        }

        private List<string> GetSamuraiActions()
        {
            return new List<string>
            {
                AttackActionName,
                GunActionName,
                SkillActionName,
                SummonActionName,
                PassTurnActionName,
                SurrenderActionName
            };
        }

        private List<string> GetMonsterActions()
        {
            return new List<string>
            {
                AttackActionName,
                SkillActionName,
                SummonActionName,
                PassTurnActionName
            };
        }

        private bool ExecuteSelectedAction(UnitActionContext context, int actionChoice)
        {
            var selectedAction = GetSelectedAction(context.ActingUnit, actionChoice);
            return ExecuteAction(context, selectedAction);
        }

        private string GetSelectedAction(UnitInstanceContext actingUnit, int actionChoice)
        {
            var availableActions = GetAvailableActions(actingUnit);
            return availableActions[actionChoice - ActionIndexOffset];
        }

        private bool ExecuteAction(UnitActionContext context, string selectedAction)
        {
            var actionProcessingContext = new ActionProcessingContext(context.ActingUnit, context.BattleState, selectedAction, context.Player1Name, context.Player2Name);
            return actionCoordinator.ProcessSelectedAction(actionProcessingContext);
        }

        private static bool IsInvalidActionChoice(int actionChoice)
        {
            return actionChoice == InvalidActionChoice;
        }
    }
}
