using System.Collections.Generic;
using System.IO;
using Shin_Megami_Tensei_Model.Domain.Entities;


namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class TeamParser
    {
        private readonly UnitParser unitParser;

        public TeamParser(UnitParser unitParser)
        {
            this.unitParser = unitParser;
        }

        public (List<UnitInfo>, List<UnitInfo>) ParseTeamsFromFile(string filePath)
        {
            var lines = ReadTeamFile(filePath);
            var (team1, team2) = ParseTeamLines(lines);
            return BuildBothTeams(team1, team2);
        }

        private string[] ReadTeamFile(string filePath)
        {
            return File.ReadAllLines(filePath);
        }

        public (List<string>, List<string>) ParseTeamLines(string[] lines)
        {
            var team1 = new List<string>();
            var team2 = new List<string>();
            bool reading1 = false;
            bool reading2 = false;

            foreach (var raw in lines)
            {
                var line = raw.Trim();
                if (line.Length == 0) continue;

                if (line.StartsWith("Player 1 Team", StringComparison.Ordinal))
                {
                    reading1 = true;
                    reading2 = false;
                    continue;
                }
                if (line.StartsWith("Player 2 Team", StringComparison.Ordinal))
                {
                    reading1 = false;
                    reading2 = true;
                    continue;
                }

                if (reading1) team1.Add(line);
                if (reading2) team2.Add(line);
            }

            return (team1, team2);
        }

        private (List<UnitInfo>, List<UnitInfo>) BuildBothTeams(List<string> team1, List<string> team2)
        {
            var parsedTeam1 = BuildUnitInfoList(team1);
            var parsedTeam2 = BuildUnitInfoList(team2);
            return (parsedTeam1, parsedTeam2);
        }

        public List<UnitInfo> BuildUnitInfoList(List<string> teamLines)
        {
            var units = new List<UnitInfo>();
            foreach (var line in teamLines)
            {
                AddValidUnitInfo(units, line);
            }
            return units;
        }

        private void AddValidUnitInfo(List<UnitInfo> units, string line)
        {
            var unitInfo = unitParser.ParseUnitDefinition(line);
            if (unitInfo != null)
            {
                units.Add(unitInfo);
            }
        }
    }
}
