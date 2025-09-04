namespace Shin_Megami_Tensei_Model.Domain.Entities
{
    public class TeamPopulationContext
    {
        public List<UnitInstanceContext> Units { get; }
        public List<UnitInfo> Team { get; }
        public int TeamSize { get; }
        public Dictionary<string, Unit> UnitData { get; }

        public TeamPopulationContext(List<UnitInstanceContext> units, List<UnitInfo> team, int teamSize, Dictionary<string, Unit> unitData)
        {
            Units = units;
            Team = team;
            TeamSize = teamSize;
            UnitData = unitData;
        }
    }
}
