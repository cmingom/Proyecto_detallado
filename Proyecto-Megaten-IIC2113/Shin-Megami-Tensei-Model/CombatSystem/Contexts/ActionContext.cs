using System;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.Domain.States;

namespace Shin_Megami_Tensei_Model.CombatSystem.Contexts
{
    public sealed class ActionContext
    {
        public ActionContext(UnitInstanceContext actingUnit, BattleState battleState, string player1Name, string player2Name)
        {
            ActingUnit = actingUnit ?? throw new ArgumentNullException(nameof(actingUnit));
            BattleState = battleState ?? throw new ArgumentNullException(nameof(battleState));
            Player1Name = player1Name ?? throw new ArgumentNullException(nameof(player1Name));
            Player2Name = player2Name ?? throw new ArgumentNullException(nameof(player2Name));
        }

        public UnitInstanceContext ActingUnit { get; }
        public BattleState BattleState { get; }
        public string Player1Name { get; }
        public string Player2Name { get; }
    }
}
