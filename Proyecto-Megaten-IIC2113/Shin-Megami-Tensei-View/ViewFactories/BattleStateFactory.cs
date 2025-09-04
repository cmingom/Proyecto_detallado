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

        public BattleState GetBattleState(string file)
        {
            var lines = ReadFileLines(file);
            var (team1, team2) = ParseTeamsFromLines(lines);
            
            if (!AreTeamsValid(team1, team2))
            {
                return null;
            }
            
            return GetBattleStateFromValidTeams(file);
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
            return gameManager.AreValidTeams(team1, team2);
        }

        private BattleState GetBattleStateFromValidTeams(string file)
        {
            var (parsedTeam1, parsedTeam2) = gameManager.ParseTeamsFromFile(file);
            var unitData = gameManager.GetUnitData();
            
            var team1Units = GetTeamUnits(parsedTeam1, unitData);
            var team2Units = GetTeamUnits(parsedTeam2, unitData);
            
            var battleTeam1 = new TeamState(team1Units);
            var battleTeam2 = new TeamState(team2Units);
            
            return new BattleState(battleTeam1, battleTeam2);
        }

        private List<GetUnitInstance> GetTeamUnits(List<UnitInfo> team, Dictionary<string, Unit> unitData)
        {
            var units = new List<GetUnitInstance>();
            var teamSize = GetTeamSize(team);
            
            var teamContext = new TeamPopulationContext(units, team, teamSize, unitData);
            PopulateTeamUnits(teamContext);
            
            return units;
        }

        private void PopulateTeamUnits(TeamPopulationContext context)
        {
            for (int i = 0; i < context.TeamSize; i++)
            {
                AddUnitToTeam(context, i);
            }
        }

        private void AddUnitToTeam(TeamPopulationContext context, int index)
        {
            var unitInstance = CreateUnitInstance(context.Team[index], TEAM_POSITIONS[index], context.UnitData);
            if (unitInstance != null)
            {
                context.Units.Add(unitInstance);
            }
        }

        private int GetTeamSize(List<UnitInfo> team)
        {
            return Math.Min(team.Count, MAX_UNITS_IN_BATTLE);
        }

        private GetUnitInstance? CreateUnitInstance(UnitInfo unitInfo, char position, Dictionary<string, Unit> unitData)
        {
            var unitTemplate = GetUnitTemplate(unitInfo.Name, unitData);
            if (unitTemplate == null)
            {
                return null;
            }
            
            return BuildUnitInstance(unitInfo, position, unitTemplate);
        }

        private Unit? GetUnitTemplate(string unitName, Dictionary<string, Unit> unitData)
        {
            return unitData.TryGetValue(unitName, out var unitTemplate) ? unitTemplate : null;
        }

        private GetUnitInstance BuildUnitInstance(UnitInfo unitInfo, char position, Unit unitTemplate)
        {
            return new GetUnitInstance(
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
