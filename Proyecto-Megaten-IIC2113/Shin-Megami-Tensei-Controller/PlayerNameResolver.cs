using System.Collections.Generic;
using Shin_Megami_Tensei_Model.CombatSystem.Core;

using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei
{
    public class PlayerNameResolver
    {
        private readonly GameManager gameService;

        public PlayerNameResolver(GameManager gameService)
        {
            this.gameService = gameService;
        }

        public (string player1Name, string player2Name) GetPlayerNames(string file)
        {
            var (parsedTeam1, parsedTeam2) = ParseTeamsFromFile(file);
            
            return GetPlayerNamesFromTeams(parsedTeam1, parsedTeam2);
        }

        private (List<UnitInfo> team1, List<UnitInfo> team2) ParseTeamsFromFile(string file)
        {
            return gameService.ParseTeamsFromFile(file);
        }

        private (string player1Name, string player2Name) GetPlayerNamesFromTeams(List<UnitInfo> team1, List<UnitInfo> team2)
        {
            var player1Name = team1.FirstOrDefault(u => u.IsSamurai)?.Name ?? team1.FirstOrDefault()?.Name ?? "Player1";
            var player2Name = team2.FirstOrDefault(u => u.IsSamurai)?.Name ?? team2.FirstOrDefault()?.Name ?? "Player2";
            
            return (player1Name, player2Name);
        }
    }
}
