namespace Shin_Megami_Tensei_Model.Exceptions.TeamFileValidation;

public class TeamHasMultipleSamuraisException : TeamFileValidationException
{
    public TeamHasMultipleSamuraisException() : base("Team has more than one samurai")
    {
    }
}
