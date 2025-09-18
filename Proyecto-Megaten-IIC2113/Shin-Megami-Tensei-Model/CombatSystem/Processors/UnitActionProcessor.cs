using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;

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
        
        // to do: Unidad abstracta
        public bool CanProcessUnitAction(UnitActionContext context)
        {
            System.Console.WriteLine($"DEBUG UnitActionProcessor: CanProcessUnitAction() iniciado para {context.ActingUnit.Name}");
            bool actionCompleted = false;

            while (!actionCompleted)
            {
                System.Console.WriteLine($"DEBUG UnitActionProcessor: Procesando acción para {context.ActingUnit.Name}");
                actionCompleted = CanProcessSingleAction(context);
                System.Console.WriteLine($"DEBUG UnitActionProcessor: Acción completada: {actionCompleted}");
                
                if (ShouldStopProcessing())
                {
                    System.Console.WriteLine($"DEBUG UnitActionProcessor: Deteniendo procesamiento para {context.ActingUnit.Name}");
                    return true;
                }
            }

            System.Console.WriteLine($"DEBUG UnitActionProcessor: CanProcessUnitAction() completado para {context.ActingUnit.Name} - resultado: false");
            return false;
        }
        
        private bool CanProcessSingleAction(UnitActionContext context)
        {
            System.Console.WriteLine($"DEBUG UnitActionProcessor: CanProcessSingleAction() para {context.ActingUnit.Name}");
            var actionChoice = GetUserActionChoice(context.ActingUnit);
            System.Console.WriteLine($"DEBUG UnitActionProcessor: Elección de acción: {actionChoice}");
            if (IsInvalidActionChoice(actionChoice))
            {
                System.Console.WriteLine($"DEBUG UnitActionProcessor: Elección inválida para {context.ActingUnit.Name}");
                return false;
            }

            var result = CanExecuteSelectedAction(context, actionChoice);
            System.Console.WriteLine($"DEBUG UnitActionProcessor: CanProcessSingleAction() - resultado: {result}");
            return result;
        }

        private int GetUserActionChoice(UnitInstanceContext actingUnit)
        {
            var availableActions = GetAvailableActions(actingUnit);
            ShowActionMenu(actingUnit, availableActions);
            return GetActionChoice(availableActions.Count);
        }

        public List<string> GetAvailableActions(UnitInstanceContext unit)
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

        private void ShowActionMenu(UnitInstanceContext actingUnit, List<string> availableActions)
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

        private bool CanExecuteSelectedAction(UnitActionContext context, int actionChoice)
        {
            var selectedAction = GetSelectedAction(context.ActingUnit, actionChoice);
            StoreLastSelectedAction(selectedAction);
            
            return CanProcessAction(context, selectedAction);
        }

        private string GetSelectedAction(UnitInstanceContext actingUnit, int actionChoice)
        {
            var availableActions = GetAvailableActions(actingUnit);
            return availableActions[actionChoice - ACTION_INDEX_OFFSET];
        }

        private bool CanProcessAction(UnitActionContext context, string selectedAction)
        {
            System.Console.WriteLine($"DEBUG UnitActionProcessor: Procesando acción '{selectedAction}' para {context.ActingUnit.Name}");
            var actionProcessingContext = new ActionProcessingContext(context.ActingUnit, context.BattleState, selectedAction, context.Player1Name, context.Player2Name);
            var result = actionExecutor.CanProcessSelectedAction(actionProcessingContext);
            System.Console.WriteLine($"DEBUG UnitActionProcessor: Resultado de '{selectedAction}' para {context.ActingUnit.Name}: {result}");
            return result;
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
