using System.Collections;

namespace Shin_Megami_Tensei_Model.Data.Skills;

public class SkillCollection : IEnumerable<Skill>
{
    private readonly List<Skill> _skills = [];

    public int Count => _skills.Count;

    public IEnumerator<Skill> GetEnumerator()
    {
        return _skills.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void AddSkills(params Skill[] newSkills)
    {
        _skills.AddRange(newSkills);
    }

    public string[] GetSkillsNames()
    {
        var skillsNames = _skills.Select(skill => skill.Name);
        return skillsNames.ToArray();
    }
    
    public List<Skill> GetUsableSkills(int currentMp)
    {
        return _skills.Where(skill => skill.Cost <= currentMp).ToList();
    }
}
