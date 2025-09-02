using System.Collections.Generic;
using System.IO;
using Shin_Megami_Tensei_Model.CombatSystem.Core;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei
{
    public class BattleStateFactory
    {
        private readonly GameManager gameManager;

        public BattleStateFactory(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

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

        private List<UnitInstance> CreateTeamUnits(List<UnitInfo> team, Dictionary<string, Unit> unitData)
        {
            var units = new List<UnitInstance>();
            var positions = GetTeamPositions();
            var teamSize = GetTeamSize(team);
            
            PopulateTeamUnits(units, team, positions, teamSize, unitData);
            
            return units;
        }

        private void PopulateTeamUnits(List<UnitInstance> units, List<UnitInfo> team, char[] positions, int teamSize, Dictionary<string, Unit> unitData)
        {
            for (int i = 0; i < teamSize; i++)
            {
                AddUnitToTeam(units, team[i], positions[i], unitData);
            }
        }

        private void AddUnitToTeam(List<UnitInstance> units, UnitInfo unitInfo, char position, Dictionary<string, Unit> unitData)
        {
            var unitInstance = CreateUnitInstance(unitInfo, position, unitData);
            if (unitInstance != null)
            {
                units.Add(unitInstance);
            }
        }

        private char[] GetTeamPositions()
        {
            return new char[] { 'A', 'B', 'C', 'D' };
        }

        private const int MAX_UNITS_IN_BATTLE = 4;

        private int GetTeamSize(List<UnitInfo> team)
        {
            return Math.Min(team.Count, MAX_UNITS_IN_BATTLE);
        }

        private UnitInstance? CreateUnitInstance(UnitInfo unitInfo, char position, Dictionary<string, Unit> unitData)
        {
            if (!unitData.TryGetValue(unitInfo.Name, out var unitTemplate))
            {
                return null;
            }
            
            return new UnitInstance(
                name: unitInfo.Name,
                maxHP: unitTemplate.Stats.HealthPoints,
                maxMP: unitTemplate.Stats.ManaPoints,
                str: unitTemplate.Stats.Strength,
                skl: unitTemplate.Stats.Skill,
                spd: unitTemplate.Stats.Speed,
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
