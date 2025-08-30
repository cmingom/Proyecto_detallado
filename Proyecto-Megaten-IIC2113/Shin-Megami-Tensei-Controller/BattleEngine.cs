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
		private const int FIRST_UNIT_INDEX = 0;
		private const int ZERO_TURNS = 0;
		private const int EMPTY_LIST_COUNT = 0;
		private const string PLAYER_1_LABEL = "J1";
		private const string PLAYER_2_LABEL = "J2";

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
				if (IsPlayerTurnComplete(battleState, player1Name, player2Name))
				{
					return;
				}
			}
		}

		private bool IsPlayerTurnComplete(BattleState battleState, string player1Name, string player2Name)
		{
			var currentTeam = GetCurrentTeam(battleState);
			ShowPlayerTurnHeader(battleState, player1Name, player2Name);
			var actionOrder = combatService.CalculateActionOrder(currentTeam);
			var battleParams = new BattleParameters { BattleState = battleState, Player1Name = player1Name, Player2Name = player2Name };
			var shouldEndBattle = ProcessActionOrder(battleParams, actionOrder, currentTeam);
			HandlePlayerTurnEnd(battleState, currentTeam, shouldEndBattle);
			return shouldEndBattle;
		}

		private bool ProcessActionOrder(BattleParameters battleParams, List<UnitInstance> actionOrder, TeamState currentTeam)
		{
			while (ShouldContinueProcessingActions(battleParams))
			{
				if (ProcessSingleActionIteration(battleParams, actionOrder, currentTeam))
					return true;
			}
			return false;
		}

		private bool ProcessSingleActionIteration(BattleParameters battleParams, List<UnitInstance> actionOrder, TeamState currentTeam)
		{
			ShowBattleStatus(battleParams, actionOrder);
			if (IsActionOrderEmpty(actionOrder)) 
				return false;
			
			var currentUnit = GetCurrentUnit(actionOrder);
			if (ProcessSingleUnitAction(currentUnit, battleParams)) 
				return true;
			
			ProcessUnitTurnEnd(actionOrder, currentTeam, currentUnit);
			return false;
		}

		private bool ProcessSingleUnitAction(UnitInstance currentUnit, BattleParameters battleParams)
		{
			if (IsUnitActionSuccessful(currentUnit, battleParams))
				return true;
			
			combatService.ConsumeTurn(battleParams.BattleState);
			
			return CheckAndHandleBattleEnd(battleParams);
		}

		private void HandlePlayerTurnEnd(BattleState battleState, TeamState currentTeam, bool shouldEndBattle)
		{
			if (!shouldEndBattle)
			{
				SwitchPlayerTurn(battleState, currentTeam);
			}
		}


		private TeamState GetCurrentTeam(BattleState battleState)
		{
			return battleState.IsPlayer1Turn ? battleState.Team1 : battleState.Team2;
		}

		private string GetCurrentPlayerName(BattleState battleState, string player1Name, string player2Name)
		{
			return battleState.IsPlayer1Turn ? player1Name : player2Name;
		}

		private string GetCurrentPlayerNumber(BattleState battleState)
		{
			return battleState.IsPlayer1Turn ? PLAYER_1_LABEL : PLAYER_2_LABEL;
		}

		private void ShowPlayerTurnHeader(BattleState battleState, string player1Name, string player2Name)
		{
			var currentPlayerName = GetCurrentPlayerName(battleState, player1Name, player2Name);
			var playerNumber = GetCurrentPlayerNumber(battleState);
			battleView.ShowRoundHeader(currentPlayerName, playerNumber);
		}

		private void ShowBattleStatus(BattleParameters battleParams, List<UnitInstance> actionOrder)
		{
			battleView.ShowBattlefield(battleParams.BattleState, battleParams.Player1Name, battleParams.Player2Name);
			battleView.ShowTurnCounters(battleParams.BattleState);
			battleView.ShowActionOrderBySpeed(actionOrder);
		}

		private bool IsUnitActionSuccessful(UnitInstance currentUnit, BattleParameters battleParams)
		{
			return combatService.ProcessUnitAction(currentUnit, battleParams.BattleState, battleParams.Player1Name, battleParams.Player2Name);
		}

		private bool CheckAndHandleBattleEnd(BattleParameters battleParams)
		{
			if (combatService.IsBattleOver(battleParams.BattleState))
			{
				AnnounceWinner(battleParams.BattleState, battleParams.Player1Name, battleParams.Player2Name);
				return true;
			}
			return false;
		}

		private void ProcessUnitTurnEnd(List<UnitInstance> actionOrder, TeamState currentTeam, UnitInstance currentUnit)
		{
			actionOrder.RemoveAt(FIRST_UNIT_INDEX);
			if (currentTeam.AliveUnits.Contains(currentUnit))
			{
				actionOrder.Add(currentUnit);
			}
		}

		private bool ShouldContinueProcessingActions(BattleParameters battleParams)
		{
			return battleParams.BattleState.FullTurns > ZERO_TURNS && !combatService.IsBattleOver(battleParams.BattleState);
		}

		private bool IsActionOrderEmpty(List<UnitInstance> actionOrder)
		{
			return actionOrder.Count == EMPTY_LIST_COUNT;
		}

		private UnitInstance GetCurrentUnit(List<UnitInstance> actionOrder)
		{
			return actionOrder[FIRST_UNIT_INDEX];
		}

		private void SwitchPlayerTurn(BattleState battleState, TeamState currentTeam)
		{
			battleState.IsPlayer1Turn = !battleState.IsPlayer1Turn;
			var newCurrentTeam = GetCurrentTeam(battleState);
			UpdateTurnCounters(battleState, newCurrentTeam);
		}

		private void UpdateTurnCounters(BattleState battleState, TeamState newCurrentTeam)
		{
			battleState.FullTurns = combatService.CalculateNextTurnCount(newCurrentTeam);
			battleState.BlinkingTurns = ZERO_TURNS;
		}
		
		private void AnnounceWinner(BattleState battleState, string player1Name, string player2Name)
		{
			var winnerName = combatService.GetWinner(battleState, player1Name, player2Name);
			var winnerNumber = combatService.GetWinnerNumber(battleState);
			battleView.ShowWinner(winnerName, winnerNumber);
		}

		private class BattleParameters
		{
			public BattleState BattleState { get; set; }
			public string Player1Name { get; set; }
			public string Player2Name { get; set; }
		}
	}
}
