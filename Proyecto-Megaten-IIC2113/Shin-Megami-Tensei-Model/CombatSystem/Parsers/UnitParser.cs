using System;
using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class UnitParser
    {
        public UnitInfo? ParseUnitDefinition(string line)
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
    }
}
