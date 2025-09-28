using System;
﻿namespace Shin_Megami_Tensei_Model.CombatSystem.Contexts
{
    public sealed class SurrenderInfo
    {
        public SurrenderInfo(PlayerInfo surrenderingPlayer, PlayerInfo winner)
        {
            SurrenderingPlayer = surrenderingPlayer ?? throw new ArgumentNullException(nameof(surrenderingPlayer));
            Winner = winner ?? throw new ArgumentNullException(nameof(winner));
        }

        public PlayerInfo SurrenderingPlayer { get; }
        public PlayerInfo Winner { get; }
    }
}
