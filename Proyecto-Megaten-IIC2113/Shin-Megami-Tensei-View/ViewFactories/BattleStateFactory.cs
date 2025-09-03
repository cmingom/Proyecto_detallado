using Shin_Megami_Tensei_Model.CombatSystem.Core;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei
{
    public class BattleStateFactory
    {
        private const int MAX_UNITS_IN_BATTLE = 4;
        private const char POSITION_A = 'A';
        private const char POSITION_B = 'B';
        private const char POSITION_C = 'C';
        private const char POSITION_D = 'D';
        private static readonly char[] TEAM_POSITIONS = { POSITION_A, POSITION_B, POSITION_C, POSITION_D };

        private readonly GameManager gameManager;

        public BattleStateFactory(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        // get
        public BattleState CreateBattleState(string file)
        {
            var lines = ReadFileLines(file);
            var (team1, team2) = ParseTeamsFromLines(lines);
            
            if (!AreTeamsValid(team1, team2))
            {
                return null;
            }
            
            return CreateBattleStateFromValidTeams(file);
        }

        private string[] ReadFileLines(string file)
        {
            return File.ReadAllLines(file);
        }

        private (List<string> team1, List<string> team2) ParseTeamsFromLines(string[] lines)
        {
            var teamParser = new TeamParser(new UnitParser());
            return teamParser.ParseTeamLines(lines);
        }

        private bool AreTeamsValid(List<string> team1, List<string> team2)
        {
            return gameManager.ValidateTeams(team1, team2);
        }

        // get
        private BattleState CreateBattleStateFromValidTeams(string file)
        {
            var (parsedTeam1, parsedTeam2) = gameManager.ParseTeamsFromFile(file);
            var unitData = gameManager.GetUnitData();
            
            var team1Units = CreateTeamUnits(parsedTeam1, unitData);
            var team2Units = CreateTeamUnits(parsedTeam2, unitData);
            
            var battleTeam1 = new TeamState(team1Units);
            var battleTeam2 = new TeamState(team2Units);
            
            return new BattleState(battleTeam1, battleTeam2);
        }

        // get
        private List<UnitInstance> CreateTeamUnits(List<UnitInfo> team, Dictionary<string, Unit> unitData)
        {
            var units = new List<UnitInstance>();
            var teamSize = GetTeamSize(team);
            
            PopulateTeamUnits(units, team, teamSize, unitData);
            
            return units;
        }

        // recibe 5
        private void PopulateTeamUnits(List<UnitInstance> units, List<UnitInfo> team, int teamSize, Dictionary<string, Unit> unitData)
        {
            for (int i = 0; i < teamSize; i++)
            {
                AddUnitToTeam(units, team[i], TEAM_POSITIONS[i], unitData);
            }
        }

        // recibe 5
        private void AddUnitToTeam(List<UnitInstance> units, UnitInfo unitInfo, char position, Dictionary<string, Unit> unitData)
        {
            var unitInstance = CreateUnitInstance(unitInfo, position, unitData);
            if (unitInstance != null)
            {
                units.Add(unitInstance);
            }
        }

        private int GetTeamSize(List<UnitInfo> team)
        {
            return Math.Min(team.Count, MAX_UNITS_IN_BATTLE);
        }

        // recibe 4
        private UnitInstance? CreateUnitInstance(UnitInfo unitInfo, char position, Dictionary<string, Unit> unitData)
        {
            if (!TryGetUnitTemplate(unitInfo.Name, unitData, out var unitTemplate))
            {
                return null;
            }
            
            return BuildUnitInstance(unitInfo, position, unitTemplate);
        }

        // recibe out
        // verbo auxiliar
        // recibe 4
        private bool TryGetUnitTemplate(string unitName, Dictionary<string, Unit> unitData, out Unit unitTemplate)
        {
            return unitData.TryGetValue(unitName, out unitTemplate);
        }

        private UnitInstance BuildUnitInstance(UnitInfo unitInfo, char position, Unit unitTemplate)
        {
            return new UnitInstance(
                name: unitInfo.Name,
                maxHP: unitTemplate.Stats.HP,
                maxMP: unitTemplate.Stats.MP,
                str: unitTemplate.Stats.Str,
                skl: unitTemplate.Stats.Skl,
                spd: unitTemplate.Stats.Spd,
                isSamurai: unitInfo.IsSamurai,
                position: position,
                skills: GetUnitSkills(unitInfo, unitTemplate)
            );
        }

        private List<string> GetUnitSkills(UnitInfo unitInfo, Unit unitTemplate)
        {
            return unitInfo.IsSamurai ? unitInfo.Skills : unitTemplate.Skills;
        }
    }
}
