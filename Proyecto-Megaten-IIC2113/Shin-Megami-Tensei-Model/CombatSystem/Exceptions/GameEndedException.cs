using System;

namespace Shin_Megami_Tensei_Model.CombatSystem.Exceptions
{
    public class GameEndedException : Exception
    {
        public GameEndedException() : base("El juego ha terminado")
        {
        }
    }
}
