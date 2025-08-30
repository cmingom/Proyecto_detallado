using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.CombatSystem.Core;
using System;

namespace Shin_Megami_Tensei
{
    public class GameSetup
    {
        private readonly Dictionary<string, Unit> unitData;
        
        public GameSetup(Dictionary<string, Unit> unitData)
        {
            this.unitData = unitData;
        }
        
        public BattleState CreateBattleState(List<UnitInfo> team1, List<UnitInfo> team2)
        {
            var team1Units = CreateTeamUnits(team1);
            var team2Units = CreateTeamUnits(team2);
            
            var battleTeam1 = new TeamState(team1Units);
            var battleTeam2 = new TeamState(team2Units);
            
            return new BattleState(battleTeam1, battleTeam2);
        }
        
        private List<UnitInstance> CreateTeamUnits(List<UnitInfo> team)
        {
            var units = new List<UnitInstance>();
            var positions = GetTeamPositions();
            var teamSize = GetTeamSize(team);
            
            PopulateTeamUnits(units, team, positions, teamSize);
            
            return units;
        }
        
        private void PopulateTeamUnits(List<UnitInstance> units, List<UnitInfo> team, char[] positions, int teamSize)
        {
            for (int i = 0; i < teamSize; i++)
            {
                var unitInstance = CreateUnitInstance(team[i], positions[i]);
                if (unitInstance != null)
                {
                    units.Add(unitInstance);
                }
            }
        }
        
        private char[] GetTeamPositions()
        {
            return new char[] { 'A', 'B', 'C', 'D' };
        }
        
        private int GetTeamSize(List<UnitInfo> team)
        {
            return Math.Min(team.Count, 4);
        }
        
        private UnitInstance? CreateUnitInstance(UnitInfo unitInfo, char position)
        {
            if (!unitData.TryGetValue(unitInfo.Name, out var unitTemplate))
            {
                return null;
            }
            
            return new UnitInstance(
                name: unitInfo.Name,
                maxHP: unitTemplate.Stats.HP,
                maxMP: unitTemplate.Stats.MP,
                str: unitTemplate.Stats.Str,
                skl: unitTemplate.Stats.Skl,
                spd: unitTemplate.Stats.Spd,
                isSamurai: unitInfo.IsSamurai,
                position: position,
                skills: GetUnitSkills(unitInfo, unitTemplate)
            );
        }
        
        private List<string> GetUnitSkills(UnitInfo unitInfo, Unit unitTemplate)
        {
            return unitInfo.IsSamurai ? unitInfo.Skills : unitTemplate.Skills;
        }
        
        public (string player1Name, string player2Name) GetPlayerNames(List<UnitInfo> team1, List<UnitInfo> team2)
        {
            var player1Name = team1.FirstOrDefault(u => u.IsSamurai)?.Name ?? team1.FirstOrDefault()?.Name ?? "Player1";
            var player2Name = team2.FirstOrDefault(u => u.IsSamurai)?.Name ?? team2.FirstOrDefault()?.Name ?? "Player2";
            
            return (player1Name, player2Name);
        }
    }
}
