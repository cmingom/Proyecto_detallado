using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.Domain.States;

namespace Shin_Megami_Tensei
{
    public class GameSetup
    {
        private readonly Dictionary<string, Unit> unitData;
        
        public GameSetup(Dictionary<string, Unit> unitData)
        {
            this.unitData = unitData;
        }
        
        public BattleState CreateBattleState(List<Game.UnitInfo> team1, 
                                           List<Game.UnitInfo> team2)
        {
            var team1Units = CreateTeamUnits(team1);
            var team2Units = CreateTeamUnits(team2);
            
            var battleTeam1 = new TeamState(team1Units);
            var battleTeam2 = new TeamState(team2Units);
            
            return new BattleState(battleTeam1, battleTeam2);
        }
        
        private List<UnitInstance> CreateTeamUnits(List<Game.UnitInfo> team)
        {
            var units = new List<UnitInstance>();
            char[] positions = { 'A', 'B', 'C', 'D' };
            
            for (int i = 0; i < team.Count && i < 4; i++)
            {
                var unitInfo = team[i];
                
                if (unitData.TryGetValue(unitInfo.Name, out var unitTemplate))
                {
                    var unitInstance = new UnitInstance(
                        name: unitInfo.Name,
                        maxHP: unitTemplate.Stats.HP,
                        maxMP: unitTemplate.Stats.MP,
                        str: unitTemplate.Stats.Str,
                        skl: unitTemplate.Stats.Skl,
                        spd: unitTemplate.Stats.Spd,
                        isSamurai: unitInfo.IsSamurai,
                        position: positions[i],
                        skills: unitInfo.IsSamurai ? unitInfo.Skills : unitTemplate.Skills
                    );
                    
                    units.Add(unitInstance);
                }
            }
            
            return units;
        }
        
        public (string player1Name, string player2Name) GetPlayerNames(List<Game.UnitInfo> team1, 
                                                                                  List<Game.UnitInfo> team2)
        {
            var player1Name = team1.FirstOrDefault(u => u.IsSamurai)?.Name ?? team1.FirstOrDefault()?.Name ?? "Player1";
            var player2Name = team2.FirstOrDefault(u => u.IsSamurai)?.Name ?? team2.FirstOrDefault()?.Name ?? "Player2";
            
            return (player1Name, player2Name);
        }
    }
}
