using System;
using Shin_Megami_Tensei_Model.Domain.States;

namespace Shin_Megami_Tensei_Model.CombatSystem.Contexts
{
    public sealed class TurnContext
    {
        public TurnContext(BattleState battleState, TeamState currentTeam, string player1Name, string player2Name)
        {
            BattleState = battleState ?? throw new ArgumentNullException(nameof(battleState));
            CurrentTeam = currentTeam ?? throw new ArgumentNullException(nameof(currentTeam));
            Player1Name = player1Name ?? throw new ArgumentNullException(nameof(player1Name));
            Player2Name = player2Name ?? throw new ArgumentNullException(nameof(player2Name));
        }

        public BattleState BattleState { get; }
        public TeamState CurrentTeam { get; }
        public string Player1Name { get; }
        public string Player2Name { get; }
    }
}
