using System;
using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.Domain.States
{
    public class TeamState
    {
        private readonly UnitInstance?[] unitsArray;
        public IReadOnlyList<UnitInstance?> Units { get; }

        public TeamState(IEnumerable<UnitInstance> units)
        {
            unitsArray = new UnitInstance?[4];
            foreach (var unit in units)
            {
                int index = unit.Position switch
                {
                    'A' => 0,
                    'B' => 1,
                    'C' => 2,
                    'D' => 3,
                    _ => -1
                };
                if (index >= 0 && index < unitsArray.Length)
                {
                    unitsArray[index] = unit;
                }
            }
            Units = Array.AsReadOnly(unitsArray);
        }

        public IEnumerable<UnitInstance> AliveUnits =>
            Units.Where(u => u != null && u.HP > 0).Cast<UnitInstance>();

        public void RemoveDeadUnits()
        {
            for (int i = 0; i < unitsArray.Length; i++)
            {
                var unit = unitsArray[i];
                if (unit != null && unit.HP <= 0)
                {
                    unitsArray[i] = null;
                }
            }
        }
    }
}