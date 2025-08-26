namespace Shin_Megami_Tensei_Model.Exceptions.TeamFileValidation;

public class InvalidSkillException : TeamFileValidationException
{
    public InvalidSkillException(string message) : base(message) { }
}
