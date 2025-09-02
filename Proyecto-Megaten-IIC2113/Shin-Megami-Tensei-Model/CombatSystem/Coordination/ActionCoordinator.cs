using System.Collections.Generic;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class ActionCoordinator
    {
        private readonly ActionSelector actionSelector;
        private readonly AttackProcessor attackExecutor;
        private readonly SkillProcessor skillManager;

        public ActionCoordinator(ActionCoordinatorConfig config)
        {
            this.actionSelector = CreateActionSelector(config);
            this.attackExecutor = CreateAttackProcessor(config);
            this.skillManager = CreateSkillProcessor(config);
        }

        private ActionSelector CreateActionSelector(ActionCoordinatorConfig config)
        {
            return new ActionSelector(config.BattleView, config.SurrenderProcessor, config.SkillData);
        }

        private AttackProcessor CreateAttackProcessor(ActionCoordinatorConfig config)
        {
            return new AttackProcessor(config.BattleView);
        }

        private SkillProcessor CreateSkillProcessor(ActionCoordinatorConfig config)
        {
            return new SkillProcessor(config.BattleView, config.SkillData);
        }

        public bool ExecuteSelectedAction(UnitInstance actingUnit, BattleState battleState, string selectedAction, string player1Name, string player2Name)
        {
            return actionSelector.ExecuteSelectedAction(actingUnit, battleState, selectedAction, player1Name, player2Name);
        }

        public bool ExecutePhysicalAttack(UnitInstance attacker, BattleState battleState)
        {
            return attackExecutor.ExecutePhysicalAttack(attacker, battleState);
        }

        public bool ExecuteGunAttack(UnitInstance attacker, BattleState battleState)
        {
            return attackExecutor.ExecuteGunAttack(attacker, battleState);
        }

        public List<Skill> GetAvailableSkills(UnitInstance unit)
        {
            return skillManager.GetAvailableSkills(unit);
        }
    }
}
