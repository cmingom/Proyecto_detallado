using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;
using Shin_Megami_Tensei_Model.CombatSystem.Exceptions;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class SummonProcessor
    {
        private readonly IBattleView battleView;

        public SummonProcessor(IBattleView battleView)
        {
            this.battleView = battleView;
        }

        public bool CanProcessSummon(UnitInstanceContext summoner, BattleState battleState)
        {
            var currentTeam = battleState.GetCurrentTeam();
            var availableUnits = GetAvailableUnitsFromReserve(currentTeam);
            
            battleView.ShowSummonMenu(availableUnits);
            var unitChoice = battleView.GetSummonChoice(availableUnits.Count + 1);
            
            if (IsUnitChoiceCancelled(unitChoice, availableUnits.Count) || availableUnits.Count == 0)
            {
                return false;
            }

            var selectedUnit = availableUnits[unitChoice - 1];

            if (summoner.IsSamurai)
            {
                return ProcessSamuraiSummon(selectedUnit, currentTeam, battleState);
            }
            else
            {
                return ProcessMonsterSummon(summoner, selectedUnit, currentTeam, battleState);
            }
        }

        private List<UnitInstanceContext> GetAvailableUnitsFromReserve(TeamState team)
        {
            return team.Reserves.Where(unit => unit.HP > 0).ToList();
        }


        private int GetUnitChoice(int unitCount)
        {
            return battleView.GetActionChoice(unitCount + 1); // +1 por Cancelar
        }

        private bool IsUnitChoiceCancelled(int choice, int unitCount)
        {
            return choice == unitCount + 1; // Última opción es Cancelar
        }

        private bool ProcessSamuraiSummon(UnitInstanceContext unitToSummon, TeamState team, BattleState battleState)
        {
            ShowPositionMenu(team);
            var positionChoice = GetPositionChoice();
            
            if (IsPositionChoiceCancelled(positionChoice))
                throw new ActionCancelledException();

            var targetPosition = positionChoice + 1; // Posiciones 2-4 (índices 1-3)
            
            if (team.Units[targetPosition] == null)
            {
                // Puesto vacío - colocar directamente
                PlaceUnitInPosition(unitToSummon, team, targetPosition);
            }
            else
            {
                // Puesto ocupado - intercambiar
                SwapUnits(unitToSummon, team, targetPosition);
            }

            ShowSummonMessage(unitToSummon.Name);
            ConsumeTurnsForSummon(battleState);
            return true;
        }

        private bool ProcessMonsterSummon(UnitInstanceContext summoner, UnitInstanceContext unitToSummon, TeamState team, BattleState battleState)
        {
            var summonerPosition = GetUnitPosition(summoner, team);
            
            // Intercambio automático: invocador sale, invocado entra en la misma posición
            SwapUnits(unitToSummon, team, summonerPosition);
            
            ShowSummonMessage(unitToSummon.Name);
            ConsumeTurnsForSummon(battleState);
            return true;
        }

        private void ShowPositionMenu(TeamState team)
        {
            // Mostrar posiciones 2-4 (B, C, D) con estado actual
        }

        private int GetPositionChoice()
        {
            return battleView.GetActionChoice(3); // 3 posiciones disponibles (2-4)
        }

        private bool IsPositionChoiceCancelled(int choice)
        {
            return choice == -1; // O el valor que indique cancelar
        }

        private void PlaceUnitInPosition(UnitInstanceContext unit, TeamState team, int position)
        {
            team.RemoveFromReserves(unit);
            // Usar método de TeamState para colocar unidad
        }

        private void SwapUnits(UnitInstanceContext unitToSummon, TeamState team, int position)
        {
            var currentUnit = team.Units[position];
            
            // Remover de reserva el que va a entrar
            team.RemoveFromReserves(unitToSummon);
            
            // Enviar el que estaba a reserva (si no es samurai)
            if (currentUnit != null && !currentUnit.IsSamurai)
            {
                // Usar métodos de TeamState para manejar intercambio
            }
        }

        private int GetUnitPosition(UnitInstanceContext unit, TeamState team)
        {
            for (int i = 0; i < team.Units.Count; i++)
            {
                if (team.Units[i] == unit)
                    return i;
            }
            return -1;
        }

        private void ShowSummonMessage(string unitName)
        {
            // Mostrar mensaje "X ha sido invocado"
        }

        private void ConsumeTurnsForSummon(BattleState battleState)
        {
            int fullTurnsConsumed = 0;
            int blinkingTurnsConsumed = 0;
            int blinkingTurnsGranted = 0;

            if (battleState.BlinkingTurns > 0)
            {
                // Consume 1 Blinking Turn si hay disponible
                battleState.ConsumeBlinkingTurn();
                blinkingTurnsConsumed = 1;
            }
            else
            {
                // Si no hay Blinking, consume 1 Full Turn y otorga 1 Blinking Turn
                battleState.ConsumeTurn();
                battleState.GrantBlinkingTurn();
                fullTurnsConsumed = 1;
                blinkingTurnsGranted = 1;
            }

            // Mostrar resumen de turnos
            battleView.ShowTurnConsumptionWithBlinking(fullTurnsConsumed, blinkingTurnsConsumed, blinkingTurnsGranted);
        }
    }
}
