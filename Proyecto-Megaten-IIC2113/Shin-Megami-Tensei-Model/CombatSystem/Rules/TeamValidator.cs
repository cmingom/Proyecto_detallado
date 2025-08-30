using System;
using System.Collections.Generic;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Rules
{
    public class TeamValidator
    {
        private readonly Func<string, bool> _unitExists;
        private readonly Func<string, bool> _skillExists;

        public TeamValidator(Func<string, bool> unitExists, Func<string, bool> skillExists)
        {
            _unitExists = unitExists ?? throw new ArgumentNullException(nameof(unitExists));
            _skillExists = skillExists ?? throw new ArgumentNullException(nameof(skillExists));
        }

        public bool IsValidTeam(List<string> teamLines)
        {
            if (!IsValidTeamInput(teamLines)) return false;
            
            var validationContext = CreateValidationContext();
            return ProcessTeamLines(teamLines, validationContext);
        }

        private bool IsValidTeamInput(List<string>? teamLines)
        {
            if (teamLines == null) return false;
            const int maxUnits = 8;
            return teamLines.Count <= maxUnits;
        }

        private ValidationContext CreateValidationContext()
        {
            return new ValidationContext(new HashSet<string>(StringComparer.Ordinal));
        }

        private bool ProcessTeamLines(List<string> teamLines, ValidationContext context)
        {
            foreach (var rawLine in teamLines)
            {
                if (!ProcessSingleTeamLine(rawLine, context)) return false;
            }
            return context.SamuraiCount == 1;
        }

        private bool ProcessSingleTeamLine(string rawLine, ValidationContext context)
        {
            var line = rawLine.Trim();
            if (line.Length == 0) return true;

            var unitInfo = ParseUnitLine(line);
            if (unitInfo == null) return false;

            return ValidateUnitInfo(unitInfo, context);
        }

        private bool ValidateUnitInfo(UnitInfo unitInfo, ValidationContext context)
        {
            if (unitInfo.IsSamurai)
            {
                context.SamuraiCount++;
                if (!ValidateSamuraiSkills(unitInfo.Skills)) return false;
            }

            if (!_unitExists(unitInfo.Name)) return false;
            return context.SeenUnits.Add(unitInfo.Name);
        }



        private UnitInfo? ParseUnitLine(string line)
        {
            return IsSamuraiLine(line) ? ParseSamuraiLine(line) : ParseMonsterLine(line);
        }

        private bool IsSamuraiLine(string line)
        {
            return line.Contains("[Samurai]", StringComparison.Ordinal);
        }

        private UnitInfo? ParseSamuraiLine(string line)
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

        private UnitInfo? ParseMonsterLine(string line)
        {
            if (line.Contains('(') || line.Contains(')') || line.Contains(':'))
                return null;
            
            return new UnitInfo(line, false, new List<string>());
        }

        private List<string>? ParseSkillList(string skillsText)
        {
            var skillParts = skillsText.Split(',', StringSplitOptions.RemoveEmptyEntries);
            return ParseSkillParts(skillParts);
        }

        private List<string>? ParseSkillParts(string[] skillParts)
        {
            var skills = new List<string>();
            foreach (var skill in skillParts)
            {
                if (!AddValidSkill(skills, skill)) return null;
            }
            return skills;
        }

        private bool AddValidSkill(List<string> skills, string skill)
        {
            var trimmedSkill = skill.Trim();
            if (trimmedSkill.Length == 0) return false;
            skills.Add(trimmedSkill);
            return true;
        }

        private bool ValidateSamuraiSkills(List<string> skills)
        {
            if (!IsValidSkillCount(skills)) return false;
            
            var uniqueSkills = new HashSet<string>(StringComparer.Ordinal);
            return ValidateAllSkills(skills, uniqueSkills);
        }

        private bool IsValidSkillCount(List<string> skills)
        {
            return skills.Count <= 8;
        }

        private bool ValidateAllSkills(List<string> skills, HashSet<string> uniqueSkills)
        {
            foreach (var skill in skills)
            {
                if (!IsValidSkill(skill, uniqueSkills)) return false;
            }
            return true;
        }

        private bool IsValidSkill(string skill, HashSet<string> uniqueSkills)
        {
            if (!uniqueSkills.Add(skill)) return false;
            return _skillExists(skill);
        }

    }
}