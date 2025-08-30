using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class SurrenderExecutor : IActionHandler
    {
        private readonly IBattleView battleView;

        public SurrenderExecutor(IBattleView battleView)
        {
            this.battleView = battleView;
        }

        public bool Execute(UnitInstance actingUnit, BattleState battleState, string player1Name, string player2Name)
        {
            ProcessSurrender(battleState, player1Name, player2Name);
            return true;
        }

        private void ProcessSurrender(BattleState battleState, string player1Name, string player2Name)
        {
            var surrenderInfo = CreateSurrenderInfo(battleState, player1Name, player2Name);
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
