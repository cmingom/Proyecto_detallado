namespace Shin_Megami_Tensei_Model.Exceptions.TeamFileValidation;

public class TeamHasNoSamuraiException : TeamFileValidationException
{
    public TeamHasNoSamuraiException() : base("Team has no samurai")
    {
    }
}
