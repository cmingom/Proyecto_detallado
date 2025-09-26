using System;

namespace Shin_Megami_Tensei_View.Exceptions
{
    public class InvalidInputRequestException : ApplicationException
    {
        public InvalidInputRequestException(string message) : base(message)
        {
        }

        public InvalidInputRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
