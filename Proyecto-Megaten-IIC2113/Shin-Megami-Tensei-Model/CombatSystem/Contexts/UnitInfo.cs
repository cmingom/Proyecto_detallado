namespace Shin_Megami_Tensei_Model.Domain.Entities;

public class UnitInfo
{
    public string Name { get; }
    public bool IsSamurai { get; }
    public List<string> Skills { get; }

    public UnitInfo(string name, bool isSamurai, List<string> skills)
    {
        Name = name;
        IsSamurai = isSamurai;
        Skills = skills;
    }
}
