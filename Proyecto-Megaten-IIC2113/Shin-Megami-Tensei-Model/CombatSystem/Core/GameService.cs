using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Rules;
using Shin_Megami_Tensei_Model.CombatSystem.Setup;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class GameService
    {
        private Dictionary<string, Unit> unitData = new(StringComparer.Ordinal);
        private Dictionary<string, Skill> skillData = new(StringComparer.Ordinal);
        private HashSet<string> skillSet = new(StringComparer.Ordinal);

        public void LoadReferenceData()
        {
            unitData.Clear();

            var samurais = LoadUnitsFromJson("data/samurai.json");
            var monsters = LoadUnitsFromJson("data/monsters.json");

            foreach (var unit in samurais.Concat(monsters))
            {
                if (!string.IsNullOrWhiteSpace(unit.Name))
                    unitData[unit.Name] = unit;
            }

            var skills = LoadSkillsFromJson("data/skills.json");
            skillSet = new(skills.Where(s => !string.IsNullOrWhiteSpace(s.Name))
                               .Select(s => s.Name), StringComparer.Ordinal);
            
            foreach (var skill in skills)
            {
                if (!string.IsNullOrWhiteSpace(skill.Name))
                    skillData[skill.Name] = skill;
            }
        }

        private List<Unit> LoadUnitsFromJson(string filePath)
        {
            return JsonSerializer.Deserialize<List<Unit>>(File.ReadAllText(filePath)) ?? new();
        }

        private List<Skill> LoadSkillsFromJson(string filePath)
        {
            return JsonSerializer.Deserialize<List<Skill>>(File.ReadAllText(filePath)) ?? new();
        }

        public List<UnitInfo> BuildUnitInfoList(List<string> teamLines)
        {
            var units = new List<UnitInfo>();
            
            foreach (var line in teamLines)
            {
                var unitInfo = ParseUnitDefinition(line);
                if (unitInfo != null)
                {
                    units.Add(unitInfo);
                }
            }
            
            return units;
        }

        private UnitInfo? ParseUnitDefinition(string line)
        {
            bool isSamurai = line.Contains("[Samurai]", StringComparison.Ordinal);
            
            if (isSamurai)
            {
                return ParseSamuraiDefinition(line);
            }
            else
            {
                return ParseMonsterDefinition(line);
            }
        }

        private UnitInfo? ParseSamuraiDefinition(string line)
        {
            var rest = line.Replace("[Samurai]", string.Empty).Trim();
            var skills = new List<string>();

            int openParen = rest.IndexOf('(');
            string name;

            if (openParen >= 0)
            {
                int closeParen = rest.IndexOf(')', openParen + 1);
                if (closeParen < 0) return null;

                name = rest.Substring(0, openParen).Trim();
                var skillsText = rest.Substring(openParen + 1, closeParen - openParen - 1);
                
                if (!string.IsNullOrWhiteSpace(skillsText))
                {
                    skills = ParseSkillList(skillsText);
                    if (skills == null) return null;
                }

                if (!string.IsNullOrWhiteSpace(rest.Substring(closeParen + 1)))
                    return null;
            }
            else
            {
                name = rest;
            }

            return new UnitInfo(name, true, skills);
        }

        private UnitInfo? ParseMonsterDefinition(string line)
        {
            if (HasInvalidCharacters(line))
                return null;
            
            return new UnitInfo(line, false, new List<string>());
        }

        private List<string>? ParseSkillList(string skillsText)
        {
            if (string.IsNullOrWhiteSpace(skillsText))
                return new List<string>();

            var skills = new List<string>();
            var parts = skillsText.Split(',');
            
            foreach (var part in parts)
            {
                var skillName = part.Trim();
                if (string.IsNullOrWhiteSpace(skillName))
                    return null;
                skills.Add(skillName);
            }
            
            return skills;
        }

        private static bool HasInvalidCharacters(string line)
            => line.Contains('(') || line.Contains(')') || line.Contains(':');

        public bool ValidateTeams(List<string> team1, List<string> team2)
        {
            var validator = new TeamValidator(
                unitName => unitData.ContainsKey(unitName),
                skillName => skillSet.Contains(skillName));

            return validator.IsValidTeam(team1) && validator.IsValidTeam(team2);
        }

        public (List<UnitInfo>, List<UnitInfo>) ParseTeamsFromFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            var (team1, team2) = TeamParser.ParseTeams(lines);

            var parsedTeam1 = BuildUnitInfoList(team1);
            var parsedTeam2 = BuildUnitInfoList(team2);

            return (parsedTeam1, parsedTeam2);
        }

        public Dictionary<string, Unit> GetUnitData()
        {
            return unitData;
        }

        public Dictionary<string, Skill> GetSkillData()
        {
            return skillData;
        }

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
    }
}
