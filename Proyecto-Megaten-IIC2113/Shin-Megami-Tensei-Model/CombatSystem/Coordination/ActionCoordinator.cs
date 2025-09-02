using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
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
            return new ActionSelector(config.BattleView, config.SurrenderProcessor, config.SkillData);
        }

        public bool ExecuteSelectedAction(UnitInstance actingUnit, BattleState battleState, string selectedAction, string player1Name, string player2Name)
        {
            return actionSelector.ExecuteSelectedAction(actingUnit, battleState, selectedAction, player1Name, player2Name);
        }
        
    }
}
