using System;
using System.Collections.Generic;
using Shin_Megami_Tensei_Model.CombatSystem.Parsers;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Rules
{
    public class TeamValidationService
    {
        private const int MaxUnitsPerTeam = 8;
        private const int MaxSamuraiSkills = 8;
        private const int RequiredSamuraiCount = 1;

        private readonly Func<string, bool> unitExists;
        private readonly Func<string, bool> skillExists;
        private readonly UnitParser unitParser = new();

        public TeamValidationService(Func<string, bool> unitExists, Func<string, bool> skillExists)
        {
            this.unitExists = unitExists ?? throw new ArgumentNullException(nameof(unitExists));
            this.skillExists = skillExists ?? throw new ArgumentNullException(nameof(skillExists));
        }

        public bool IsValidTeam(List<string>? teamLines)
        {
            if (!IsInputWithinLimits(teamLines))
            {
                return false;
            }

            var validationContext = new ValidationContext(new HashSet<string>(StringComparer.Ordinal));

            foreach (var line in teamLines!)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                if (!ValidateUnitLine(line.Trim(), validationContext))
                {
                    return false;
                }
            }

            return validationContext.SamuraiCount == RequiredSamuraiCount;
        }

        private static bool IsInputWithinLimits(List<string>? teamLines)
        {
            return teamLines != null && teamLines.Count <= MaxUnitsPerTeam;
        }

        private bool ValidateUnitLine(string rawLine, ValidationContext context)
        {
            var unitInfo = unitParser.ParseUnitDefinition(rawLine);
            if (unitInfo == null)
            {
                return false;
            }

            if (!unitExists(unitInfo.Name))
            {
                return false;
            }

            if (!context.SeenUnits.Add(unitInfo.Name))
            {
                return false;
            }

            if (unitInfo.IsSamurai)
            {
                context.SamuraiCount++;
                if (!ValidateSamuraiSkills(unitInfo.Skills))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidateSamuraiSkills(List<string> skills)
        {
            if (skills.Count > MaxSamuraiSkills)
            {
                return false;
            }

            var uniqueSkills = new HashSet<string>(StringComparer.Ordinal);
            foreach (var skill in skills)
            {
                if (!uniqueSkills.Add(skill) || !skillExists(skill))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
