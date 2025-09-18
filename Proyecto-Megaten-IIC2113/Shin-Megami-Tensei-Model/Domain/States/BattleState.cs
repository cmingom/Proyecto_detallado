namespace Shin_Megami_Tensei_Model.Domain.States
{
    public class BattleState
    {
        private const int INITIAL_BLINKING_TURNS = 0;
        private const bool INITIAL_PLAYER_1_TURN = true;
        
        private int fullTurns;
        private int blinkingTurns;
        private bool isPlayer1Turn;
        private bool turnConsumptionMessageShown;
        
        public TeamState Team1 { get; }
        public TeamState Team2 { get; }
        public int FullTurns => fullTurns;
        public int BlinkingTurns => blinkingTurns;
        public bool IsPlayer1Turn => isPlayer1Turn;

        public BattleState(TeamState team1, TeamState team2)
        {
            Team1 = team1;
            Team2 = team2;
            fullTurns = team1.AliveUnits.Count();
            blinkingTurns = INITIAL_BLINKING_TURNS;
            isPlayer1Turn = INITIAL_PLAYER_1_TURN;
        }

        public void ConsumeTurn()
        {
            if (fullTurns > 0)
            {
                fullTurns--;
            }
        }

        public void SwitchPlayer()
        {
            isPlayer1Turn = !isPlayer1Turn;
        }

        public void SetFullTurns(int turns)
        {
            fullTurns = turns;
        }

        public void ResetBlinkingTurns()
        {
            blinkingTurns = INITIAL_BLINKING_TURNS;
        }

        public void ConsumeBlinkingTurn()
        {
            if (blinkingTurns > 0)
            {
                blinkingTurns--;
            }
        }

        public void GrantBlinkingTurn()
        {
            blinkingTurns++;
        }

        public void MarkTurnConsumptionMessageShown()
        {
            turnConsumptionMessageShown = true;
        }

        public bool IsTurnConsumptionMessageShown()
        {
            return turnConsumptionMessageShown;
        }

        public void ResetTurnConsumptionMessageFlag()
        {
            turnConsumptionMessageShown = false;
        }

        public TeamState GetCurrentTeam()
        {
            return isPlayer1Turn ? Team1 : Team2;
        }

        public TeamState GetOpponentTeam()
        {
            return isPlayer1Turn ? Team2 : Team1;
        }
    }
}