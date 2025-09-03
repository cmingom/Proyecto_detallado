using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class SurrenderProcessor
    {
        private const string PLAYER_1_LABEL = "J1";
        private const string PLAYER_2_LABEL = "J2";
        
        private readonly IBattleView battleView;

        public SurrenderProcessor(IBattleView battleView)
        {
            this.battleView = battleView;
        }

        // verbo auxiliar (revisar en caso de que caiga) le saque el return y antes era bool
        public void ProcessSurrender(BattleState battleState, string player1Name, string player2Name)
        {
            var surrenderInfo = CreateSurrenderInfo(battleState, player1Name, player2Name);
            ShowSurrenderInfo(surrenderInfo);
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
            return battleState.IsPlayer1Turn ? PLAYER_1_LABEL : PLAYER_2_LABEL;
        }

        private string GetSurrenderWinnerName(BattleState battleState, string player1Name, string player2Name)
        {
            return battleState.IsPlayer1Turn ? player2Name : player1Name;
        }

        private string GetSurrenderWinnerNumber(BattleState battleState)
        {
            return battleState.IsPlayer1Turn ? PLAYER_2_LABEL : PLAYER_1_LABEL;
        }

        private void ShowSurrenderInfo(SurrenderInfo surrenderInfo)
        {
            // recibe 4
            battleView.ShowSurrender(surrenderInfo.SurrenderingPlayer.Name, surrenderInfo.SurrenderingPlayer.Number, 
                                   surrenderInfo.Winner.Name, surrenderInfo.Winner.Number);
        }
    }
}
