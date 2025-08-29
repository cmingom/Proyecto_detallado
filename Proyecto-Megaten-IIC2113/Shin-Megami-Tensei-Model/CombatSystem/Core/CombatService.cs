using System;
using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Core;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class CombatService
    {
        private readonly DamageCalculator damageCalculator;
        private readonly Dictionary<string, Skill> skillData;
        private readonly IBattleView battleView;

        public CombatService(Dictionary<string, Skill> skillData, IBattleView battleView)
        {
            this.damageCalculator = new DamageCalculator();
            this.skillData = skillData;
            this.battleView = battleView;
        }

        public List<UnitInstance> CalculateActionOrder(TeamState team)
        {
            return team.AliveUnits.OrderByDescending(u => u.Spd).ToList();
        }

        public int CalculateNextTurnCount(TeamState team)
        {
            return team.AliveUnits.Count();
        }

        public bool ProcessUnitAction(UnitInstance actingUnit, BattleState battleState, string player1Name, string player2Name)
        {
            bool actionCompleted = false;

            while (!actionCompleted)
            {
                actionCompleted = ProcessSingleAction(actingUnit, battleState, player1Name, player2Name);
                
                if (IsPlayerSurrendering(GetLastSelectedAction()))
                {
                    return true;
                }
            }

            return false;
        }

        private bool ProcessSingleAction(UnitInstance actingUnit, BattleState battleState, string player1Name, string player2Name)
        {
            var availableActions = GetAvailableActions(actingUnit);
            battleView.ShowActionMenu(actingUnit, availableActions);

            var actionChoice = battleView.GetActionChoice(availableActions.Count);
            if (actionChoice == -1) return false;

            var selectedAction = availableActions[actionChoice - 1];
            StoreLastSelectedAction(selectedAction);
            
            return ExecuteSelectedAction(actingUnit, battleState, selectedAction, player1Name, player2Name);
        }

        private string lastSelectedAction;

        private void StoreLastSelectedAction(string action)
        {
            lastSelectedAction = action;
        }

        private string GetLastSelectedAction()
        {
            return lastSelectedAction;
        }

        private bool ExecuteSelectedAction(UnitInstance actingUnit, BattleState battleState, string selectedAction, string player1Name, string player2Name)
        {
            switch (selectedAction)
            {
                case "Atacar":
                    return ExecutePhysicalAttack(actingUnit, battleState);
                case "Disparar":
                    return ExecuteGunAttack(actingUnit, battleState);
                case "Usar Habilidad":
                    return ProcessUseSkill(actingUnit, battleState);
                case "Invocar":
                    return true;
                case "Pasar Turno":
                    return true;
                case "Rendirse":
                    ProcessSurrender(battleState, player1Name, player2Name);
                    return true;
                default:
                    return false;
            }
        }

        private bool IsPlayerSurrendering(string selectedAction)
        {
            return selectedAction == "Rendirse";
        }

        public bool ExecutePhysicalAttack(UnitInstance attacker, BattleState battleState)
        {
            var attackContext = new AttackContext(attacker, battleState, AttackType.Physical);
            return ProcessAttack(attackContext);
        }

        public bool ExecuteGunAttack(UnitInstance attacker, BattleState battleState)
        {
            var attackContext = new AttackContext(attacker, battleState, AttackType.Gun);
            return ProcessAttack(attackContext);
        }

        public List<string> GetAvailableActions(UnitInstance unit)
        {
            return unit.IsSamurai ? GetSamuraiActions() : GetRegularActions();
        }

        private List<string> GetSamuraiActions()
        {
            return new List<string>
            {
                "Atacar",
                "Disparar",
                "Usar Habilidad",
                "Invocar",
                "Pasar Turno",
                "Rendirse"
            };
        }

        private List<string> GetRegularActions()
        {
            return new List<string>
            {
                "Atacar",
                "Usar Habilidad",
                "Invocar",
                "Pasar Turno"
            };
        }

        private bool ProcessAttack(AttackContext attackContext)
        {
            var availableTargets = GetAvailableTargetsForAttack(attackContext);
            if (!availableTargets.Any()) return false;

            var selectedTarget = SelectTargetForAttack(attackContext, availableTargets);
            if (selectedTarget == null) return false;

            ExecuteAttackOnTarget(attackContext, selectedTarget);
            return true;
        }

        private List<UnitInstance> GetAvailableTargetsForAttack(AttackContext attackContext)
        {
            var enemyTeam = GetEnemyTeam(attackContext.BattleState);
            return GetAvailableTargets(enemyTeam);
        }

        private UnitInstance SelectTargetForAttack(AttackContext attackContext, List<UnitInstance> availableTargets)
        {
            battleView.ShowTargetSelection(attackContext.Attacker, availableTargets);

            var targetChoice = battleView.GetTargetChoice(availableTargets.Count);
            if (IsInvalidTargetChoice(targetChoice, availableTargets.Count))
                return null;

            return availableTargets[targetChoice - 1];
        }

        private void ExecuteAttackOnTarget(AttackContext attackContext, UnitInstance selectedTarget)
        {
            var damage = CalculateAttackDamage(attackContext);
            ApplyDamageToTarget(selectedTarget, damage);
            ShowAttackResult(attackContext, selectedTarget, damage);
        }

        private bool ProcessUseSkill(UnitInstance unit, BattleState battleState)
        {
            var availableSkills = GetAvailableSkills(unit);
            battleView.ShowSkillSelection(unit, availableSkills);

            var skillChoice = battleView.GetSkillChoice(availableSkills.Count);
            return !IsInvalidSkillChoice(skillChoice, availableSkills.Count);
        }

        private bool IsInvalidSkillChoice(int skillChoice, int skillCount)
        {
            return skillChoice == -1 || skillChoice == skillCount + 1;
        }

        public List<Skill> GetAvailableSkills(UnitInstance unit)
        {
            var availableSkills = new List<Skill>();
            foreach (var skillName in unit.Skills)
            {
                AddSkillIfAffordable(availableSkills, skillName, unit.MP);
            }
            return availableSkills;
        }

        private void AddSkillIfAffordable(List<Skill> availableSkills, string skillName, int unitMP)
        {
            if (skillData.TryGetValue(skillName, out var skill) && skill.Cost <= unitMP)
            {
                availableSkills.Add(skill);
            }
        }

        private bool ProcessSurrender(BattleState battleState, string player1Name, string player2Name)
        {
            var surrenderInfo = CreateSurrenderInfo(battleState, player1Name, player2Name);
            battleView.ShowSurrender(surrenderInfo.SurrenderingPlayerName, surrenderInfo.SurrenderingPlayerNumber, 
                                   surrenderInfo.WinnerName, surrenderInfo.WinnerNumber);
            return true;
        }

        private SurrenderInfo CreateSurrenderInfo(BattleState battleState, string player1Name, string player2Name)
        {
            var surrenderingPlayerName = GetSurrenderingPlayerName(battleState, player1Name, player2Name);
            var surrenderingPlayerNumber = GetSurrenderingPlayerNumber(battleState);
            var winnerName = GetSurrenderWinnerName(battleState, player1Name, player2Name);
            var winnerNumber = GetSurrenderWinnerNumber(battleState);

            return new SurrenderInfo(surrenderingPlayerName, surrenderingPlayerNumber, winnerName, winnerNumber);
        }

        private string GetSurrenderingPlayerName(BattleState battleState, string player1Name, string player2Name)
        {
            return battleState.IsPlayer1Turn ? player1Name : player2Name;
        }

        private string GetSurrenderingPlayerNumber(BattleState battleState)
        {
            return battleState.IsPlayer1Turn ? "J1" : "J2";
        }

        private string GetSurrenderWinnerName(BattleState battleState, string player1Name, string player2Name)
        {
            return battleState.IsPlayer1Turn ? player2Name : player1Name;
        }

        private string GetSurrenderWinnerNumber(BattleState battleState)
        {
            return battleState.IsPlayer1Turn ? "J2" : "J1";
        }

        private TeamState GetEnemyTeam(BattleState battleState)
        {
            return battleState.IsPlayer1Turn ? battleState.Team2 : battleState.Team1;
        }

        private List<UnitInstance> GetAvailableTargets(TeamState enemyTeam)
        {
            return enemyTeam.AliveUnits.ToList();
        }

        private bool IsInvalidTargetChoice(int targetChoice, int targetCount)
        {
            return targetChoice == -1 || targetChoice == targetCount + 1;
        }

        private int CalculateAttackDamage(AttackContext attackContext)
        {
            return attackContext.AttackType == AttackType.Gun
                ? damageCalculator.CalculateGunDamage(attackContext.Attacker.Skl)
                : damageCalculator.CalculatePhysicalDamage(attackContext.Attacker.Str);
        }

        private void ApplyDamageToTarget(UnitInstance target, int damage)
        {
            target.HP = Math.Max(0, target.HP - damage);
        }

        private void ShowAttackResult(AttackContext attackContext, UnitInstance target, int damage)
        {
            var isGunAttack = attackContext.AttackType == AttackType.Gun;
            battleView.ShowAttackResult(attackContext.Attacker, target, damage, isGunAttack);
        }

        public List<UnitInstance> GetTargets(TeamState enemyTeam)
        {
            return enemyTeam.AliveUnits.ToList();
        }

        public void ConsumeTurn(BattleState battleState)
        {
            battleView.ShowTurnConsumption();
            DecreaseFullTurns(battleState);
        }

        private void DecreaseFullTurns(BattleState battleState)
        {
            battleState.FullTurns = Math.Max(0, battleState.FullTurns - 1);
        }

        public bool IsBattleOver(BattleState battleState)
        {
            return !battleState.Team1.AliveUnits.Any() || !battleState.Team2.AliveUnits.Any();
        }

        public string GetWinner(BattleState battleState, string player1Name, string player2Name)
        {
            return !battleState.Team1.AliveUnits.Any() ? player2Name : player1Name;
        }

        public string GetWinnerNumber(BattleState battleState)
        {
            return !battleState.Team1.AliveUnits.Any() ? "J2" : "J1";
        }
    }
}

