using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class ActionContext
    {
        public UnitInstance ActingUnit { get; }
        public BattleState BattleState { get; }
        public string Player1Name { get; }
        public string Player2Name { get; }

        public ActionContext(UnitInstance actingUnit, BattleState battleState, string player1Name, string player2Name)
        {
            ActingUnit = actingUnit;
            BattleState = battleState;
            Player1Name = player1Name;
            Player2Name = player2Name;
        }
    }
}
