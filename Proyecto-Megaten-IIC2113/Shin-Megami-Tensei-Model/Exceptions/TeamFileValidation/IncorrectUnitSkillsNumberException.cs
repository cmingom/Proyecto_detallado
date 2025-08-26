namespace Shin_Megami_Tensei_Model.Exceptions.TeamFileValidation;

public class IncorrectUnitSkillsNumberException : TeamFileValidationException
{
    public IncorrectUnitSkillsNumberException() : base("Samurai has more than 8 skills")
    {
    }
}
