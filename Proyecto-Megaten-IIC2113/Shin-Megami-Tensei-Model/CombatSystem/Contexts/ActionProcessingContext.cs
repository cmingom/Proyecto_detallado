using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Contexts
{
    public class ActionProcessingContext
    {
        public UnitInstanceContext ActingUnit { get; }
        public BattleState BattleState { get; }
        public string SelectedAction { get; }
        public string Player1Name { get; }
        public string Player2Name { get; }

        public ActionProcessingContext(UnitInstanceContext actingUnit, BattleState battleState, string selectedAction, string player1Name, string player2Name)
        {
            ActingUnit = actingUnit;
            BattleState = battleState;
            SelectedAction = selectedAction;
            Player1Name = player1Name;
            Player2Name = player2Name;
        }
    }
}
