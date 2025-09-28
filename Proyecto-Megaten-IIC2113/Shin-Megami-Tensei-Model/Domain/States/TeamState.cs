using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.Domain.States
{
    public class TeamState
    {
        // Reglas del juego: 1 Samurai + hasta 7 monstruos = máximo 8 unidades total
        // En el tablero: Samurai + primeros 3 monstruos = máximo 4 unidades activas
        // En reserva: los monstruos restantes se almacenan en la reserva con capacidad dinamica
        private const int MAX_ACTIVE_UNITS = 4; // Samurai + hasta 3 monstruos
        private const int MAX_TOTAL_MONSTERS = 7; // Maximo de monstruos permitidos en el equipo
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
        
        private readonly UnitInstanceContext?[] activeUnitsArray;
        private readonly List<UnitInstanceContext> reservesList;
        
        public IReadOnlyList<UnitInstanceContext?> ActiveUnits { get; }
        public IReadOnlyList<UnitInstanceContext> Reserves { get; }

        public TeamState(IEnumerable<UnitInstanceContext> activeUnits, IEnumerable<UnitInstanceContext> reserves)
        {
            activeUnitsArray = new UnitInstanceContext?[MAX_ACTIVE_UNITS];
            reservesList = new List<UnitInstanceContext>();
            
            PopulateActiveUnitsArray(activeUnits);
            PopulateReservesList(reserves);
            
            ActiveUnits = Array.AsReadOnly(activeUnitsArray);
            Reserves = reservesList.AsReadOnly();
        }
        
        private void PopulateActiveUnitsArray(IEnumerable<UnitInstanceContext> units)
        {
            foreach (var unit in units)
            {
                PlaceUnitInActiveArray(unit);
            }
        }

        private void PopulateReservesList(IEnumerable<UnitInstanceContext> reserves)
        {
            foreach (var reserve in reserves)
            {
                reservesList.Add(reserve);
            }
        }

        private void PlaceUnitInActiveArray(UnitInstanceContext unit)
        {
            int index = GetPositionIndex(unit.Position);
            if (IsValidIndex(index))
            {
                activeUnitsArray[index] = unit;
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
            return index >= 0 && index < activeUnitsArray.Length;
        }

        public IEnumerable<UnitInstanceContext> AliveUnits =>
            GetAliveUnitsFromCollection();

        private IEnumerable<UnitInstanceContext> GetAliveUnitsFromCollection()
        {
            return ActiveUnits.Where(IsUnitAlive).Cast<UnitInstanceContext>();
        }

        private bool IsUnitAlive(UnitInstanceContext? unit)
        {
            return unit != null && unit.HP > MINIMUM_HP;
        }



        public bool CanAddToReserves()
        {
            var activeMonsters = ActiveUnits.Count(unit => unit != null && !unit.IsSamurai);
            return Reserves.Count + activeMonsters < MAX_TOTAL_MONSTERS;
        }
        

        public void RemoveFromReserves(UnitInstanceContext unit)
        {
            reservesList.Remove(unit);
        }


        // Propiedad de compatibilidad para mantener funcionalidad existente
        public IReadOnlyList<UnitInstanceContext?> Units => ActiveUnits;

        public UnitInstanceContext? GetActiveUnitAt(char position)
        {
            int index = GetPositionIndex(position);
            if (!IsValidIndex(index))
            {
                throw new ArgumentException("Invalid position", nameof(position));
            }

            return activeUnitsArray[index];
        }

        public void SetActiveUnitAt(char position, UnitInstanceContext? unit)
        {
            int index = GetPositionIndex(position);
            if (!IsValidIndex(index))
            {
                throw new ArgumentException("Invalid position", nameof(position));
            }

            activeUnitsArray[index] = unit;
        }

        public void AddToReserves(UnitInstanceContext unit)
        {
            if (reservesList.Contains(unit) || !CanAddToReserves())
            {
                return;
            }

            var insertIndex = reservesList.FindIndex(existing => existing.OriginalOrder > unit.OriginalOrder);
            if (insertIndex >= 0)
            {
                reservesList.Insert(insertIndex, unit);
            }
            else
            {
                reservesList.Add(unit);
            }
        }
    }
}

