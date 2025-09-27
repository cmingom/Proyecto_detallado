using System;
using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_View.ConsoleLib;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Core;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;
using Shin_Megami_Tensei_Model.CombatSystem.Exceptions;

namespace Shin_Megami_Tensei
{
    public class ActionProcessor
    {
        private const int NO_TURNS_REMAINING = 0;
        private const int EMPTY_LIST_COUNT = 0;
        private const int FIRST_UNIT_INDEX = 0;
        
        private readonly BattleView battleView;
        private readonly CombatManager combatManager;

        public ActionProcessor(BattleView battleView, CombatManager combatManager)
        {
            this.battleView = battleView;
            this.combatManager = combatManager;
        }
        
        
        public bool ShouldProcessActionOrder(BattleContext battleContext, List<UnitInstanceContext> actionOrder, TeamState currentTeam)
        {
            if (battleContext.BattleState.IsBattleFinished)
            {
                return true;
            }

            while (ShouldContinueProcessingActions(battleContext))
            {
                if (battleContext.BattleState.IsBattleFinished)
                {
                    return true;
                }

                if (ShouldProcessSingleActionIteration(battleContext, actionOrder, currentTeam))
                {
                    return true;
                }
            }
            return false;
        }

        private bool ShouldContinueProcessingActions(BattleContext battleContext)
        {
            return battleContext.HasRemainingTurns() && !battleContext.IsBattleOver(combatManager);
        }

        private bool ShouldProcessSingleActionIteration(BattleContext battleContext, List<UnitInstanceContext> actionOrder, TeamState currentTeam)
        {
            if (battleContext.BattleState.IsBattleFinished)
            {
                return true;
            }

            ShowBattleStatus(battleContext, actionOrder);
            
            if (IsActionOrderEmpty(actionOrder)) 
                return false;
            
            return ShouldProcessCurrentUnit(battleContext, actionOrder, currentTeam);
        }

        private void ShowBattleStatus(BattleContext battleContext, List<UnitInstanceContext> actionOrder)
        {
            battleView.ShowBattlefield(battleContext.BattleState, battleContext.Player1Name, battleContext.Player2Name);
            battleView.ShowTurnCounters(battleContext.BattleState);
            battleView.ShowActionOrderBySpeed(actionOrder);
        }

        private bool IsActionOrderEmpty(List<UnitInstanceContext> actionOrder)
        {
            return actionOrder.Count == EMPTY_LIST_COUNT;
        }

        private bool ShouldProcessCurrentUnit(BattleContext battleContext, List<UnitInstanceContext> actionOrder, TeamState currentTeam)
        {
            // Verificar si la batalla terminó antes de procesar cualquier unidad
            if (battleContext.BattleState.IsBattleFinished)
            {
                return false;
            }

            var currentUnit = GetCurrentUnit(actionOrder);

            if (ShouldProcessSingleUnitAction(currentUnit, battleContext))
            {
                if (!battleContext.BattleState.IsBattleFinished)
                {
                    ProcessUnitTurnEnd(actionOrder, currentTeam, currentUnit);
                }

                return true;
            }

            ProcessUnitTurnEnd(actionOrder, currentTeam, currentUnit);
            return false;
        }

        private UnitInstanceContext GetCurrentUnit(List<UnitInstanceContext> actionOrder)
        {
            return actionOrder[FIRST_UNIT_INDEX];
        }

        private bool ShouldProcessSingleUnitAction(UnitInstanceContext currentUnit, BattleContext battleContext)
        {
            if (battleContext.BattleState.IsBattleFinished)
            {
                return true;
            }

            // Resetear el flag de mensaje de consumo de turnos para cada nueva accion
            battleContext.BattleState.ResetTurnConsumptionMessageFlag();
            currentUnit.OnTurnStart();

            if (IsUnitActionSuccessful(currentUnit, battleContext))
            {
                // Verificar inmediatamente si la batalla terminó (ej: por rendición)
                if (battleContext.BattleState.IsBattleFinished)
                {
                    return true;
                }
                
                EnsureTurnConsumptionForSuccessfulAction(battleContext);
                
                // Verificar si la batalla terminó después de una acción exitosa (ej: rendirse)
                if (IsBattleOver(battleContext.BattleState))
                {
                    HandleBattleEnd(battleContext);
                    return true;
                }
                
                return true;
            }

            // Solo consumir turno si no se marcó el mensaje (evita duplicación con PassTurn)
            if (!battleContext.BattleState.IsTurnConsumptionMessageShown())
            {
                combatManager.ConsumeTurn(battleContext.BattleState);
            }

            return ShouldEndBattle(battleContext);
        }

        private bool IsUnitActionSuccessful(UnitInstanceContext currentUnit, BattleContext battleContext)
        {
            var unitActionContext = new UnitActionContext(currentUnit, battleContext.BattleState, battleContext.Player1Name, battleContext.Player2Name);
            return combatManager.CanProcessUnitAction(unitActionContext);
        }

        private bool ShouldEndBattle(BattleContext battleContext)
        {
            if (IsBattleOver(battleContext.BattleState))
            {
                HandleBattleEnd(battleContext);
                return true;
            }
            return false;
        }

        private bool IsBattleOver(BattleState battleState)
        {
            return combatManager.IsBattleOver(battleState);
        }

        private void HandleBattleEnd(BattleContext battleContext)
        {
            // Solo anunciar el ganador si no se estableció previamente (ej: por rendirse)
            if (battleContext.BattleState.WinnerSide == null)
            {
                try
                {
                    AnnounceWinner(battleContext.BattleState, battleContext.Player1Name, battleContext.Player2Name);
                }
                catch (GameEndedException)
                {
                    // La excepción se maneja aquí - la batalla terminó por KO
                    // No hacer nada, la excepción ya terminó el flujo
                }
            }
        }

        private void AnnounceWinner(BattleState battleState, string player1Name, string player2Name)
        {
            var winnerName = combatManager.GetWinner(battleState, player1Name, player2Name);
            var winnerNumber = combatManager.GetWinnerNumber(battleState);
            battleView.ShowWinner(winnerName, winnerNumber);
            
            // Lanzar GameEndedException después de imprimir el ganador por KO
            throw new GameEndedException();
        }

        private void ProcessUnitTurnEnd(List<UnitInstanceContext> actionOrder, TeamState currentTeam, UnitInstanceContext currentUnit)
        {            actionOrder.RemoveAt(FIRST_UNIT_INDEX);
            if (currentTeam.AliveUnits.Contains(currentUnit))
            {
                actionOrder.Add(currentUnit);
            }

            SyncActionOrderWithTeam(actionOrder, currentTeam, currentUnit);
        }

        private void SyncActionOrderWithTeam(List<UnitInstanceContext> actionOrder, TeamState currentTeam, UnitInstanceContext actingUnit)
        {
            var aliveUnits = currentTeam.AliveUnits.ToList();
            var aliveSet = new HashSet<UnitInstanceContext>(aliveUnits);

            var insertionIndexes = new Queue<int>();
            var filteredOrder = new List<UnitInstanceContext>(actionOrder.Count);

            foreach (var unit in actionOrder)
            {
                if (aliveSet.Contains(unit))
                {
                    filteredOrder.Add(unit);
                }
                else
                {
                    insertionIndexes.Enqueue(filteredOrder.Count);
                }
            }

            actionOrder.Clear();
            actionOrder.AddRange(filteredOrder);

            foreach (var unit in aliveUnits)
            {
                if (actionOrder.Contains(unit))
                {
                    continue;
                }

                if (insertionIndexes.Count > 0)
                {
                    var index = insertionIndexes.Dequeue();
                    index = Math.Min(index, actionOrder.Count);
                    actionOrder.Insert(index, unit);
                }
                else
                {
                    var actingIndex = actionOrder.IndexOf(actingUnit);
                    if (actingIndex >= 0)
                    {
                        actionOrder.Insert(actingIndex, unit);
                    }
                    else
                    {
                        actionOrder.Add(unit);
                    }
                }
            }
        }
        


        private void EnsureTurnConsumptionForSuccessfulAction(BattleContext battleContext)
        {
            if (battleContext.BattleState.IsBattleFinished)
            {
                return;
            }

            if (battleContext.BattleState.IsTurnConsumptionMessageShown())
            {
                return;
            }

            combatManager.ConsumeTurn(battleContext.BattleState);
        }
    }
}


// to do: ojala que las funciones no retornen null. separacion por partes de lineas largas. ver bien los modificadores. las skills deben tener poliformismo






