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
                
                if (ShouldStopProcessing())
                {
                    return true;
                }
            }

            return false;
        }

        private bool ShouldStopProcessing()
        {
            return IsPlayerSurrendering(GetLastSelectedAction());
        }

        private bool ProcessSingleAction(UnitInstance actingUnit, BattleState battleState, string player1Name, string player2Name)
        {
            var availableActions = GetAvailableActions(actingUnit);
            ShowActionMenu(actingUnit, availableActions);

            var actionChoice = GetActionChoice(availableActions.Count);
            if (IsInvalidActionChoice(actionChoice))
                return false;

            var selectedAction = GetSelectedAction(availableActions, actionChoice);
            StoreLastSelectedAction(selectedAction);
            
            return ExecuteSelectedAction(actingUnit, battleState, selectedAction, player1Name, player2Name);
        }

        private void ShowActionMenu(UnitInstance actingUnit, List<string> availableActions)
        {
            battleView.ShowActionMenu(actingUnit, availableActions);
        }

        private int GetActionChoice(int actionCount)
        {
            return battleView.GetActionChoice(actionCount);
        }

        private bool IsInvalidActionChoice(int actionChoice)
        {
            return actionChoice == -1;
        }

        private string GetSelectedAction(List<string> availableActions, int actionChoice)
        {
            return availableActions[actionChoice - 1];
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
            return GetActionResult(selectedAction, actingUnit, battleState, player1Name, player2Name);
        }

        private bool GetActionResult(string selectedAction, UnitInstance actingUnit, BattleState battleState, string player1Name, string player2Name)
        {
            return selectedAction switch
            {
                "Atacar" => ExecutePhysicalAttack(actingUnit, battleState),
                "Disparar" => ExecuteGunAttack(actingUnit, battleState),
                "Usar Habilidad" => ProcessUseSkill(actingUnit, battleState),
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
            ProcessSurrender(battleState, player1Name, player2Name);
            return true;
        }

        private bool GetDefaultResult()
        {
            return false;
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
            if (HasNoAvailableTargets(availableTargets))
                return false;

            var selectedTarget = SelectTargetForAttack(attackContext, availableTargets);
            if (IsInvalidTarget(selectedTarget))
                return false;

            ExecuteAttackOnTarget(attackContext, selectedTarget);
            return true;
        }

        private bool HasNoAvailableTargets(List<UnitInstance> availableTargets)
        {
            return !availableTargets.Any();
        }

        private bool IsInvalidTarget(UnitInstance selectedTarget)
        {
            return selectedTarget == null;
        }

        private List<UnitInstance> GetAvailableTargetsForAttack(AttackContext attackContext)
        {
            var enemyTeam = GetEnemyTeam(attackContext.BattleState);
            return GetAvailableTargets(enemyTeam);
        }

        private UnitInstance SelectTargetForAttack(AttackContext attackContext, List<UnitInstance> availableTargets)
        {
            ShowTargetSelection(attackContext.Attacker, availableTargets);

            var targetChoice = GetTargetChoice(availableTargets.Count);
            if (IsInvalidTargetChoice(targetChoice, availableTargets.Count))
                return null;

            return GetSelectedTarget(availableTargets, targetChoice);
        }

        private void ShowTargetSelection(UnitInstance attacker, List<UnitInstance> availableTargets)
        {
            battleView.ShowTargetSelection(attacker, availableTargets);
        }

        private int GetTargetChoice(int targetCount)
        {
            return battleView.GetTargetChoice(targetCount);
        }

        private UnitInstance GetSelectedTarget(List<UnitInstance> availableTargets, int targetChoice)
        {
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
            ShowSkillSelection(unit, availableSkills);

            var skillChoice = GetSkillChoice(availableSkills.Count);
            return IsValidSkillChoice(skillChoice, availableSkills.Count);
        }

        private void ShowSkillSelection(UnitInstance unit, List<Skill> availableSkills)
        {
            battleView.ShowSkillSelection(unit, availableSkills);
        }

        private int GetSkillChoice(int skillCount)
        {
            return battleView.GetSkillChoice(skillCount);
        }

        private bool IsValidSkillChoice(int skillChoice, int skillCount)
        {
            return !IsInvalidSkillChoice(skillChoice, skillCount);
        }

        private bool IsInvalidSkillChoice(int skillChoice, int skillCount)
        {
            return skillChoice == -1 || skillChoice == skillCount + 1;
        }

        public List<Skill> GetAvailableSkills(UnitInstance unit)
        {
            var availableSkills = CreateEmptySkillList();
            PopulateAffordableSkills(availableSkills, unit);
            return availableSkills;
        }

        private List<Skill> CreateEmptySkillList()
        {
            return new List<Skill>();
        }

        private void PopulateAffordableSkills(List<Skill> availableSkills, UnitInstance unit)
        {
            foreach (var skillName in unit.Skills)
            {
                AddSkillIfAffordable(availableSkills, skillName, unit.MP);
            }
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
            ShowSurrenderInfo(surrenderInfo);
            return true;
        }

        private void ShowSurrenderInfo(SurrenderInfo surrenderInfo)
        {
            battleView.ShowSurrender(surrenderInfo.SurrenderingPlayerName, surrenderInfo.SurrenderingPlayerNumber, 
                                   surrenderInfo.WinnerName, surrenderInfo.WinnerNumber);
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
            return IsGunAttack(attackContext.AttackType)
                ? CalculateGunDamage(attackContext.Attacker.Skl)
                : CalculatePhysicalDamage(attackContext.Attacker.Str);
        }

        private bool IsGunAttack(AttackType attackType)
        {
            return attackType == AttackType.Gun;
        }

        private int CalculateGunDamage(int skill)
        {
            return damageCalculator.CalculateGunDamage(skill);
        }

        private int CalculatePhysicalDamage(int strength)
        {
            return damageCalculator.CalculatePhysicalDamage(strength);
        }

        private void ApplyDamageToTarget(UnitInstance target, int damage)
        {
            var newHP = CalculateNewHP(target.HP, damage);
            target.HP = newHP;
        }

        private int CalculateNewHP(int currentHP, int damage)
        {
            return Math.Max(0, currentHP - damage);
        }

        private void ShowAttackResult(AttackContext attackContext, UnitInstance target, int damage)
        {
            var isGunAttack = IsGunAttack(attackContext.AttackType);
            ShowAttackResultToView(attackContext.Attacker, target, damage, isGunAttack);
        }

        private void ShowAttackResultToView(UnitInstance attacker, UnitInstance target, int damage, bool isGunAttack)
        {
            battleView.ShowAttackResult(attacker, target, damage, isGunAttack);
        }

        public List<UnitInstance> GetTargets(TeamState enemyTeam)
        {
            return enemyTeam.AliveUnits.ToList();
        }

        public void ConsumeTurn(BattleState battleState)
        {
            ShowTurnConsumption();
            DecreaseFullTurns(battleState);
        }

        private void ShowTurnConsumption()
        {
            battleView.ShowTurnConsumption();
        }

        private void DecreaseFullTurns(BattleState battleState)
        {
            var newTurnCount = CalculateNewTurnCount(battleState.FullTurns);
            battleState.FullTurns = newTurnCount;
        }

        private int CalculateNewTurnCount(int currentTurns)
        {
            return Math.Max(0, currentTurns - 1);
        }

        public bool IsBattleOver(BattleState battleState)
        {
            return IsTeamDefeated(battleState.Team1) || IsTeamDefeated(battleState.Team2);
        }

        private bool IsTeamDefeated(TeamState team)
        {
            return !team.AliveUnits.Any();
        }

        public string GetWinner(BattleState battleState, string player1Name, string player2Name)
        {
            return IsTeam1Defeated(battleState) ? player2Name : player1Name;
        }

        private bool IsTeam1Defeated(BattleState battleState)
        {
            return !battleState.Team1.AliveUnits.Any();
        }

        public string GetWinnerNumber(BattleState battleState)
        {
            return IsTeam1Defeated(battleState) ? "J2" : "J1";
        }
    }
}

