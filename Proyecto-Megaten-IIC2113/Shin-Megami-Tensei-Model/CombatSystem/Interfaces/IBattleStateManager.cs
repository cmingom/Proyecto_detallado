using Shin_Megami_Tensei_Model.Domain.States;

namespace Shin_Megami_Tensei_Model.CombatSystem.Interfaces
{
    public interface IBattleStateManager
    {
        bool IsBattleOver(BattleState battleState);
        string GetWinner(BattleState battleState, string player1Name, string player2Name);
        string GetWinnerNumber(BattleState battleState);
    }
}
