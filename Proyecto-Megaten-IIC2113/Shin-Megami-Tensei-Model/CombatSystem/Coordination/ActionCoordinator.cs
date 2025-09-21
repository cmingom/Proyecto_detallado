using Shin_Megami_Tensei_Model.CombatSystem.Contexts;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class ActionCoordinator
    {
        private readonly ActionSelector actionSelector;

        public ActionCoordinator(ActionCoordinatorConfig config)
        {
            actionSelector = CreateActionSelector(config);
        }

        private ActionSelector CreateActionSelector(ActionCoordinatorConfig config)
        {
            var targetSelector = new TargetSelector(config.BattleView);
            var damageCalculator = new DamageCalculator();
            var turnOutcomeProcessor = new TurnOutcomeProcessor(config.BattleView);
            var actionSelectorConfig = new ActionSelectorConfig(config.BattleView, config.SurrenderProcessor, config.PassTurnProcessor, config.SkillData, targetSelector, damageCalculator, turnOutcomeProcessor);
            return new ActionSelector(actionSelectorConfig);
        }

        public bool CanProcessSelectedAction(ActionProcessingContext context)
        {
            var actionContext = new ActionContext(context.ActingUnit, context.BattleState, context.Player1Name, context.Player2Name);
            return actionSelector.CanProcessSelectedAction(actionContext, context.SelectedAction);
        }
    }
}
