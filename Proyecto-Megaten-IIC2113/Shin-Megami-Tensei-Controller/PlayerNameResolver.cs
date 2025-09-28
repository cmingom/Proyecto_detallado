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
            var (team1, team2) = gameService.ParseTeamsFromFile(file);
            return ResolvePlayerNames(team1, team2);
        }

        private (string player1Name, string player2Name) ResolvePlayerNames(List<UnitInfo> team1, List<UnitInfo> team2)
        {
            var player1Name = ResolvePlayerName(team1, DEFAULT_PLAYER_1_NAME);
            var player2Name = ResolvePlayerName(team2, DEFAULT_PLAYER_2_NAME);

            return (player1Name, player2Name);
        }

        private string ResolvePlayerName(List<UnitInfo> team, string defaultName)
        {
            var samuraiName = GetSamuraiName(team);
            if (samuraiName != null)
            {
                return samuraiName;
            }

            var firstUnitName = GetFirstUnitName(team);
            return firstUnitName ?? defaultName;
        }

        private static string? GetSamuraiName(List<UnitInfo> team)
        {
            return team.FirstOrDefault(unit => unit.IsSamurai)?.Name;
        }

        private static string? GetFirstUnitName(List<UnitInfo> team)
        {
            return team.FirstOrDefault()?.Name;
        }
    }
}
