using System.Text.Json;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class GameDataLoader
    {
        private Dictionary<string, Unit> unitData = new(StringComparer.Ordinal);
        private Dictionary<string, Skill> skillData = new(StringComparer.Ordinal);
        private HashSet<string> skillSet = new(StringComparer.Ordinal);

        public void LoadReferenceData()
        {
            unitData.Clear();
            LoadAllUnits();
            LoadAllSkills();
        }

        private void LoadAllUnits()
        {
            var allUnits = GetLoadedUnitsFromMultipleSources();
            AddValidUnitsToDictionary(allUnits);
        }

        private IEnumerable<Unit> GetLoadedUnitsFromMultipleSources()
        {
            var samurais = GetLoadedUnitsFromJson("data/samurai.json");
            var monsters = GetLoadedUnitsFromJson("data/monsters.json");
            return samurais.Concat(monsters);
        }

        private void LoadAllSkills()
        {
            var skills = GetLoadedSkillsFromJson("data/skills.json");
            BuildSkillSet(skills);
            AddValidSkillsToDictionary(skills);
        }

        private void AddValidUnitsToDictionary(IEnumerable<Unit> units)
        {
            foreach (var unit in units)
            {
                if (IsValidUnitName(unit.Name))
                    unitData[unit.Name] = unit;
            }
        }

        private void BuildSkillSet(List<Skill> skills)
        {
            skillSet = new(skills.Where(s => IsValidUnitName(s.Name))
                               .Select(s => s.Name), StringComparer.Ordinal);
        }

        private void AddValidSkillsToDictionary(List<Skill> skills)
        {
            foreach (var skill in skills)
            {
                if (IsValidUnitName(skill.Name))
                    skillData[skill.Name] = skill;
            }
        }

        private bool IsValidUnitName(string? name)
        {
            return !string.IsNullOrWhiteSpace(name);
        }

        private List<Unit> GetLoadedUnitsFromJson(string filePath)
        {
            return JsonSerializer.Deserialize<List<Unit>>(File.ReadAllText(filePath)) ?? new();
        }

        private List<Skill> GetLoadedSkillsFromJson(string filePath)
        {
            return JsonSerializer.Deserialize<List<Skill>>(File.ReadAllText(filePath)) ?? new();
        }

        public Dictionary<string, Unit> GetUnitData()
        {
            return unitData;
        }

        public Dictionary<string, Skill> GetSkillData()
        {
            return skillData;
        }

        public HashSet<string> GetSkillSet()
        {
            return skillSet;
        }
    }
}
