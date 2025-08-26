namespace Shin_Megami_Tensei_Model.Exceptions.TeamFileValidation;

public class TeamUnitsAreNotUniqueException : TeamFileValidationException
{
    public TeamUnitsAreNotUniqueException() : base("Team has duplicate units")
    {
    }
}
