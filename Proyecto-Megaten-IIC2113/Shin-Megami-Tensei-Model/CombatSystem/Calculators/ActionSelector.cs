using System;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Core;
using System.Collections.Generic;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class ActionSelector
    {
        private const string ATTACK_ACTION = "Atacar";
        private const string GUN_ACTION = "Disparar";
        private const string SKILL_ACTION = "Usar Habilidad";
        private const string SUMMON_ACTION = "Invocar";
        private const string PASS_TURN_ACTION = "Pasar Turno";
        private const string SURRENDER_ACTION = "Rendirse";
        
        private readonly IBattleView battleView;
        private readonly SurrenderProcessor surrenderHandler;
        private readonly AttackProcessor attackExecutor;
        private readonly SkillProcessor skillManager;

        public ActionSelector(IBattleView battleView, SurrenderProcessor surrenderHandler, Dictionary<string, Skill> skillData)
        {
            this.battleView = battleView;
            this.surrenderHandler = surrenderHandler;
            this.attackExecutor = new AttackProcessor(battleView);
            this.skillManager = new SkillProcessor(battleView, skillData);
        }

        public bool ExecuteSelectedAction(UnitInstance actingUnit, BattleState battleState, string selectedAction, string player1Name, string player2Name)
        {
            return GetActionResult(selectedAction, actingUnit, battleState, player1Name, player2Name);
        }

        private bool GetActionResult(string selectedAction, UnitInstance actingUnit, BattleState battleState, string player1Name, string player2Name)
        {
            return selectedAction switch
            {
                ATTACK_ACTION => attackExecutor.ExecutePhysicalAttack(actingUnit, battleState),
                GUN_ACTION => attackExecutor.ExecuteGunAttack(actingUnit, battleState),
                SKILL_ACTION => skillManager.ProcessUseSkill(actingUnit, battleState),
                SUMMON_ACTION => GetSummonResult(),
                PASS_TURN_ACTION => GetPassTurnResult(),
                SURRENDER_ACTION => GetSurrenderResult(battleState, player1Name, player2Name),
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
