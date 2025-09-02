using System;
using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Core;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class CombatManager
    {
        private readonly UnitActionProcessor unitActionManager;
        private readonly ActionCoordinator actionExecutor;
        private readonly BattleStateProcessor battleStateManager;
        private readonly SkillProcessor skillManager;

        public CombatManager(Dictionary<string, Skill> skillData, IBattleView battleView)
        {
            var config = CreateActionCoordinatorConfig(battleView, skillData);
            this.actionExecutor = new ActionCoordinator(config);
            this.unitActionManager = new UnitActionProcessor(battleView, this.actionExecutor);
            this.battleStateManager = new BattleStateProcessor(battleView);
            this.skillManager = new SkillProcessor(battleView, skillData);
        }

        private ActionCoordinatorConfig CreateActionCoordinatorConfig(IBattleView battleView, Dictionary<string, Skill> skillData)
        {
            var surrenderHandler = new SurrenderProcessor(battleView);
            return new ActionCoordinatorConfig(battleView, surrenderHandler, skillData);
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

