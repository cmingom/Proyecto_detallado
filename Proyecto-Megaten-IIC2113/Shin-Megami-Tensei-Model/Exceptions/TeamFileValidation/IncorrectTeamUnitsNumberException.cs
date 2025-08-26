namespace Shin_Megami_Tensei_Model.Exceptions.TeamFileValidation;

public class IncorrectTeamUnitsNumberException : TeamFileValidationException
{
    public IncorrectTeamUnitsNumberException() : base("Team has more than 8 units")
    {
    }
}
