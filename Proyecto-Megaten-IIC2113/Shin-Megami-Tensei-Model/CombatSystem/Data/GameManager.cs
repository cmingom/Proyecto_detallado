using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Rules;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class GameManager
    {
        private readonly GameDataLoader dataLoader;
        private readonly TeamParser teamParser;
        private TeamValidator teamValidator;
        private bool UnitExists(string unitName) => dataLoader.GetUnitData().ContainsKey(unitName);
        private bool SkillExists(string skillName) => dataLoader.GetSkillSet().Contains(skillName);

        public GameManager()
        {
            this.dataLoader = new GameDataLoader();
            this.teamParser = new TeamParser(new UnitParser());
            this.teamValidator = new TeamValidator(UnitExists, SkillExists);
        }

        public void LoadReferenceData()
        {
            dataLoader.LoadReferenceData();
            this.teamValidator = new TeamValidator(UnitExists, SkillExists);
        }

        // verbo auxiliar
        public bool ValidateTeams(List<string> team1, List<string> team2)
        {
            return teamValidator.IsValidTeam(team1) && teamValidator.IsValidTeam(team2);
        }

        public (List<UnitInfo>, List<UnitInfo>) ParseTeamsFromFile(string filePath)
        {
            return teamParser.ParseTeamsFromFile(filePath);
        }

        public Dictionary<string, Unit> GetUnitData()
        {
            return dataLoader.GetUnitData();
        }

        public Dictionary<string, Skill> GetSkillData()
        {
            return dataLoader.GetSkillData();
        }
    }
}
