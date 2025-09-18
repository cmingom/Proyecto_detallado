using Shin_Megami_Tensei_Model.CombatSystem.Core;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei
{
    public class BattleStateFactory
    {
        // Reglas del juego: 1 Samurai + hasta 7 monstruos = máximo 8 unidades total
        // En el tablero: Samurai + primeros 3 monstruos = máximo 4 unidades activas
        // En reserva: monstruos restantes = máximo 4 unidades en reserva
        private const int MAX_ACTIVE_UNITS = 4; // Samurai + primeros 3 monstruos
        private const int MAX_RESERVE_UNITS = 4; // Máximo 4 monstruos en reserva
        private const char POSITION_A = 'A';
        private const char POSITION_B = 'B';
        private const char POSITION_C = 'C';
        private const char POSITION_D = 'D';
        private static readonly char[] TEAM_POSITIONS = { POSITION_A, POSITION_B, POSITION_C, POSITION_D };

        private readonly GameManager gameManager;

        public BattleStateFactory(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public BattleState GetBattleState(string file)
        {
            var lines = ReadFileLines(file);
            var (team1, team2) = ParseTeamsFromLines(lines);
            
            if (!AreTeamsValid(team1, team2))
            {
                return null;
            }
            
            return GetBattleStateFromValidTeams(file);
        }

        private string[] ReadFileLines(string file)
        {
            return File.ReadAllLines(file);
        }

        private (List<string> team1, List<string> team2) ParseTeamsFromLines(string[] lines)
        {
            var teamParser = new TeamParser(new UnitParser());
            return teamParser.ParseTeamLines(lines);
        }

        private bool AreTeamsValid(List<string> team1, List<string> team2)
        {
            return gameManager.AreValidTeams(team1, team2);
        }

        private BattleState GetBattleStateFromValidTeams(string file)
        {
            var (parsedTeam1, parsedTeam2) = gameManager.ParseTeamsFromFile(file);
            var unitData = gameManager.GetUnitData();
            
            var (team1ActiveUnits, team1Reserves) = GetTeamUnitsWithReserves(parsedTeam1, unitData);
            var (team2ActiveUnits, team2Reserves) = GetTeamUnitsWithReserves(parsedTeam2, unitData);
            
            var battleTeam1 = new TeamState(team1ActiveUnits, team1Reserves);
            var battleTeam2 = new TeamState(team2ActiveUnits, team2Reserves);
            
            return new BattleState(battleTeam1, battleTeam2);
        }

        private (List<UnitInstanceContext> activeUnits, List<UnitInstanceContext> reserves) GetTeamUnitsWithReserves(List<UnitInfo> team, Dictionary<string, Unit> unitData)
        {
            var activeUnits = new List<UnitInstanceContext>();
            var reserves = new List<UnitInstanceContext>();
            
            // Separar Samurai y monstruos
            var samurai = team.FirstOrDefault(u => u.IsSamurai);
            var monsters = team.Where(u => !u.IsSamurai).ToList();
            
            // El Samurai siempre va al campo de batalla (posición A)
            if (samurai != null)
            {
                var samuraiInstance = CreateUnitInstance(samurai, POSITION_A, unitData);
                if (samuraiInstance != null)
                {
                    activeUnits.Add(samuraiInstance);
                }
            }
            
            // Los primeros 3 monstruos van al campo de batalla (posiciones B, C, D)
            var activeMonstersCount = Math.Min(monsters.Count, MAX_ACTIVE_UNITS - 1); // -1 porque el Samurai ya ocupa una posición
            for (int i = 0; i < activeMonstersCount; i++)
            {
                var monsterInstance = CreateUnitInstance(monsters[i], TEAM_POSITIONS[i + 1], unitData); // +1 porque posición A es para el Samurai
                if (monsterInstance != null)
                {
                    activeUnits.Add(monsterInstance);
                }
            }
            
            // Los monstruos restantes van a las reservas (máximo 4)
            var reserveMonsters = monsters.Skip(activeMonstersCount).Take(MAX_RESERVE_UNITS).ToList();
            foreach (var monster in reserveMonsters)
            {
                var reserveInstance = CreateReserveUnitInstance(monster, unitData);
                if (reserveInstance != null)
                {
                    reserves.Add(reserveInstance);
                }
            }
            
            return (activeUnits, reserves);
        }

        private List<UnitInstanceContext> GetTeamUnits(List<UnitInfo> team, Dictionary<string, Unit> unitData)
        {
            var units = new List<UnitInstanceContext>();
            var teamSize = GetTeamSize(team);
            
            var teamContext = new TeamPopulationContext(units, team, teamSize, unitData);
            PopulateTeamUnits(teamContext);
            
            return units;
        }

        private void PopulateTeamUnits(TeamPopulationContext context)
        {
            for (int i = 0; i < context.TeamSize; i++)
            {
                AddUnitToTeam(context, i);
            }
        }

        private void PopulateReserveUnits(TeamPopulationContext context)
        {
            for (int i = 0; i < context.TeamSize; i++)
            {
                AddUnitToReserves(context, i);
            }
        }

        private void AddUnitToTeam(TeamPopulationContext context, int index)
        {
            var unitInstance = CreateUnitInstance(context.Team[index], TEAM_POSITIONS[index], context.UnitData);
            if (unitInstance != null)
            {
                context.Units.Add(unitInstance);
            }
        }

        private void AddUnitToReserves(TeamPopulationContext context, int index)
        {
            var unitInstance = CreateReserveUnitInstance(context.Team[index], context.UnitData);
            if (unitInstance != null)
            {
                context.Units.Add(unitInstance);
            }
        }

        private int GetTeamSize(List<UnitInfo> team)
        {
            return Math.Min(team.Count, MAX_ACTIVE_UNITS);
        }

        private UnitInstanceContext? CreateUnitInstance(UnitInfo unitInfo, char position, Dictionary<string, Unit> unitData)
        {
            var unitTemplate = GetUnitTemplate(unitInfo.Name, unitData);
            if (unitTemplate == null)
            {
                return null;
            }
            
            return BuildUnitInstance(unitInfo, position, unitTemplate);
        }

        private UnitInstanceContext? CreateReserveUnitInstance(UnitInfo unitInfo, Dictionary<string, Unit> unitData)
        {
            var unitTemplate = GetUnitTemplate(unitInfo.Name, unitData);
            if (unitTemplate == null)
            {
                return null;
            }
            
            // Las unidades de reserva no tienen posición específica inicialmente
            return BuildUnitInstance(unitInfo, 'R', unitTemplate); // 'R' para Reserve
        }

        private Unit? GetUnitTemplate(string unitName, Dictionary<string, Unit> unitData)
        {
            return unitData.TryGetValue(unitName, out var unitTemplate) ? unitTemplate : null;
        }

        private UnitInstanceContext BuildUnitInstance(UnitInfo unitInfo, char position, Unit unitTemplate)
        {
            return new UnitInstanceContext(
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
    }

}
