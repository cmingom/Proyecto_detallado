using Shin_Megami_Tensei_Model.Data.Components;
using Shin_Megami_Tensei_Model.SetUp.Validator;

namespace Shin_Megami_Tensei_Model.SetUp.Loader;

public static class UnitLoader
{
    public static Unit GetUnitFromTextLine(string textLine)
    {
        var newUnitInfo = UnitInfoLoader.GetFromTextLine(textLine);
        var newUnit = new Unit(newUnitInfo);
        
        if (newUnitInfo.IsSamurai)
        {
            AddSkillsFromTextLineToUnit(textLine, newUnit);
        }
        else
        {
            AddPredefinedSkillsToUnit(newUnitInfo, newUnit);
        }
        
        UnitValidator.ValidateUnit(newUnit);
        return newUnit;
    }

    private static void AddSkillsFromTextLineToUnit(string textLine, Unit unit)
    {
        string[] skillNames = [];
        if (UnitHasSkills(textLine))
            skillNames = GetSkillsNamesFromTextLine(textLine);
        var newSkills = skillNames.Select(SkillFactory.CreateByName);
        unit.Skills.AddSkills(newSkills.ToArray());
    }
    
    private static void AddPredefinedSkillsToUnit(UnitInfo unitInfo, Unit unit)
    {
        if (unitInfo.Skills != null)
        {
            var skills = unitInfo.Skills.Select(SkillFactory.CreateByName);
            unit.Skills.AddSkills(skills.ToArray());
        }
    }

    private static bool UnitHasSkills(string textLine)
    {
        return textLine.Contains('(');
    }

    private static string[] GetSkillsNamesFromTextLine(string textLine)
    {
        var skillsText = GetSkillsText(textLine);
        return GetIndividualSkillsTexts(skillsText);
    }

    private static string GetSkillsText(string textLine)
    {
        var startIndex = textLine.IndexOf('(') + 1;
        var endIndex = textLine.IndexOf(')');
        var skillsText = textLine[startIndex..endIndex];
        skillsText = skillsText.Trim();
        return skillsText;
    }

    private static string[] GetIndividualSkillsTexts(string skillsText)
    {
        var skillsIndividualTexts = skillsText.Split(',');
        var skillsIndividualTextsCleaned = skillsIndividualTexts.Select(skill => skill.Trim());
        return skillsIndividualTextsCleaned.ToArray();
    }
}