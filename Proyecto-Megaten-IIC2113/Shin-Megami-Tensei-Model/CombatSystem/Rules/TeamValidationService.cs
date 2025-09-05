using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Core;

namespace Shin_Megami_Tensei_Model.CombatSystem.Rules
{
    public class TeamValidationService
    {
        private const int MAX_UNITS_PER_TEAM = 8;
        private const int MAX_SKILLS_PER_SAMURAI = 8;
        private const int REQUIRED_SAMURAI_COUNT = 1;
        private const int EMPTY_LINE_LENGTH = 0;
        
        private readonly Func<string, bool> _unitExists;
        private readonly Func<string, bool> _skillExists;
        private readonly UnitParser unitParser;

        public TeamValidationService(Func<string, bool> unitExists, Func<string, bool> skillExists)
        {
            _unitExists = unitExists ?? throw new ArgumentNullException(nameof(unitExists));
            _skillExists = skillExists ?? throw new ArgumentNullException(nameof(skillExists));
            this.unitParser = new UnitParser();
        }

        public bool IsValidTeam(List<string> teamLines)
        {
            if (!IsValidTeamInput(teamLines)) return false;
            
            return CanValidateTeam(teamLines);
        }

        private bool CanValidateTeam(List<string> teamLines)
        {
            var validationContext = CreateValidationContext();
            return CanProcessTeamLines(teamLines, validationContext);
        }

        private bool IsValidTeamInput(List<string>? teamLines)
        {
            if (teamLines == null) return false;
            return teamLines.Count <= MAX_UNITS_PER_TEAM;
        }

        private ValidationContext CreateValidationContext()
        {
            return new ValidationContext(new HashSet<string>(StringComparer.Ordinal));
        }

        private bool CanProcessTeamLines(List<string> teamLines, ValidationContext context)
        {
            if (!CanProcessAllTeamLines(teamLines, context)) return false;
            return IsValidSamuraiCount(context);
        }

        private bool CanProcessAllTeamLines(List<string> teamLines, ValidationContext context)
        {
            foreach (var rawLine in teamLines)
            {
                if (!CanProcessSingleTeamLine(rawLine, context)) return false;
            }
            return true;
        }

        private bool IsValidSamuraiCount(ValidationContext context)
        {
            return context.SamuraiCount == REQUIRED_SAMURAI_COUNT;
        }

        private bool CanProcessSingleTeamLine(string rawLine, ValidationContext context)
        {
            var line = GetTrimmedLine(rawLine);
            if (IsEmptyLine(line)) return true;

            return CanProcessUnitLine(line, context);
        }

        private string GetTrimmedLine(string rawLine)
        {
            return rawLine.Trim();
        }

        private bool IsEmptyLine(string line)
        {
            return line.Length == EMPTY_LINE_LENGTH;
        }

        private bool CanProcessUnitLine(string line, ValidationContext context)
        {
            var unitInfo = unitParser.ParseUnitDefinition(line);
            if (unitInfo == null) return false;

            return IsValidUnitInfo(unitInfo, context);
        }

        // to do: poner exception
        private bool IsValidUnitInfo(UnitInfo unitInfo, ValidationContext context)
        {
            if (unitInfo.IsSamurai)
            {
                context.SamuraiCount++;
                if (!AreValidSamuraiSkills(unitInfo.Skills)) return false;
            }

            if (!_unitExists(unitInfo.Name)) return false;
            return context.SeenUnits.Add(unitInfo.Name);
        }

        private bool AreValidSamuraiSkills(List<string> skills)
        {
            if (!IsValidSkillCount(skills)) return false;
            
            var uniqueSkills = new HashSet<string>(StringComparer.Ordinal);
            return AreAllValidSkills(skills, uniqueSkills);
        }

        private bool IsValidSkillCount(List<string> skills)
        {
            return skills.Count <= MAX_SKILLS_PER_SAMURAI;
        }

        private bool AreAllValidSkills(List<string> skills, HashSet<string> uniqueSkills)
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
