using System;
using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_View.ConsoleLib;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Core;
using Shin_Megami_Tensei_View;

namespace Shin_Megami_Tensei
{
	public class BattleEngine
	{
		private readonly BattleView battleView;
		private readonly DamageCalculator damageCalculator;
		private readonly Dictionary<string, Skill> skillData;
		
		public BattleEngine(View view, Dictionary<string, Skill> skillData)
		{
			this.battleView = new BattleView(view);
			this.damageCalculator = new DamageCalculator();
			this.skillData = skillData;
		}
		
		public void StartBattle(BattleState battleState, string player1Name, string player2Name)
		{
			while (!IsBattleOver(battleState))
			{
				var currentTeam = battleState.IsPlayer1Turn ? battleState.Team1 : battleState.Team2;
				var currentPlayerName = battleState.IsPlayer1Turn ? player1Name : player2Name;
				var playerNumber = battleState.IsPlayer1Turn ? "J1" : "J2";
				
				battleView.ShowRoundHeader(currentPlayerName, playerNumber);
				
				var actionOrder = CalculateActionOrder(currentTeam);
				
				while (battleState.FullTurns > 0 && !IsBattleOver(battleState))
				{
					battleView.ShowBattlefield(battleState, player1Name, player2Name);
					battleView.ShowTurnCounters(battleState);
					
					battleView.ShowActionOrderBySpeed(actionOrder);
					
					if (actionOrder.Count == 0)
						break;
						
					var currentUnit = actionOrder[0];
					
					if (ProcessUnitAction(currentUnit, battleState, player1Name, player2Name))
					{
						return;
					}
					
					ConsumeTurn(battleState);
					
					if (IsBattleOver(battleState))
					{
						AnnounceWinner(battleState, player1Name, player2Name);
						return;
					}
					
					actionOrder.RemoveAt(0);
					if (currentTeam.AliveUnits.Contains(currentUnit))
					{
						actionOrder.Add(currentUnit);
					}
				}
				
				battleState.IsPlayer1Turn = !battleState.IsPlayer1Turn;
				currentTeam = battleState.IsPlayer1Turn ? battleState.Team1 : battleState.Team2;
				battleState.FullTurns = CalculateNextTurnCount(currentTeam);
				battleState.BlinkingTurns = 0;
			}
		}
		
		private List<UnitInstance> CalculateActionOrder(TeamState team)
		{
			return team.AliveUnits.OrderByDescending(u => u.Spd).ToList();
		}
		
		private int CalculateNextTurnCount(TeamState team)
		{
			return team.AliveUnits.Count();
		}
		
		private bool ProcessUnitAction(UnitInstance actingUnit, BattleState battleState, string player1Name, string player2Name)
		{
			bool actionCompleted = false;
			
			while (!actionCompleted)
			{
				var actions = GetAvailableActions(actingUnit);
				battleView.ShowActionMenu(actingUnit, actions);
				
				var choice = battleView.GetActionChoice(actions.Count);
				if (choice == -1) continue;
					
				var selectedAction = actions[choice - 1];
				
				switch (selectedAction)
				{
					case "Atacar":
						actionCompleted = ExecutePhysicalAttack(actingUnit, battleState);
						break;
					case "Disparar":
						actionCompleted = ExecuteGunAttack(actingUnit, battleState);
						break;
					case "Usar Habilidad":
						if (ProcessUseSkill(actingUnit, battleState))
						{
							actionCompleted = true;
						}
						break;
					case "Invocar":
						actionCompleted = true;
						break;
					case "Pasar Turno":
						actionCompleted = true;
						break;
					case "Rendirse":
						return ProcessSurrender(battleState, player1Name, player2Name);
					default:
						break;
				}
			}
			
			return false;
		}

		private bool ExecutePhysicalAttack(UnitInstance attacker, BattleState battleState)
		{
			return ProcessAttack(attacker, battleState, isGunAttack: false);
		}

		private bool ExecuteGunAttack(UnitInstance attacker, BattleState battleState)
		{
			return ProcessAttack(attacker, battleState, isGunAttack: true);
		}
		
		private List<string> GetAvailableActions(UnitInstance unit)
		{
			if (unit.IsSamurai)
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
			else
			{
				return new List<string>
				{
					"Atacar",
					"Usar Habilidad",
					"Invocar",
					"Pasar Turno"
				};
			}
		}
		
		private bool ProcessAttack(UnitInstance attacker, BattleState battleState, bool isGunAttack)
		{
			var enemyTeam = battleState.IsPlayer1Turn ? battleState.Team2 : battleState.Team1;
			var targets = GetTargets(enemyTeam);
			
			if (!targets.Any())
				return false;
				
			battleView.ShowTargetSelection(attacker, targets);
			
			var choice = battleView.GetTargetChoice(targets.Count);
			if (choice == -1 || choice == targets.Count + 1)
				return false;
				
			var target = targets[choice - 1];
			
			int damage = isGunAttack 
				? damageCalculator.CalculateGunDamage(attacker.Skl)
				: damageCalculator.CalculatePhysicalDamage(attacker.Str);
			
			target.HP = Math.Max(0, target.HP - damage);
			
			battleView.ShowAttackResult(attacker, target, damage, isGunAttack);
			
			return true;
		}
		
		private bool ProcessUseSkill(UnitInstance unit, BattleState battleState)
		{
			var availableSkills = GetAvailableSkills(unit);
			
			battleView.ShowSkillSelection(unit, availableSkills);
			
			var choice = battleView.GetSkillChoice(availableSkills.Count);
			if (choice == -1 || choice == availableSkills.Count + 1)
			{
				return false;
			}
			
			return true;
		}
		
		private List<Skill> GetAvailableSkills(UnitInstance unit)
		{
			var availableSkills = new List<Skill>();
			
			foreach (var skillName in unit.Skills)
			{
				if (skillData.TryGetValue(skillName, out var skill) && skill.Cost <= unit.MP)
				{
					availableSkills.Add(skill);
				}
			}
			
			return availableSkills;
		}
		
		private bool ProcessSurrender(BattleState battleState, string player1Name, string player2Name)
		{
			var playerName = battleState.IsPlayer1Turn ? player1Name : player2Name;
			var playerNumber = battleState.IsPlayer1Turn ? "J1" : "J2";
			var winnerName = battleState.IsPlayer1Turn ? player2Name : player1Name;
			var winnerNumber = battleState.IsPlayer1Turn ? "J2" : "J1";
			
			battleView.ShowSurrender(playerName, playerNumber, winnerName, winnerNumber);
			
			return true;
		}
		
		private List<UnitInstance> GetTargets(TeamState enemyTeam)
		{
			return enemyTeam.AliveUnits.ToList();
		}
		
		private void ConsumeTurn(BattleState battleState)
		{
			battleView.ShowTurnConsumption();
			
			battleState.FullTurns = Math.Max(0, battleState.FullTurns - 1);
		}
		
		private bool IsBattleOver(BattleState battleState)
		{
			return !battleState.Team1.AliveUnits.Any() || !battleState.Team2.AliveUnits.Any();
		}
		
		private void AnnounceWinner(BattleState battleState, string player1Name, string player2Name)
		{
			if (!battleState.Team1.AliveUnits.Any())
			{
				battleView.ShowWinner(player2Name, "J2");
			}
			else
			{
				battleView.ShowWinner(player1Name, "J1");
			}
		}
	}
}
