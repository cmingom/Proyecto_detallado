using System;
using Shin_Megami_Tensei_Model.CombatSystem.Core;
using Shin_Megami_Tensei_Model.Domain.States;

namespace Shin_Megami_Tensei_Model.CombatSystem.Contexts
{
    public sealed class BattleContext
    {
        public BattleContext(BattleState battleState, string player1Name, string player2Name)
        {
            BattleState = battleState ?? throw new ArgumentNullException(nameof(battleState));
            Player1Name = player1Name ?? throw new ArgumentNullException(nameof(player1Name));
            Player2Name = player2Name ?? throw new ArgumentNullException(nameof(player2Name));
        }

        public BattleState BattleState { get; }
        public string Player1Name { get; }
        public string Player2Name { get; }

        public bool HasRemainingTurns()
        {
            return BattleState.FullTurns > 0 || BattleState.BlinkingTurns > 0;
        }

        public bool HasBattleEnded(CombatManager combatManager)
        {
            if (combatManager == null)
            {
                throw new ArgumentNullException(nameof(combatManager));
            }

            return combatManager.HasBattleEnded(BattleState);
        }

        public string GetCurrentPlayerName()
        {
            return BattleState.IsPlayer1Turn ? Player1Name : Player2Name;
        }

        public string GetCurrentPlayerNumber()
        {
            return BattleState.IsPlayer1Turn ? "J1" : "J2";
        }
    }
}
