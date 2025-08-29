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
		private readonly CombatService combatService;
		
		public BattleEngine(View view, Dictionary<string, Skill> skillData)
		{
			this.battleView = new BattleView(view);
			this.combatService = new CombatService(skillData, this.battleView);
		}
		
		public void StartBattle(BattleState battleState, string player1Name, string player2Name)
		{
			while (!combatService.IsBattleOver(battleState))
			{
				var currentTeam = battleState.IsPlayer1Turn ? battleState.Team1 : battleState.Team2;
				var currentPlayerName = battleState.IsPlayer1Turn ? player1Name : player2Name;
				var playerNumber = battleState.IsPlayer1Turn ? "J1" : "J2";
				
				battleView.ShowRoundHeader(currentPlayerName, playerNumber);
				
				var actionOrder = combatService.CalculateActionOrder(currentTeam);
				
				while (battleState.FullTurns > 0 && !combatService.IsBattleOver(battleState))
				{
					battleView.ShowBattlefield(battleState, player1Name, player2Name);
					battleView.ShowTurnCounters(battleState);
					
					battleView.ShowActionOrderBySpeed(actionOrder);
					
					if (actionOrder.Count == 0)
						break;
						
					var currentUnit = actionOrder[0];
					
					if (combatService.ProcessUnitAction(currentUnit, battleState, player1Name, player2Name))
					{
						return;
					}
					
					combatService.ConsumeTurn(battleState);
					
					if (combatService.IsBattleOver(battleState))
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
				battleState.FullTurns = combatService.CalculateNextTurnCount(currentTeam);
				battleState.BlinkingTurns = 0;
			}
		}

		
		
		private void AnnounceWinner(BattleState battleState, string player1Name, string player2Name)
		{
			var winnerName = combatService.GetWinner(battleState, player1Name, player2Name);
			var winnerNumber = combatService.GetWinnerNumber(battleState);
			battleView.ShowWinner(winnerName, winnerNumber);
		}
	}
}
