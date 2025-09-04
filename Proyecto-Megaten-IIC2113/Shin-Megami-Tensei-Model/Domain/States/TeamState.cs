using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.Domain.States
{
    public class TeamState
    {
        private const int MAX_TEAM_SIZE = 4;
        private const int MINIMUM_HP = 0;
        private const int POSITION_A_INDEX = 0;
        private const int POSITION_B_INDEX = 1;
        private const int POSITION_C_INDEX = 2;
        private const int POSITION_D_INDEX = 3;
        private const int INVALID_POSITION_INDEX = -1;
        private const char POSITION_A = 'A';
        private const char POSITION_B = 'B';
        private const char POSITION_C = 'C';
        private const char POSITION_D = 'D';
        
        private readonly UnitInstanceContext?[] unitsArray;
        public IReadOnlyList<UnitInstanceContext?> Units { get; }

        public TeamState(IEnumerable<UnitInstanceContext> units)
        {
            unitsArray = new UnitInstanceContext?[MAX_TEAM_SIZE];
            PopulateUnitsArray(units);
            Units = Array.AsReadOnly(unitsArray);
        }

        private void PopulateUnitsArray(IEnumerable<UnitInstanceContext> units)
        {
            foreach (var unit in units)
            {
                PlaceUnitInArray(unit);
            }
        }

        private void PlaceUnitInArray(UnitInstanceContext unit)
        {
            int index = GetPositionIndex(unit.Position);
            if (IsValidIndex(index))
            {
                unitsArray[index] = unit;
            }
        }

        private int GetPositionIndex(char position)
        {
            return position switch
            {
                POSITION_A => POSITION_A_INDEX,
                POSITION_B => POSITION_B_INDEX,
                POSITION_C => POSITION_C_INDEX,
                POSITION_D => POSITION_D_INDEX,
                _ => INVALID_POSITION_INDEX
            };
        }

        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < unitsArray.Length;
        }

        public IEnumerable<UnitInstanceContext> AliveUnits =>
            GetAliveUnitsFromCollection();

        private IEnumerable<UnitInstanceContext> GetAliveUnitsFromCollection()
        {
            return Units.Where(IsUnitAlive).Cast<UnitInstanceContext>();
        }

        private bool IsUnitAlive(UnitInstanceContext? unit)
        {
            return unit != null && unit.HP > MINIMUM_HP;
        }

    }
}