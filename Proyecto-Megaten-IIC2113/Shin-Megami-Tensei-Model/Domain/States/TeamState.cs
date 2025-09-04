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
        
        private readonly GetUnitInstance?[] unitsArray;
        public IReadOnlyList<GetUnitInstance?> Units { get; }

        public TeamState(IEnumerable<GetUnitInstance> units)
        {
            unitsArray = new GetUnitInstance?[MAX_TEAM_SIZE];
            PopulateUnitsArray(units);
            Units = Array.AsReadOnly(unitsArray);
        }

        private void PopulateUnitsArray(IEnumerable<GetUnitInstance> units)
        {
            foreach (var unit in units)
            {
                PlaceUnitInArray(unit);
            }
        }

        private void PlaceUnitInArray(GetUnitInstance getUnit)
        {
            int index = GetPositionIndex(getUnit.Position);
            if (IsValidIndex(index))
            {
                unitsArray[index] = getUnit;
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

        public IEnumerable<GetUnitInstance> AliveUnits =>
            GetAliveUnitsFromCollection();

        private IEnumerable<GetUnitInstance> GetAliveUnitsFromCollection()
        {
            return Units.Where(IsUnitAlive).Cast<GetUnitInstance>();
        }

        private bool IsUnitAlive(GetUnitInstance? unit)
        {
            return unit != null && unit.HP > MINIMUM_HP;
        }

    }
}