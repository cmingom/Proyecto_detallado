using System.Collections.Generic;
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

        // Constructor para procesar una línea individual
        public TeamParsingContext(string rawLine, List<string> team1, List<string> team2, ParsingState state)
        {
            RawLine = rawLine;
            Team1 = team1;
            Team2 = team2;
            State = state;
        }

        // Constructor para procesar múltiples líneas
        public TeamParsingContext(string[] lines, List<string> team1, List<string> team2, ParsingState state)
        {
            Lines = lines;
            Team1 = team1;
            Team2 = team2;
            State = state;
        }

        // Propiedad para determinar si estamos procesando una línea o múltiples líneas
        public bool IsSingleLine => RawLine != null;
        public bool IsMultipleLines => Lines != null;
    }
}
