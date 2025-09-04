using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;


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
            var lines = GetTeamFile(filePath);
            var (team1, team2) = ParseTeamLines(lines);
            return BuildBothTeams(team1, team2);
        }

        private string[] GetTeamFile(string filePath)
        {
            return File.ReadAllLines(filePath);
        }

        public (List<string>, List<string>) ParseTeamLines(string[] lines)
        {
            var team1 = new List<string>();
            var team2 = new List<string>();
            var parsingState = CreateParsingState();

            var context = new TeamParsingContext(team1, team2, parsingState, lines: lines);
            ProcessAllLines(context);

            return (team1, team2);
        }

        private ParsingState CreateParsingState()
        {
            return new ParsingState();
        }
        private void ProcessAllLines(TeamParsingContext context)
        {
            foreach (var rawLine in context.Lines!)
            {
                var lineContext = new TeamParsingContext(context.Team1, context.Team2, context.State, rawLine: rawLine);
                ProcessSingleLine(lineContext);
            }
        }
        
        private void ProcessSingleLine(TeamParsingContext context)
        {
            var line = context.RawLine!.Trim();
            if (IsEmptyLine(line)) return;
            
            ProcessValidLine(line, context);
        }

        private void ProcessValidLine(string line, TeamParsingContext context)
        {
            var headerResult = AnalyzeTeamHeader(line);
            if (headerResult.IsTeamHeader)
            {
                UpdateParsingState(context.State, headerResult);
                return;
            }

            AddLineToAppropriateTeam(line, context);
        }

        private bool IsEmptyLine(string line)
        {
            return line.Length == EMPTY_LINE_LENGTH;
        }

        private TeamHeaderResult AnalyzeTeamHeader(string line)
        {
            if (line.StartsWith(PLAYER_1_TEAM_HEADER, StringComparison.Ordinal))
            {
                return new TeamHeaderResult(true, true);
            }
            if (line.StartsWith(PLAYER_2_TEAM_HEADER, StringComparison.Ordinal))
            {
                return new TeamHeaderResult(true, false);
            }
            return new TeamHeaderResult(false, false);
        }

        private void UpdateParsingState(ParsingState state, TeamHeaderResult headerResult)
        {
            state.ReadingTeam1 = headerResult.IsTeam1;
            state.ReadingTeam2 = !headerResult.IsTeam1;
        }

        private void AddLineToAppropriateTeam(string line, TeamParsingContext context)
        {
            if (context.State.ReadingTeam1) context.Team1.Add(line);
            if (context.State.ReadingTeam2) context.Team2.Add(line);
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

    }
}
