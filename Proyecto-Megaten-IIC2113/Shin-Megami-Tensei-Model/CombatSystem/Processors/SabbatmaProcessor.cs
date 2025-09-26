using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.CombatSystem.Exceptions;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class SabbatmaProcessor
    {
        private const int INVALID_CHOICE = -1;
        private const int CANCEL_CHOICE_OFFSET = 1;
        private static readonly char[] SAMURAI_POSITIONS = { 'B', 'C', 'D' };

        private readonly IBattleView battleView;
        private readonly TurnOutcomeProcessor turnOutcomeProcessor;

        public SabbatmaProcessor(IBattleView battleView, TurnOutcomeProcessor turnOutcomeProcessor)
        {
            this.battleView = battleView;
            this.turnOutcomeProcessor = turnOutcomeProcessor;
        }

        public bool CanProcessSabbatma(UnitInstanceContext summoner, BattleState battleState)
        {
            if (battleState.IsBattleFinished)
            {
                return false;
            }

            var currentTeam = battleState.GetCurrentTeam();
            var availableUnits = GetAvailableAliveUnitsFromReserve(currentTeam); // Solo vivas

            if (!availableUnits.Any())
            {
                return false;
            }

            battleView.ShowSummonMenu(availableUnits);
            var unitChoice = battleView.GetSummonChoice(availableUnits.Count + CANCEL_CHOICE_OFFSET);

            if (IsInvalidChoice(unitChoice) || IsUnitChoiceCancelled(unitChoice, availableUnits.Count))
            {
                return false;
            }

            var selectedUnit = availableUnits[unitChoice - 1];

            try
            {
                return summoner.IsSamurai
                    ? ProcessSamuraiSabbatma(selectedUnit, currentTeam, battleState, summoner)
                    : ProcessMonsterSabbatma(summoner, selectedUnit, currentTeam, battleState);
            }
            catch (ActionCancelledException)
            {
                return false;
            }
        }

        public bool CanProcessInvitation(UnitInstanceContext summoner, UnitInstanceContext target, BattleState battleState)
        {
            if (battleState.IsBattleFinished)
            {
                return false;
            }

            var currentTeam = battleState.GetCurrentTeam();
            var availableUnits = GetAvailableUnitsFromReserve(currentTeam);

            // Invitation: elegir unidad de reserva (viva o KO)
            battleView.ShowSummonMenu(availableUnits);
            var unitChoice = battleView.GetSummonChoice(availableUnits.Count + CANCEL_CHOICE_OFFSET);

            if (IsInvalidChoice(unitChoice) || IsUnitChoiceCancelled(unitChoice, availableUnits.Count))
            {
                return false;
            }

            if (!availableUnits.Any())
            {
                // No hay candidatos, solo mostrar "Cancelar" y no ejecutar
                return false;
            }

            var selectedUnit = availableUnits[unitChoice - 1];

            // Si el elegido está KO, se revive primero
            if (selectedUnit.HP <= 0)
            {
                selectedUnit.HP = selectedUnit.MaxHP;
                battleView.ShowReviveResult(summoner, selectedUnit, "Invitation");
            }

            try
            {
                return summoner.IsSamurai
                    ? ProcessSamuraiSabbatma(selectedUnit, currentTeam, battleState, summoner)
                    : ProcessMonsterSabbatma(summoner, selectedUnit, currentTeam, battleState);
            }
            catch (ActionCancelledException)
            {
                return false;
            }
        }

        private List<UnitInstanceContext> GetAvailableUnitsFromReserve(TeamState team)
        {
            return team.Reserves.ToList();
        }

        private List<UnitInstanceContext> GetAvailableAliveUnitsFromReserve(TeamState team)
        {
            return team.Reserves.Where(unit => unit.HP > 0).ToList();
        }

        private bool IsInvalidChoice(int choice)
        {
            return choice == INVALID_CHOICE;
        }

        private bool IsUnitChoiceCancelled(int choice, int availableUnitsCount)
        {
            return choice == availableUnitsCount + CANCEL_CHOICE_OFFSET;
        }

        private bool ProcessSamuraiSabbatma(UnitInstanceContext selectedUnit, TeamState currentTeam, BattleState battleState, UnitInstanceContext summoner)
        {
            var positionOptions = GetSamuraiPositionOptions(currentTeam);
            battleView.ShowSummonPositionMenu(positionOptions);
            var positionChoice = battleView.GetSummonPositionChoice(positionOptions.Count + CANCEL_CHOICE_OFFSET);

            if (IsInvalidChoice(positionChoice) || IsPositionChoiceCancelled(positionChoice, positionOptions.Count))
            {
                throw new ActionCancelledException();
            }

            var (slot, existingUnit) = positionOptions[positionChoice - 1];
            PerformSamuraiSabbatma(selectedUnit, existingUnit, slot, currentTeam, battleState, summoner);
            return true;
        }

        private bool ProcessMonsterSabbatma(UnitInstanceContext summoner, UnitInstanceContext selectedUnit, TeamState currentTeam, BattleState battleState)
        {
            // Monstruo: se intercambia por el invocado
            var summonerSlot = GetSummonerSlot(summoner, currentTeam);
            if (summonerSlot == '\0')
            {
                return false;
            }

            PerformMonsterSabbatma(summoner, selectedUnit, summonerSlot, currentTeam, battleState);
            return true;
        }

        private List<(char Slot, UnitInstanceContext? Unit)> GetSamuraiPositionOptions(TeamState currentTeam)
        {
            var options = new List<(char Slot, UnitInstanceContext? Unit)>();
            foreach (var slot in SAMURAI_POSITIONS)
            {
                var unit = currentTeam.GetActiveUnitAt(slot);
                options.Add((slot, unit));
            }
            return options;
        }

        private bool IsPositionChoiceCancelled(int choice, int positionOptionsCount)
        {
            return choice == positionOptionsCount + CANCEL_CHOICE_OFFSET;
        }

        private void PerformSamuraiSabbatma(UnitInstanceContext selectedUnit, UnitInstanceContext? existingUnit, char slot, TeamState currentTeam, BattleState battleState, UnitInstanceContext summoner)
        {
            // Samurai: invoca a un puesto vacío o reemplaza a otro monstruo
            currentTeam.SetActiveUnitAt(slot, selectedUnit);
            currentTeam.RemoveFromReserves(selectedUnit);

            if (existingUnit != null)
            {
                currentTeam.AddToReserves(existingUnit);
            }

            battleView.ShowSummonResult(selectedUnit.Name);
            
            // Aplicar reglas de turnos para Sabbatma (igual que Invocar)
            turnOutcomeProcessor.ApplySummonTurnOutcome(battleState);
            
            // Incrementar contador de habilidades del jugador después de usar cualquier habilidad
            battleState.IncrementCurrentPlayerSkillCounter();
        }

        private void PerformMonsterSabbatma(UnitInstanceContext summoner, UnitInstanceContext selectedUnit, char summonerSlot, TeamState currentTeam, BattleState battleState)
        {
            // Monstruo: se intercambia por el invocado
            currentTeam.SetActiveUnitAt(summonerSlot, selectedUnit);
            currentTeam.RemoveFromReserves(selectedUnit);
            currentTeam.AddToReserves(summoner);

            battleView.ShowSummonResult(selectedUnit.Name);
            
            // Aplicar reglas de turnos para Sabbatma (igual que Invocar)
            turnOutcomeProcessor.ApplySummonTurnOutcome(battleState);
            
            // Incrementar contador de habilidades del jugador después de usar cualquier habilidad
            battleState.IncrementCurrentPlayerSkillCounter();
        }

        private char GetSummonerSlot(UnitInstanceContext summoner, TeamState currentTeam)
        {
            foreach (var slot in SAMURAI_POSITIONS)
            {
                var unit = currentTeam.GetActiveUnitAt(slot);
                if (unit == summoner)
                {
                    return slot;
                }
            }
            return '\0';
        }
    }
}
