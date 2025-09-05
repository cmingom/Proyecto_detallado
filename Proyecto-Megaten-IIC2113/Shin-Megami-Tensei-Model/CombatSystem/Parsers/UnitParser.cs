using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class UnitParser
    {
        private const string SAMURAI_TAG = "[Samurai]";
        private const char OPEN_PARENTHESIS = '(';
        private const char CLOSE_PARENTHESIS = ')';
        private const char COLON = ':';
        private const char COMMA = ',';
        private const int PARENTHESIS_OFFSET = 1;
        
        public UnitInfo? ParseUnitDefinition(string line)
        {
            return IsSamuraiLine(line) ? ParseSamuraiDefinition(line) : ParseMonsterDefinition(line);
        }

        private bool IsSamuraiLine(string line)
        {
            return line.Contains(SAMURAI_TAG, StringComparison.Ordinal);
        }

        private UnitInfo? ParseSamuraiDefinition(string line)
        {
            var rest = GetSamuraiWhithoutTag(line);
            var name = ParseSamuraiName(rest);
            var skills = ParseSamuraiSkills(rest);
            
            if (name == null) return null;
            
            return CreateSamuraiUnitInfo(name, skills);
        }

        private string? ParseSamuraiName(string rest)
        {
            int openParen = rest.IndexOf(OPEN_PARENTHESIS);
            
            if (openParen < 0)
                return rest;

            return GetName(rest, openParen);
        }

        private List<string>? ParseSamuraiSkills(string rest)
        {
            int openParen = rest.IndexOf(OPEN_PARENTHESIS);
            
            if (openParen < 0)
                return null;

            var closeParen = GetCloseParenthesis(rest, openParen);
            if (closeParen < 0) return null;

            return ParseSkillsFromText(rest, openParen, closeParen);
        }

        private UnitInfo CreateSamuraiUnitInfo(string name, List<string>? skills)
        {
            var validSkills = GetValidSkills(skills);
            return new UnitInfo(name, true, validSkills);
        }

        private List<string> GetValidSkills(List<string>? skills)
        {
            return skills ?? new List<string>();
        }

        private string GetSamuraiWhithoutTag(string line)
        {
            return line.Replace(SAMURAI_TAG, string.Empty).Trim();
        }

        private (string? name, List<string>? skills) ParseSamuraiNameAndSkills(string rest)
        {
            int openParen = rest.IndexOf(OPEN_PARENTHESIS);
            
            if (openParen < 0)
                return (rest, null);

            return ParseSamuraiWithSkills(rest, openParen);
        }

        private (string? name, List<string>? skills) ParseSamuraiWithSkills(string rest, int openParen)
        {
            var closeParen = GetCloseParenthesis(rest, openParen);
            if (closeParen < 0) return (null, null);

            var name = GetName(rest, openParen);
            var skills = ParseSkillsFromText(rest, openParen, closeParen);
            
            return (name, skills);
        }

        private int GetCloseParenthesis(string rest, int openParen)
        {
            return rest.IndexOf(CLOSE_PARENTHESIS, openParen + PARENTHESIS_OFFSET);
        }

        private string GetName(string rest, int openParen)
        {
            return rest.Substring(0, openParen).Trim();
        }

        private List<string>? ParseSkillsFromText(string rest, int openParen, int closeParen)
        {
            var skillsText = GetSkillsText(rest, openParen, closeParen);
            return ParseSkillList(skillsText);
        }

        private string GetSkillsText(string rest, int openParen, int closeParen)
        {
            return rest.Substring(openParen + PARENTHESIS_OFFSET, closeParen - openParen - PARENTHESIS_OFFSET);
        }


        private List<string>? ParseSkillList(string skillsText)
        {
            if (string.IsNullOrWhiteSpace(skillsText))
                return new List<string>();

            var parts = skillsText.Split(COMMA);
            return ParseSkillParts(parts);
        }

        private List<string>? ParseSkillParts(string[] parts)
        {
            var skills = new List<string>();
            foreach (var part in parts)
            {
                if (!CanAddValidSkill(skills, part))
                    return null;
            }
            return skills;
        }

        private bool CanAddValidSkill(List<string> skills, string part)
        {
            var skillName = part.Trim();
            if (IsValidSkillName(skillName))
            {
                AddSkillToList(skills, skillName);
                return true;
            }
            return false;
        }

        private bool IsValidSkillName(string skillName)
        {
            return !string.IsNullOrWhiteSpace(skillName);
        }

        private void AddSkillToList(List<string> skills, string skillName)
        {
            skills.Add(skillName);
        }

        private UnitInfo? ParseMonsterDefinition(string line)
        {
            if (HasInvalidCharacters(line))
                return null;
            
            return new UnitInfo(line, false, new List<string>());
        }

        private static bool HasInvalidCharacters(string line)
            => line.Contains(OPEN_PARENTHESIS) || line.Contains(CLOSE_PARENTHESIS) || line.Contains(COLON);
    }
}
