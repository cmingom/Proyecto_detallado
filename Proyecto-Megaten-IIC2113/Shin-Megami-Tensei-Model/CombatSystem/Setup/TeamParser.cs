using System;
using System.Collections.Generic;

namespace Shin_Megami_Tensei_Model.CombatSystem.Setup
{
    public static class TeamParser
    {
        public static (List<string> Team1, List<string> Team2) ParseTeams(string[] lines)
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
    }
}