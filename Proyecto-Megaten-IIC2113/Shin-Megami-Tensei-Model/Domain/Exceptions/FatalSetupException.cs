using System;

namespace Shin_Megami_Tensei_Model.Domain.Exceptions
{
    public class FatalSetupException : Exception
    {
        public FatalSetupException() : base("Error fatal en el setup del juego")
        {
        }

        public FatalSetupException(string message) : base(message)
        {
        }

        public FatalSetupException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

