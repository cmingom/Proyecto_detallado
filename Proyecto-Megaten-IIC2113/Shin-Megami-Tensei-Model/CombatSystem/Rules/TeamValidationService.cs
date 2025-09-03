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

        // ver si es cacho sacar el func
        public TeamValidationService(Func<string, bool> unitExists, Func<string, bool> skillExists)
        {
            _unitExists = unitExists ?? throw new ArgumentNullException(nameof(unitExists));
            _skillExists = skillExists ?? throw new ArgumentNullException(nameof(skillExists));
            this.unitParser = new UnitParser();
        }

        // separar verificar y procesar
        public bool IsValidTeam(List<string> teamLines)
        {
            if (!IsValidTeamInput(teamLines)) return false;
            
            var validationContext = CreateValidationContext();
            return ProcessTeamLines(teamLines, validationContext);
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

        // verbo auxiliar
        // hace dos cosas
        private bool ProcessTeamLines(List<string> teamLines, ValidationContext context)
        {
            foreach (var rawLine in teamLines)
            {
                if (!ProcessSingleTeamLine(rawLine, context)) return false;
            }
            return context.SamuraiCount == REQUIRED_SAMURAI_COUNT;
        }

        // verbo auxiliar
        // hace dos cosas
        private bool ProcessSingleTeamLine(string rawLine, ValidationContext context)
        {
            var line = rawLine.Trim();
            if (line.Length == EMPTY_LINE_LENGTH) return true;

            var unitInfo = unitParser.ParseUnitDefinition(line);
            if (unitInfo == null) return false;

            return ValidateUnitInfo(unitInfo, context);
        }

        // verbo auxiliar
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

        // verbo auxiliar
        private bool ValidateSamuraiSkills(List<string> skills)
        {
            if (!IsValidSkillCount(skills)) return false;
            
            var uniqueSkills = new HashSet<string>(StringComparer.Ordinal);
            return ValidateAllSkills(skills, uniqueSkills);
        }

        private bool IsValidSkillCount(List<string> skills)
        {
            return skills.Count <= MAX_SKILLS_PER_SAMURAI;
        }

        // verbo auxiliar
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
