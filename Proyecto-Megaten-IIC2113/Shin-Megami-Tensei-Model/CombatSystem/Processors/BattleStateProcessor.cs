using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class BattleStateProcessor
    {
        private const int MINIMUM_TURNS = 0;
        private const int TURN_DECREMENT = 1;
        private const string PLAYER_1_LABEL = "J1";
        private const string PLAYER_2_LABEL = "J2";
        
        private readonly IBattleView battleView;

        public BattleStateProcessor(IBattleView battleView)
        {
            this.battleView = battleView;
        }

        public List<UnitInstance> CalculateActionOrder(TeamState team)
        {
            return team.AliveUnits.OrderByDescending(u => u.Spd).ToList();
        }

        public int CalculateNextTurnCount(TeamState team)
        {
            return team.AliveUnits.Count();
        }

        public void ConsumeTurn(BattleState battleState)
        {
            ShowTurnConsumption();
            DecreaseFullTurns(battleState);
        }

        private void ShowTurnConsumption()
        {
            battleView.ShowTurnConsumption();
        }

        private void DecreaseFullTurns(BattleState battleState)
        {
            var newTurnCount = CalculateNewTurnCount(battleState.FullTurns);
            battleState.FullTurns = newTurnCount;
        }

        private int CalculateNewTurnCount(int currentTurns)
        {
            return Math.Max(MINIMUM_TURNS, currentTurns - TURN_DECREMENT);
        }

        public bool IsBattleOver(BattleState battleState)
        {
            return IsTeamDefeated(battleState.Team1) || IsTeamDefeated(battleState.Team2);
        }

        private bool IsTeamDefeated(TeamState team)
        {
            return !team.AliveUnits.Any();
        }

        public string GetWinner(BattleState battleState, string player1Name, string player2Name)
        {
            return IsTeam1Defeated(battleState) ? player2Name : player1Name;
        }

        private bool IsTeam1Defeated(BattleState battleState)
        {
            return !battleState.Team1.AliveUnits.Any();
        }

        public string GetWinnerNumber(BattleState battleState)
        {
            return IsTeam1Defeated(battleState) ? PLAYER_2_LABEL : PLAYER_1_LABEL;
        }
    }
}
