using System;
using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.Domain.States
{
    public class TeamState
    {
        private const int MAX_TEAM_SIZE = 4;
        private readonly UnitInstance?[] unitsArray;
        public IReadOnlyList<UnitInstance?> Units { get; }

        public TeamState(IEnumerable<UnitInstance> units)
        {
            unitsArray = new UnitInstance?[MAX_TEAM_SIZE];
            PopulateUnitsArray(units);
            Units = Array.AsReadOnly(unitsArray);
        }

        private void PopulateUnitsArray(IEnumerable<UnitInstance> units)
        {
            foreach (var unit in units)
            {
                PlaceUnitInArray(unit);
            }
        }

        private void PlaceUnitInArray(UnitInstance unit)
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
                'A' => 0,
                'B' => 1,
                'C' => 2,
                'D' => 3,
                _ => -1
            };
        }

        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < unitsArray.Length;
        }

        public IEnumerable<UnitInstance> AliveUnits =>
            GetAliveUnitsFromCollection();

        private IEnumerable<UnitInstance> GetAliveUnitsFromCollection()
        {
            return Units.Where(IsUnitAlive).Cast<UnitInstance>();
        }

        private bool IsUnitAlive(UnitInstance? unit)
        {
            return unit != null && unit.HP > 0;
        }

        public void RemoveDeadUnits()
        {
            ProcessAllUnits();
        }

        private void ProcessAllUnits()
        {
            for (int i = 0; i < unitsArray.Length; i++)
            {
                ProcessUnitAtIndex(i);
            }
        }

        private void ProcessUnitAtIndex(int index)
        {
            var unit = unitsArray[index];
            if (IsUnitDead(unit))
            {
                unitsArray[index] = null;
            }
        }

        private bool IsUnitDead(UnitInstance? unit)
        {
            return unit != null && unit.HP <= 0;
        }
    }
}