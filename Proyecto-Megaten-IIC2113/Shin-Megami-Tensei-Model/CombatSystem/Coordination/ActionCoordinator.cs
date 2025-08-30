using System.Collections.Generic;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class ActionCoordinator
    {
        private readonly ActionSelector actionSelector;
        private readonly AttackProcessor attackExecutor;
        private readonly SkillProcessor skillManager;

        public ActionCoordinator(IBattleView battleView, SurrenderProcessor surrenderProcessor, Dictionary<string, Skill> skillData)
        {
            this.actionSelector = new ActionSelector(battleView, surrenderProcessor, skillData);
            this.attackExecutor = new AttackProcessor(battleView);
            this.skillManager = new SkillProcessor(battleView, skillData);
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
