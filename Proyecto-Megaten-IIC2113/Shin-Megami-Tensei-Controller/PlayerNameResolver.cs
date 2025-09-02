using Shin_Megami_Tensei_Model.CombatSystem.Core;

using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei
{
    public class PlayerNameResolver
    {
        private const string DEFAULT_PLAYER_1_NAME = "Player1";
        private const string DEFAULT_PLAYER_2_NAME = "Player2";
        
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
            var player1Name = GetPlayerNameFromTeam(team1, DEFAULT_PLAYER_1_NAME);
            var player2Name = GetPlayerNameFromTeam(team2, DEFAULT_PLAYER_2_NAME);
            
            return (player1Name, player2Name);
        }

        private string GetPlayerNameFromTeam(List<UnitInfo> team, string defaultName)
        {
            var samuraiName = GetSamuraiNameFromTeam(team);
            if (samuraiName != null)
                return samuraiName;
                
            var firstUnitName = GetFirstUnitNameFromTeam(team);
            return firstUnitName ?? defaultName;
        }

        private string? GetSamuraiNameFromTeam(List<UnitInfo> team)
        {
            return team.FirstOrDefault(u => u.IsSamurai)?.Name;
        }

        private string? GetFirstUnitNameFromTeam(List<UnitInfo> team)
        {
            return team.FirstOrDefault()?.Name;
        }
    }
}
