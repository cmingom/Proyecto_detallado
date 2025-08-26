namespace Shin_Megami_Tensei_Model.Exceptions.TeamFileValidation;

public class TeamFileValidationException : Exception
{
    public TeamFileValidationException()
    {
    }

    public TeamFileValidationException(string message) : base(message)
    {
    }

    public TeamFileValidationException(string message, Exception inner) : base(message, inner)
    {
    }
}
