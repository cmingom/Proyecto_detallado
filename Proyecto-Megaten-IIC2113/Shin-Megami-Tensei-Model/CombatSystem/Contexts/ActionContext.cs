using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class ActionContext
    {
        public UnitInstanceContext ActingUnit { get; set; }
        public BattleState BattleState { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }

        public ActionContext(UnitInstanceContext actingUnit, BattleState battleState, string player1Name, string player2Name)
        {
            ActingUnit = actingUnit;
            BattleState = battleState;
            Player1Name = player1Name;
            Player2Name = player2Name;
        }
    }
}
