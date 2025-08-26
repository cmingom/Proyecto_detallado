namespace Shin_Megami_Tensei_Model.Exceptions.TeamFileValidation;

public class UnitInfoLoadException : TeamFileValidationException
{
    public UnitInfoLoadException() : base("Failed to load unit information")
    {
    }
}
