using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.CombatSystem.Core;

namespace Shin_Megami_Tensei
{
    public class BattleContext
    {
        public BattleState BattleState { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }

        public bool HasRemainingTurns()
        {
            var hasFullTurns = BattleState?.FullTurns > 0;
            var hasBlinkingTurns = BattleState?.BlinkingTurns > 0;
            var result = hasFullTurns || hasBlinkingTurns;
            return result;
        }

        public bool HasBattleEnded(CombatManager combatManager)
        {
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
