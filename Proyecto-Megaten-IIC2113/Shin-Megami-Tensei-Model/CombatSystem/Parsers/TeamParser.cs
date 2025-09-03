using Shin_Megami_Tensei_Model.Domain.Entities;


namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class TeamParser
    {
        private const int EMPTY_LINE_LENGTH = 0;
        private const string PLAYER_1_TEAM_HEADER = "Player 1 Team";
        private const string PLAYER_2_TEAM_HEADER = "Player 2 Team";
        
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

        // poner get
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
        // recibe 4

        private void ProcessAllLines(string[] lines, List<string> team1, List<string> team2, ParsingState state)
        {
            foreach (var rawLine in lines)
            {
                ProcessSingleLine(rawLine, team1, team2, state);
            }
        }
        
// recibe 4
// encapsular validaciones en un metodo (separar en 2)
        private void ProcessSingleLine(string rawLine, List<string> team1, List<string> team2, ParsingState state)
        {
            var line = rawLine.Trim();
            if (IsEmptyLine(line)) return;
            
// recibe bool
// no se puede usar out
            if (IsTeamHeader(line, out bool isTeam1))
            {
                UpdateParsingState(state, isTeam1);
                return;
            }

            AddLineToAppropriateTeam(line, team1, team2, state);
        }

        private bool IsEmptyLine(string line)
        {
            return line.Length == EMPTY_LINE_LENGTH;
        }

        // bool y out
        private bool IsTeamHeader(string line, out bool isTeam1)
        {
            if (line.StartsWith(PLAYER_1_TEAM_HEADER, StringComparison.Ordinal))
            {
                isTeam1 = true;
                return true;
            }
            if (line.StartsWith(PLAYER_2_TEAM_HEADER, StringComparison.Ordinal))
            {
                isTeam1 = false;
                return true;
            }
            isTeam1 = false;
            return false;
        }

        // recibe bool
        private void UpdateParsingState(ParsingState state, bool isTeam1)
        {
            state.ReadingTeam1 = isTeam1;
            state.ReadingTeam2 = !isTeam1;
        }

        // recibe 4
        private void AddLineToAppropriateTeam(string line, List<string> team1, List<string> team2, ParsingState state)
        {
            if (state.ReadingTeam1) team1.Add(line);
            if (state.ReadingTeam2) team2.Add(line);
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

        private (List<UnitInfo>, List<UnitInfo>) BuildBothTeams(List<string> team1, List<string> team2)
        {
            var parsedTeam1 = BuildUnitInfoList(team1);
            var parsedTeam2 = BuildUnitInfoList(team2);
            return (parsedTeam1, parsedTeam2);
        }

        // archivo aparte
        private class ParsingState
        {
            public bool ReadingTeam1 { get; set; }
            public bool ReadingTeam2 { get; set; }
        }
    }
}
