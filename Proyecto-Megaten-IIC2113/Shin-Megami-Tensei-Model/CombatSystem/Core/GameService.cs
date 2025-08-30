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
            LoadAllUnits();
            LoadAllSkills();
        }

        private void LoadAllUnits()
        {
            var samurais = LoadUnitsFromJson("data/samurai.json");
            var monsters = LoadUnitsFromJson("data/monsters.json");
            AddValidUnitsToDictionary(samurais.Concat(monsters));
        }

        private void LoadAllSkills()
        {
            var skills = LoadSkillsFromJson("data/skills.json");
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
                AddValidUnitInfo(units, line);
            }
            return units;
        }

        private void AddValidUnitInfo(List<UnitInfo> units, string line)
        {
            var unitInfo = ParseUnitDefinition(line);
            if (unitInfo != null)
            {
                units.Add(unitInfo);
            }
        }

        private UnitInfo? ParseUnitDefinition(string line)
        {
            return IsSamuraiLine(line) ? ParseSamuraiDefinition(line) : ParseMonsterDefinition(line);
        }

        private bool IsSamuraiLine(string line)
        {
            return line.Contains("[Samurai]", StringComparison.Ordinal);
        }

        private UnitInfo? ParseSamuraiDefinition(string line)
        {
            var rest = RemoveSamuraiTag(line);
            var (name, skills) = ParseSamuraiNameAndSkills(rest);
            
            if (name == null) return null;
            
            return new UnitInfo(name, true, skills ?? new List<string>());
        }

        private string RemoveSamuraiTag(string line)
        {
            return line.Replace("[Samurai]", string.Empty).Trim();
        }

        private (string? name, List<string>? skills) ParseSamuraiNameAndSkills(string rest)
        {
            int openParen = rest.IndexOf('(');
            
            if (openParen < 0)
                return (rest, null);

            return ParseSamuraiWithSkills(rest, openParen);
        }

        private (string? name, List<string>? skills) ParseSamuraiWithSkills(string rest, int openParen)
        {
            var closeParen = FindCloseParenthesis(rest, openParen);
            if (closeParen < 0) return (null, null);

            var name = ExtractName(rest, openParen);
            var skills = ParseSkillsFromText(rest, openParen, closeParen);
            
            if (skills == null || HasRemainingTextAfterSkills(rest, closeParen)) return (null, null);

            return (name, skills);
        }

        private int FindCloseParenthesis(string rest, int openParen)
        {
            return rest.IndexOf(')', openParen + 1);
        }

        private string ExtractName(string rest, int openParen)
        {
            return rest.Substring(0, openParen).Trim();
        }

        private List<string>? ParseSkillsFromText(string rest, int openParen, int closeParen)
        {
            var skillsText = ExtractSkillsText(rest, openParen, closeParen);
            if (!IsValidSkillsText(skillsText)) return null;
            return ParseSkillList(skillsText);
        }

        private string ExtractSkillsText(string rest, int openParen, int closeParen)
        {
            return rest.Substring(openParen + 1, closeParen - openParen - 1);
        }

        private bool IsValidSkillsText(string skillsText)
        {
            return !string.IsNullOrWhiteSpace(skillsText);
        }

        private bool HasRemainingTextAfterSkills(string rest, int closeParen)
        {
            return !string.IsNullOrWhiteSpace(rest.Substring(closeParen + 1));
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

            var parts = skillsText.Split(',');
            return ParseSkillParts(parts);
        }

        private List<string>? ParseSkillParts(string[] parts)
        {
            var skills = new List<string>();
            foreach (var part in parts)
            {
                if (!AddValidSkill(skills, part))
                    return null;
            }
            return skills;
        }

        private bool AddValidSkill(List<string> skills, string part)
        {
            var skillName = part.Trim();
            if (string.IsNullOrWhiteSpace(skillName))
                return false;
            
            skills.Add(skillName);
            return true;
        }

        private static bool HasInvalidCharacters(string line)
            => line.Contains('(') || line.Contains(')') || line.Contains(':');

        public bool ValidateTeams(List<string> team1, List<string> team2)
        {
            var validator = CreateTeamValidator();
            return AreBothTeamsValid(validator, team1, team2);
        }

        private TeamValidator CreateTeamValidator()
        {
            return new TeamValidator(
                unitName => unitData.ContainsKey(unitName),
                skillName => skillSet.Contains(skillName));
        }

        private bool AreBothTeamsValid(TeamValidator validator, List<string> team1, List<string> team2)
        {
            return validator.IsValidTeam(team1) && validator.IsValidTeam(team2);
        }

        public (List<UnitInfo>, List<UnitInfo>) ParseTeamsFromFile(string filePath)
        {
            var lines = ReadTeamFile(filePath);
            var (team1, team2) = ParseTeamLines(lines);
            return BuildBothTeams(team1, team2);
        }

        private string[] ReadTeamFile(string filePath)
        {
            return File.ReadAllLines(filePath);
        }

        private (List<string>, List<string>) ParseTeamLines(string[] lines)
        {
            return TeamParser.ParseTeams(lines);
        }

        private (List<UnitInfo>, List<UnitInfo>) BuildBothTeams(List<string> team1, List<string> team2)
        {
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
    }
}
