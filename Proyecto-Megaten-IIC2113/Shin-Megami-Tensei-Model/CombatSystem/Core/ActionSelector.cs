using System;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Core;
using System.Collections.Generic;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class ActionSelector
    {
        private readonly IBattleView battleView;
        private readonly SurrenderHandler surrenderHandler;
        private readonly AttackExecutor attackExecutor;
        private readonly SkillManager skillManager;

        public ActionSelector(IBattleView battleView, SurrenderHandler surrenderHandler, Dictionary<string, Skill> skillData)
        {
            this.battleView = battleView;
            this.surrenderHandler = surrenderHandler;
            this.attackExecutor = new AttackExecutor(battleView);
            this.skillManager = new SkillManager(battleView, skillData);
        }

        public bool ExecuteSelectedAction(UnitInstance actingUnit, BattleState battleState, string selectedAction, string player1Name, string player2Name)
        {
            return GetActionResult(selectedAction, actingUnit, battleState, player1Name, player2Name);
        }

        private bool GetActionResult(string selectedAction, UnitInstance actingUnit, BattleState battleState, string player1Name, string player2Name)
        {
            return selectedAction switch
            {
                "Atacar" => attackExecutor.ExecutePhysicalAttack(actingUnit, battleState),
                "Disparar" => attackExecutor.ExecuteGunAttack(actingUnit, battleState),
                "Usar Habilidad" => skillManager.ProcessUseSkill(actingUnit, battleState),
                "Invocar" => GetSummonResult(),
                "Pasar Turno" => GetPassTurnResult(),
                "Rendirse" => GetSurrenderResult(battleState, player1Name, player2Name),
                _ => GetDefaultResult()
            };
        }

        private bool GetSummonResult()
        {
            return true;
        }

        private bool GetPassTurnResult()
        {
            return true;
        }

        private bool GetSurrenderResult(BattleState battleState, string player1Name, string player2Name)
        {
            surrenderHandler.ProcessSurrender(battleState, player1Name, player2Name);
            return true;
        }

        private bool GetDefaultResult()
        {
            return false;
        }
    }
}
