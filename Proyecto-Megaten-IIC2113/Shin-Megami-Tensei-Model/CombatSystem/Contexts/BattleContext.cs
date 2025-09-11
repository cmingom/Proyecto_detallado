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
            return BattleState?.FullTurns > 0;
        }

        public bool IsBattleOver(CombatManager combatManager)
        {
            return combatManager.IsBattleOver(BattleState);
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
