using Shin_Megami_Tensei_Model.CombatSystem.Contexts;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class ActionCoordinator
    {
        private readonly ActionSelector actionSelector;

        public ActionCoordinator(ActionCoordinatorConfig config)
        {
            this.actionSelector = CreateActionSelector(config);
        }

        private ActionSelector CreateActionSelector(ActionCoordinatorConfig config)
        {
            var actionSelectorConfig = new ActionSelectorConfig(config.BattleView, config.SurrenderProcessor, config.SkillData);
            return new ActionSelector(actionSelectorConfig);
        }

        public bool CanProcessSelectedAction(ActionProcessingContext context)
        {
            var actionContext = new ActionContext(context.ActingUnit, context.BattleState, context.Player1Name, context.Player2Name);
            return actionSelector.CanProcessSelectedAction(actionContext, context.SelectedAction);
        }
        
    }
}
