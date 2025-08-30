using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public interface IActionHandler
    {
        bool Execute(UnitInstance actingUnit, BattleState battleState, string player1Name, string player2Name);
    }
}
