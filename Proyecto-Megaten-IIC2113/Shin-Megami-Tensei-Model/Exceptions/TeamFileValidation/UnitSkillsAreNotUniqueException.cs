namespace Shin_Megami_Tensei_Model.Exceptions.TeamFileValidation;

public class UnitSkillsAreNotUniqueException : TeamFileValidationException
{
    public UnitSkillsAreNotUniqueException() : base("Samurai has duplicate skills")
    {
    }
}
