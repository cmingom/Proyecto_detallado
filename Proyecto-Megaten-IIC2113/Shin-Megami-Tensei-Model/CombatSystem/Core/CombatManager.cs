using System;
using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Core;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class CombatManager
    {
        private readonly UnitActionManager unitActionManager;
        private readonly ActionExecutor actionExecutor;
        private readonly BattleStateManager battleStateManager;
        private readonly SkillManager skillManager;

        public CombatManager(Dictionary<string, Skill> skillData, IBattleView battleView)
        {
            var surrenderHandler = new SurrenderHandler(battleView);
            this.actionExecutor = new ActionExecutor(battleView, surrenderHandler, skillData);
            this.unitActionManager = new UnitActionManager(battleView, this.actionExecutor);
            this.battleStateManager = new BattleStateManager(battleView);
            this.skillManager = new SkillManager(battleView, skillData);
        }

        public List<UnitInstance> CalculateActionOrder(TeamState team)
        {
            return battleStateManager.CalculateActionOrder(team);
        }

        public int CalculateNextTurnCount(TeamState team)
        {
            return battleStateManager.CalculateNextTurnCount(team);
        }

        public bool ProcessUnitAction(UnitInstance actingUnit, BattleState battleState, string player1Name, string player2Name)
        {
            return unitActionManager.ProcessUnitAction(actingUnit, battleState, player1Name, player2Name);
        }

        public bool ExecutePhysicalAttack(UnitInstance attacker, BattleState battleState)
        {
            return actionExecutor.ExecutePhysicalAttack(attacker, battleState);
        }

        public bool ExecuteGunAttack(UnitInstance attacker, BattleState battleState)
        {
            return actionExecutor.ExecuteGunAttack(attacker, battleState);
        }

        public List<string> GetAvailableActions(UnitInstance unit)
        {
            return unitActionManager.GetAvailableActions(unit);
        }

        public List<Skill> GetAvailableSkills(UnitInstance unit)
        {
            return skillManager.GetAvailableSkills(unit);
        }

        public List<UnitInstance> GetTargets(TeamState enemyTeam)
        {
            return battleStateManager.GetTargets(enemyTeam);
        }

        public void ConsumeTurn(BattleState battleState)
        {
            battleStateManager.ConsumeTurn(battleState);
        }

        public bool IsBattleOver(BattleState battleState)
        {
            return battleStateManager.IsBattleOver(battleState);
        }

        public string GetWinner(BattleState battleState, string player1Name, string player2Name)
        {
            return battleStateManager.GetWinner(battleState, player1Name, player2Name);
        }

        public string GetWinnerNumber(BattleState battleState)
        {
            return battleStateManager.GetWinnerNumber(battleState);
        }
    }
}

