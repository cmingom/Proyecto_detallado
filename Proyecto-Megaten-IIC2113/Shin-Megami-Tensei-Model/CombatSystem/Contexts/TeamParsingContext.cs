using Shin_Megami_Tensei_Model.CombatSystem.Core;

namespace Shin_Megami_Tensei_Model.CombatSystem.Contexts
{
    public class TeamParsingContext
    {
        public string? RawLine { get; }
        public string[]? Lines { get; }
        public List<string> Team1 { get; }
        public List<string> Team2 { get; }
        public ParsingState State { get; }

        public TeamParsingContext(List<string> team1, List<string> team2, ParsingState state, string? rawLine = null, string[]? lines = null)
        {
            RawLine = rawLine;
            Lines = lines;
            Team1 = team1;
            Team2 = team2;
            State = state;
        }
    }
}
