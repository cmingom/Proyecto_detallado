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

        public List<UnitInstanceContext> GetActionOrder(TeamState team)
        {
            return team.AliveUnits.OrderByDescending(u => u.Spd).ToList();
        }

        public int GetNextTurnCount(TeamState team)
        {
            return team.AliveUnits.Count();
        }

        public void ConsumeTurn(BattleState battleState)
        {
            if (battleState.IsTurnConsumptionMessageShown())
            {
                return;
            }

            int fullTurnsConsumed = 0;
            int blinkingTurnsConsumed = 0;

            if (battleState.BlinkingTurns > 0)
            {
                battleState.ConsumeBlinkingTurn();
                blinkingTurnsConsumed = 1;
            }
            else
            {
                DecreaseFullTurns(battleState);
                fullTurnsConsumed = 1;
            }

            battleState.MarkTurnConsumptionMessageShown();
            battleView.ShowTurnConsumptionWithBlinking(fullTurnsConsumed, blinkingTurnsConsumed, 0);
        }

        private void DecreaseFullTurns(BattleState battleState)
        {
            var newTurnCount = GetNewTurnCount(battleState.FullTurns);
            battleState.SetFullTurns(newTurnCount);
        }

        private int GetNewTurnCount(int currentTurns)
        {
            return Math.Max(MINIMUM_TURNS, currentTurns - TURN_DECREMENT);
        }

        public bool IsBattleOver(BattleState battleState)
        {
            return battleState.IsBattleFinished || IsTeamDefeated(battleState.Team1) || IsTeamDefeated(battleState.Team2);
        }

        private bool IsTeamDefeated(TeamState team)
        {
            return !team.AliveUnits.Any();
        }

        public string GetWinner(BattleState battleState, string player1Name, string player2Name)
        {
            if (battleState.WinnerSide != null)
            {
                return battleState.WinnerSide == PLAYER_1_LABEL ? battleState.WinnerSamuraiName ?? player1Name : battleState.WinnerSamuraiName ?? player2Name;
            }

            var winnerName = IsTeamOneDefeated(battleState) ? player2Name : player1Name;
            var winnerSide = IsTeamOneDefeated(battleState) ? PLAYER_2_LABEL : PLAYER_1_LABEL;
            battleState.MarkWinner(winnerSide, winnerName);
            return winnerName;
        }

        private bool IsTeamOneDefeated(BattleState battleState)
        {
            return !battleState.Team1.AliveUnits.Any();
        }

        public string GetWinnerNumber(BattleState battleState)
        {
            if (battleState.WinnerSide != null)
            {
                return battleState.WinnerSide;
            }

            var winnerSide = IsTeamOneDefeated(battleState) ? PLAYER_2_LABEL : PLAYER_1_LABEL;
            return winnerSide;
        }
    }
}
