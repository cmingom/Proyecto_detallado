using System;
using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Core;

namespace Shin_Megami_Tensei
{
	public class BattleEngine
	{
		private readonly View view;
		private readonly DamageCalculator damageCalculator;
		private readonly Dictionary<string, Skill> skillData;
		
		public BattleEngine(View view, Dictionary<string, Skill> skillData)
		{
			this.view = view;
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
				
				ShowRoundHeader(currentPlayerName, playerNumber);
				
				var actionOrder = CalculateActionOrder(currentTeam);
				
				while (battleState.FullTurns > 0 && !IsBattleOver(battleState))
				{
					ShowBattlefield(battleState, player1Name, player2Name);
					ShowTurnCounters(battleState);
					
					ShowActionOrderBySpeed(actionOrder);
					
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
		
		private void ShowRoundHeader(string playerName, string playerNumber)
		{
			view.WriteLine("----------------------------------------");
			view.WriteLine($"Ronda de {playerName} ({playerNumber})");
		}
		
		private void ShowBattlefield(BattleState battleState, string player1Name, string player2Name)
		{
			view.WriteLine("----------------------------------------");
			ShowTeamStatus(battleState.Team1, player1Name, "J1");
			ShowTeamStatus(battleState.Team2, player2Name, "J2");
		}
		
		private void ShowTeamStatus(TeamState team, string playerName, string playerNumber)
		{
			view.WriteLine($"Equipo de {playerName} ({playerNumber})");
			
			char[] positions = { 'A', 'B', 'C', 'D' };
			
			for (int i = 0; i < 4; i++)
			{
				var unit = team.Units[i];
				if (unit != null)
				{
								if (unit.IsSamurai || unit.HP > 0)
					{
						view.WriteLine($"{positions[i]}-{unit.Name} HP:{unit.HP}/{unit.MaxHP} MP:{unit.MP}/{unit.MaxMP}");
					}
					else
					{
						view.WriteLine($"{positions[i]}-");
					}
				}
				else
				{
					view.WriteLine($"{positions[i]}-");
				}
			}
		}
		
		private void ShowTurnCounters(BattleState battleState)
		{
			view.WriteLine("----------------------------------------");
			view.WriteLine($"Full Turns: {battleState.FullTurns}");
			view.WriteLine($"Blinking Turns: {battleState.BlinkingTurns}");
		}
		
		private void ShowActionOrderBySpeed(List<UnitInstance> actionOrder)
		{
			view.WriteLine("----------------------------------------");
			view.WriteLine("Orden:");
			
			for (int i = 0; i < actionOrder.Count; i++)
			{
				view.WriteLine($"{i + 1}-{actionOrder[i].Name}");
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
				view.WriteLine("----------------------------------------");
				view.WriteLine($"Seleccione una acción para {actingUnit.Name}");
				
				var actions = GetAvailableActions(actingUnit);
				for (int i = 0; i < actions.Count; i++)
				{
					view.WriteLine($"{i + 1}: {actions[i]}");
				}
				
				var input = view.ReadLine();
				if (!int.TryParse(input, out int choice) || choice < 1 || choice > actions.Count)
					continue;
					
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
				
			view.WriteLine("----------------------------------------");
			view.WriteLine($"Seleccione un objetivo para {attacker.Name}");
			
			for (int i = 0; i < targets.Count; i++)
			{
				var currentTarget = targets[i];
				view.WriteLine($"{i + 1}-{currentTarget.Name} HP:{currentTarget.HP}/{currentTarget.MaxHP} MP:{currentTarget.MP}/{currentTarget.MaxMP}");
			}
			view.WriteLine($"{targets.Count + 1}-Cancelar");
			
			var input = view.ReadLine();
			if (!int.TryParse(input, out int choice) || choice < 1 || choice > targets.Count + 1)
				return false;
				
			if (choice == targets.Count + 1)
				return false;
				
			var target = targets[choice - 1];
			
			view.WriteLine("----------------------------------------");
			
			int damage = isGunAttack 
				? damageCalculator.CalculateGunDamage(attacker.Skl)
				: damageCalculator.CalculatePhysicalDamage(attacker.Str);
			
			string attackType = isGunAttack ? "dispara a" : "ataca a";
			view.WriteLine($"{attacker.Name} {attackType} {target.Name}");
			view.WriteLine($"{target.Name} recibe {damage} de daño");
			
			target.HP = Math.Max(0, target.HP - damage);
			view.WriteLine($"{target.Name} termina con HP:{target.HP}/{target.MaxHP}");
			
			return true;
		}
		
		private bool ProcessUseSkill(UnitInstance unit, BattleState battleState)
		{
			var availableSkills = GetAvailableSkills(unit);
			
			view.WriteLine("----------------------------------------");
			view.WriteLine($"Seleccione una habilidad para que {unit.Name} use");
			
			for (int i = 0; i < availableSkills.Count; i++)
			{
				var skill = availableSkills[i];
				view.WriteLine($"{i + 1}-{skill.Name} MP:{skill.Cost}");
			}
			view.WriteLine($"{availableSkills.Count + 1}-Cancelar");
			
			var input = view.ReadLine();
			if (!int.TryParse(input, out int choice) || choice < 1 || choice > availableSkills.Count + 1)
				return false;
				
			if (choice == availableSkills.Count + 1)
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
			
			view.WriteLine("----------------------------------------");
			view.WriteLine($"{playerName} ({playerNumber}) se rinde");
			view.WriteLine("----------------------------------------");
			view.WriteLine($"Ganador: {winnerName} ({winnerNumber})");
			
			return true;
		}
		
		private List<UnitInstance> GetTargets(TeamState enemyTeam)
		{
			return enemyTeam.AliveUnits.ToList();
		}
		
		private void ConsumeTurn(BattleState battleState)
		{
			view.WriteLine("----------------------------------------");
			view.WriteLine("Se han consumido 1 Full Turn(s) y 0 Blinking Turn(s)");
			view.WriteLine("Se han obtenido 0 Blinking Turn(s)");
			
			battleState.FullTurns = Math.Max(0, battleState.FullTurns - 1);
		}
		
		private bool IsBattleOver(BattleState battleState)
		{
			return !battleState.Team1.AliveUnits.Any() || !battleState.Team2.AliveUnits.Any();
		}
		
		private void AnnounceWinner(BattleState battleState, string player1Name, string player2Name)
		{
			view.WriteLine("----------------------------------------");
			
			if (!battleState.Team1.AliveUnits.Any())
			{
				view.WriteLine($"Ganador: {player2Name} (J2)");
			}
			else
			{
				view.WriteLine($"Ganador: {player1Name} (J1)");
			}
		}
	}
}
