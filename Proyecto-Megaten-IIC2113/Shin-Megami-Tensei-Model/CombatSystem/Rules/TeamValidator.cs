using Shin_Megami_Tensei_Model.CombatSystem.Core;

namespace Shin_Megami_Tensei_Model.CombatSystem.Rules
{
    public class TeamValidator
    {
        private readonly TeamValidationService teamValidationService;

        public TeamValidator(Func<string, bool> unitExists, Func<string, bool> skillExists)
        {
            this.teamValidationService = new TeamValidationService(unitExists, skillExists);
        }

        public bool IsValidTeam(List<string> teamLines)
        {
            return teamValidationService.IsValidTeam(teamLines);
        }
    }
}