using System;

namespace Shin_Megami_Tensei_Model.Domain.Exceptions
{
    public class GameEndedException : Exception
    {
        public GameEndedException() : base("El juego ha terminado")
        {
        }

        public GameEndedException(string message) : base(message)
        {
        }

        public GameEndedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

