using System;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Core;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class SurrenderProcessor
    {
        private readonly IBattleView battleView;

        public SurrenderProcessor(IBattleView battleView)
        {
            this.battleView = battleView;
        }

        public bool ProcessSurrender(BattleState battleState, string player1Name, string player2Name)
        {
            var surrenderInfo = CreateSurrenderInfo(battleState, player1Name, player2Name);
            ShowSurrenderInfo(surrenderInfo);
            return true;
        }

        private void ShowSurrenderInfo(SurrenderInfo surrenderInfo)
        {
            battleView.ShowSurrender(surrenderInfo.SurrenderingPlayer.Name, surrenderInfo.SurrenderingPlayer.Number, 
                                   surrenderInfo.Winner.Name, surrenderInfo.Winner.Number);
        }

        private SurrenderInfo CreateSurrenderInfo(BattleState battleState, string player1Name, string player2Name)
        {
            var surrenderingPlayer = CreateSurrenderingPlayer(battleState, player1Name, player2Name);
            var winner = CreateWinner(battleState, player1Name, player2Name);
            return new SurrenderInfo(surrenderingPlayer, winner);
        }

        private PlayerInfo CreateSurrenderingPlayer(BattleState battleState, string player1Name, string player2Name)
        {
            var name = GetSurrenderingPlayerName(battleState, player1Name, player2Name);
            var number = GetSurrenderingPlayerNumber(battleState);
            return new PlayerInfo(name, number);
        }

        private PlayerInfo CreateWinner(BattleState battleState, string player1Name, string player2Name)
        {
            var name = GetSurrenderWinnerName(battleState, player1Name, player2Name);
            var number = GetSurrenderWinnerNumber(battleState);
            return new PlayerInfo(name, number);
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
    }
}
