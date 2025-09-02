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
            var parsingState = CreateParsingState();

            ProcessAllLines(lines, team1, team2, parsingState);

            return (team1, team2);
        }

        private ParsingState CreateParsingState()
        {
            return new ParsingState();
        }

        private void ProcessAllLines(string[] lines, List<string> team1, List<string> team2, ParsingState state)
        {
            foreach (var rawLine in lines)
            {
                ProcessSingleLine(rawLine, team1, team2, state);
            }
        }

        private void ProcessSingleLine(string rawLine, List<string> team1, List<string> team2, ParsingState state)
        {
            var line = rawLine.Trim();
            if (IsEmptyLine(line)) return;

            if (IsTeamHeader(line, out bool isTeam1))
            {
                UpdateParsingState(state, isTeam1);
                return;
            }

            AddLineToAppropriateTeam(line, team1, team2, state);
        }

        private bool IsEmptyLine(string line)
        {
            return line.Length == 0;
        }

        private bool IsTeamHeader(string line, out bool isTeam1)
        {
            if (line.StartsWith("Player 1 Team", StringComparison.Ordinal))
            {
                isTeam1 = true;
                return true;
            }
            if (line.StartsWith("Player 2 Team", StringComparison.Ordinal))
            {
                isTeam1 = false;
                return true;
            }
            isTeam1 = false;
            return false;
        }

        private void UpdateParsingState(ParsingState state, bool isTeam1)
        {
            state.ReadingTeam1 = isTeam1;
            state.ReadingTeam2 = !isTeam1;
        }

        private void AddLineToAppropriateTeam(string line, List<string> team1, List<string> team2, ParsingState state)
        {
            if (state.ReadingTeam1) team1.Add(line);
            if (state.ReadingTeam2) team2.Add(line);
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

        private class ParsingState
        {
            public bool ReadingTeam1 { get; set; }
            public bool ReadingTeam2 { get; set; }
        }
    }
}
