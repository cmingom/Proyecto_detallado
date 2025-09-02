using Shin_Megami_Tensei_Model.CombatSystem.Contexts;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class SurrenderInfo
    {
        public PlayerInfo SurrenderingPlayer { get; }
        public PlayerInfo Winner { get; }

        public SurrenderInfo(PlayerInfo surrenderingPlayer, PlayerInfo winner)
        {
            SurrenderingPlayer = surrenderingPlayer;
            Winner = winner;
        }
    }
}
