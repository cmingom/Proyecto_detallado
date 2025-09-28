using System.Collections.Generic;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Contexts
{
    public sealed class TeamPopulationContext
    {
        public TeamPopulationContext(List<UnitInstanceContext> units, List<UnitInfo> team, int teamSize, Dictionary<string, Unit> unitData)
        {
            Units = units;
            Team = team;
            TeamSize = teamSize;
            UnitData = unitData;
        }

        public List<UnitInstanceContext> Units { get; }
        public List<UnitInfo> Team { get; }
        public int TeamSize { get; }
        public Dictionary<string, Unit> UnitData { get; }
    }
}
