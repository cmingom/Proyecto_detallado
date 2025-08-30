using System;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Core;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class SurrenderHandler
    {
        private readonly IBattleView battleView;

        public SurrenderHandler(IBattleView battleView)
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
    }
}
