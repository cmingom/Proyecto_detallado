using Shin_Megami_Tensei_Model.Data.Components;
using Shin_Megami_Tensei_Model.Data.Skills;
using Shin_Megami_Tensei_Model.Exceptions.TeamFileValidation;

namespace Shin_Megami_Tensei_Model.SetUp.Validator;

public static class UnitValidator
{
    public static void ValidateUnit(Unit unit)
    {
        if (unit.IsSamurai)
        {
            ValidateSamuraiSkills(unit.Skills);
        }
    }

    private static void ValidateSamuraiSkills(SkillCollection skills)
    {
        CorrectSkillsNumber(skills);
        SkillsAreUnique(skills);
    }

    private static void CorrectSkillsNumber(SkillCollection skills)
    {
        if (skills.Count > 8) 
            throw new IncorrectUnitSkillsNumberException();
    }

    private static void SkillsAreUnique(SkillCollection skills)
    {
        var allSkillsWithName = skills.Select(skill => skill.Name);
        var allUniqueSkillsWithName = allSkillsWithName.Distinct();
        if (allUniqueSkillsWithName.Count() != skills.Count)
            throw new UnitSkillsAreNotUniqueException();
    }
}