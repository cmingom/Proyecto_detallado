using System;
using System.Collections.Generic;

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
            if (teamLines == null) return false;
            
            const int maxUnits = 8;
            if (teamLines.Count > maxUnits) return false;

            int samuraiCount = 0;
            var seenUnits = new HashSet<string>(StringComparer.Ordinal);

            foreach (var rawLine in teamLines)
            {
                var line = rawLine.Trim();
                if (line.Length == 0) continue;

                var unitInfo = ParseUnitLine(line);
                if (unitInfo == null) return false;

                if (unitInfo.IsSamurai)
                {
                    samuraiCount++;
                    if (!ValidateSamuraiSkills(unitInfo.Skills)) return false;
                }

                if (!_unitExists(unitInfo.Name)) return false;
                if (!seenUnits.Add(unitInfo.Name)) return false;
            }

            return samuraiCount == 1;
        }

        private UnitInfo? ParseUnitLine(string line)
        {
            bool isSamurai = line.Contains("[Samurai]", StringComparison.Ordinal);
            
            if (isSamurai)
            {
                return ParseSamuraiLine(line);
            }
            else
            {
                return ParseMonsterLine(line);
            }
        }

        private UnitInfo? ParseSamuraiLine(string line)
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

        private UnitInfo? ParseMonsterLine(string line)
        {
            if (line.Contains('(') || line.Contains(')') || line.Contains(':'))
                return null;
            
            return new UnitInfo(line, false, new List<string>());
        }

        private List<string>? ParseSkillList(string skillsText)
        {
            var skills = new List<string>();
            foreach (var skill in skillsText.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                var trimmedSkill = skill.Trim();
                if (trimmedSkill.Length == 0) return null;
                skills.Add(trimmedSkill);
            }
            return skills;
        }

        private bool ValidateSamuraiSkills(List<string> skills)
        {
            if (skills.Count > 8) return false;
            
            var uniqueSkills = new HashSet<string>(StringComparer.Ordinal);
            foreach (var skill in skills)
            {
                if (!uniqueSkills.Add(skill)) return false;
                if (!_skillExists(skill)) return false;
            }
            return true;
        }

        private record UnitInfo(string Name, bool IsSamurai, List<string> Skills);
    }
}