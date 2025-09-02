using System;
using System.Collections.Generic;
using Shin_Megami_Tensei_View.ConsoleLib;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Core;
using Shin_Megami_Tensei_View;

namespace Shin_Megami_Tensei
{
    public class TurnManager
    {
        private const string PLAYER_1_LABEL = "J1";
        private const string PLAYER_2_LABEL = "J2";

        private readonly BattleView battleView;
        private readonly CombatManager combatService;
        private readonly ActionProcessor actionProcessor;

        public TurnManager(BattleView battleView, CombatManager combatService)
        {
            this.battleView = battleView;
            this.combatService = combatService;
            this.actionProcessor = new ActionProcessor(battleView, combatService);
        }

        public bool IsPlayerTurnComplete(BattleState battleState, string player1Name, string player2Name)
        {
            var currentTeam = GetCurrentTeam(battleState);
            ShowPlayerTurnHeader(battleState, player1Name, player2Name);
            var actionOrder = combatService.CalculateActionOrder(currentTeam);
            var battleParams = new BattleContext { BattleState = battleState, Player1Name = player1Name, Player2Name = player2Name };
            var shouldEndBattle = actionProcessor.ProcessActionOrder(battleParams, actionOrder, currentTeam);
            HandlePlayerTurnEnd(battleState, currentTeam, shouldEndBattle);
            return shouldEndBattle;
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

        private void HandlePlayerTurnEnd(BattleState battleState, TeamState currentTeam, bool shouldEndBattle)
        {
            if (!shouldEndBattle)
            {
                SwitchPlayerTurn(battleState, currentTeam);
            }
        }

        private void SwitchPlayerTurn(BattleState battleState, TeamState currentTeam)
        {
            battleState.IsPlayer1Turn = !battleState.IsPlayer1Turn;
            var newCurrentTeam = GetCurrentTeam(battleState);
            UpdateTurnCounters(battleState, newCurrentTeam);
        }

        private const int INITIAL_BLINKING_TURNS = 0;

        private void UpdateTurnCounters(BattleState battleState, TeamState newCurrentTeam)
        {
            battleState.FullTurns = combatService.CalculateNextTurnCount(newCurrentTeam);
            battleState.BlinkingTurns = INITIAL_BLINKING_TURNS;
        }
    }
}
