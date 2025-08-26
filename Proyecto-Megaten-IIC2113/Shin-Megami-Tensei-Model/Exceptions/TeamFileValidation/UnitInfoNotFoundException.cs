namespace Shin_Megami_Tensei_Model.Exceptions.TeamFileValidation;

public class UnitInfoNotFoundException : TeamFileValidationException
{
    public UnitInfoNotFoundException() : base("Unit info not found")
    {
    }
}
