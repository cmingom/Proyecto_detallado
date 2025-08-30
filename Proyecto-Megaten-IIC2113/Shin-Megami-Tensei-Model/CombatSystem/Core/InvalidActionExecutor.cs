using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class InvalidActionExecutor : IActionExecutor
    {
        public bool Execute(UnitInstance actingUnit, BattleState battleState, string player1Name, string player2Name)
        {
            return false; // Acción inválida nunca se completa
        }
    }
}
