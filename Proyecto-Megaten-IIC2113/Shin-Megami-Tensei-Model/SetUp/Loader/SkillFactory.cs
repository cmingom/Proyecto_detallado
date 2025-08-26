using System.Text.Json;
using Shin_Megami_Tensei_Model.Data.Skills;
using Shin_Megami_Tensei_Model.Exceptions.TeamFileValidation;

namespace Shin_Megami_Tensei_Model.SetUp.Loader;

public static class SkillFactory
{
    private static Dictionary<string, Skill> _skillsCache;
    private static string _basePath = "";

    public static void SetBasePath(string basePath)
    {
        _basePath = basePath;
        _skillsCache = null;
    }

    public static Skill CreateByName(string skillName)
    {
        LoadSkillsIfNeeded();
        
        if (_skillsCache.TryGetValue(skillName, out var skill))
        {
            return skill;
        }
        
        throw new InvalidSkillException($"Skill '{skillName}' not found");
    }

    private static void LoadSkillsIfNeeded()
    {
        if (_skillsCache != null) return;
        
        _skillsCache = new Dictionary<string, Skill>();
        
        var skillsPath = Path.Combine(_basePath, "skills.json");
        var skillsJson = File.ReadAllText(skillsPath);
        var skillsData = JsonSerializer.Deserialize<SkillData[]>(skillsJson) ?? throw new InvalidSkillException("Failed to load skills");
        
        foreach (var skillData in skillsData)
        {
            var skill = new Skill(
                skillData.name,
                skillData.type,
                skillData.cost,
                skillData.skill_power,
                skillData.target,
                skillData.hits,
                skillData.effects
            );
            _skillsCache[skillData.name] = skill;
        }
    }

    private class SkillData
    {
        public string name { get; set; }
        public string type { get; set; }
        public int cost { get; set; }
        public int skill_power { get; set; }
        public string target { get; set; }
        public int hits { get; set; }
        public List<string> effects { get; set; }
    }
}
