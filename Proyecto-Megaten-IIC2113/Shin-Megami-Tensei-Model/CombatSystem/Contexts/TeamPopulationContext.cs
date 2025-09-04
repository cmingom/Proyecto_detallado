using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.Domain.Entities
{
    public class TeamPopulationContext
    {
        public List<GetUnitInstance> Units { get; }
        public List<UnitInfo> Team { get; }
        public int TeamSize { get; }
        public Dictionary<string, Unit> UnitData { get; }

        public TeamPopulationContext(List<GetUnitInstance> units, List<UnitInfo> team, int teamSize, Dictionary<string, Unit> unitData)
        {
            Units = units;
            Team = team;
            TeamSize = teamSize;
            UnitData = unitData;
        }
    }
}
