using Shin_Megami_Tensei_Model.Domain.States;

namespace Shin_Megami_Tensei
{
    public class TurnContext
    {
        public BattleState BattleState { get; set; }
        public TeamState CurrentTeam { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }

        public TurnContext(BattleState battleState, TeamState currentTeam, string player1Name, string player2Name)
        {
            BattleState = battleState;
            CurrentTeam = currentTeam;
            Player1Name = player1Name;
            Player2Name = player2Name;
        }
    }
}
