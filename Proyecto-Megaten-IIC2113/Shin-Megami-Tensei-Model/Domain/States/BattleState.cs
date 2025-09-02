using System;
using System.Linq;

namespace Shin_Megami_Tensei_Model.Domain.States
{
    public class BattleState
    {
        private const int INITIAL_BLINKING_TURNS = 0;
        private const bool INITIAL_PLAYER_1_TURN = true;
        
        public TeamState Team1 { get; }
        public TeamState Team2 { get; }
        public int FullTurns { get; set; }
        public int BlinkingTurns { get; set; }
        public bool IsPlayer1Turn { get; set; }

        public BattleState(TeamState team1, TeamState team2)
        {
            Team1 = team1;
            Team2 = team2;
            FullTurns = team1.AliveUnits.Count();
            BlinkingTurns = INITIAL_BLINKING_TURNS;
            IsPlayer1Turn = INITIAL_PLAYER_1_TURN;
        }
    }
}