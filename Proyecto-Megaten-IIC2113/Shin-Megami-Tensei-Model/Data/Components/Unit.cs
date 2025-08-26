using Shin_Megami_Tensei_Model.Data.Modifiers;
using Shin_Megami_Tensei_Model.Data.Skills;
using Shin_Megami_Tensei_Model.Enum;
using Shin_Megami_Tensei_Model.SetUp.Loader;

namespace Shin_Megami_Tensei_Model.Data.Components;

public class Unit(UnitInfo unitInfo)
{
    public readonly string Name = unitInfo.Name;
    public readonly bool IsSamurai = unitInfo.IsSamurai;
    public readonly UnitAllModifiers AllModifiers = unitInfo.GetAllModifiers();
    public readonly SkillCollection Skills = new();

    public bool IsAlive()
    {
        return AllModifiers.Hp.Value > 0;
    }
}