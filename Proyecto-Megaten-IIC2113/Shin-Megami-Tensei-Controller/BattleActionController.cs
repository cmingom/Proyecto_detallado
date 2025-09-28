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
    public class BattleActionController
    {
        private const int FirstUnitIndex = 0;

        private readonly BattleView battleView;
        private readonly CombatManager combatManager;

        public BattleActionController(BattleView battleView, CombatManager combatManager)
        {
            this.battleView = battleView;
            this.combatManager = combatManager;
        }

        public bool ResolveActionPhase(BattleContext battleContext, List<UnitInstanceContext> actionOrder, TeamState actingTeam)
        {
            if (IsBattleAlreadyDecided(battleContext))
            {
                return true;
            }

            while (ShouldProcessNextAction(battleContext))
            {
                if (IsBattleAlreadyDecided(battleContext))
                {
                    return true;
                }

                if (IsActionOrderEmpty(actionOrder))
                {
                    return false;
                }

                ShowBattleStatus(battleContext, actionOrder);

                var battleEnded = ProcessNextUnitTurn(battleContext, actionOrder, actingTeam);
                if (battleEnded)
                {
                    return true;
                }
            }

            return false;
        }

        private bool ShouldProcessNextAction(BattleContext battleContext)
        {
            return battleContext.HasRemainingTurns() && !battleContext.HasBattleEnded(combatManager);
        }

        private static bool IsActionOrderEmpty(ICollection<UnitInstanceContext> actionOrder)
        {
            return actionOrder.Count == 0;
        }

        private void ShowBattleStatus(BattleContext battleContext, List<UnitInstanceContext> actionOrder)
        {
            battleView.ShowBattlefield(battleContext.BattleState, battleContext.Player1Name, battleContext.Player2Name);
            battleView.ShowTurnCounters(battleContext.BattleState);
            battleView.ShowActionOrderBySpeed(actionOrder);
        }

        private bool ProcessNextUnitTurn(BattleContext battleContext, List<UnitInstanceContext> actionOrder, TeamState actingTeam)
        {
            var actingUnit = actionOrder[FirstUnitIndex];
            var battleEnded = ExecuteUnitTurn(battleContext, actingUnit);

            UpdateActionOrder(actionOrder, actingTeam, actingUnit, battleEnded);

            return battleEnded;
        }

        private bool ExecuteUnitTurn(BattleContext battleContext, UnitInstanceContext actingUnit)
        {
            if (IsBattleAlreadyDecided(battleContext))
            {
                return true;
            }

            PrepareUnitTurn(battleContext, actingUnit);

            var battleEndedDuringExecution = ExecuteUnitAction(battleContext, actingUnit);
            if (battleEndedDuringExecution)
            {
                return HandleBattleConclusion(battleContext);
            }

            ConsumeTurnIfNeeded(battleContext);

            return HandleBattleConclusion(battleContext);
        }

        private static void PrepareUnitTurn(BattleContext battleContext, UnitInstanceContext actingUnit)
        {
            battleContext.BattleState.ResetTurnConsumptionMessageFlag();
            actingUnit.OnTurnStart();
        }

        private bool ExecuteUnitAction(BattleContext battleContext, UnitInstanceContext actingUnit)
        {
            var unitActionContext = new UnitActionContext(actingUnit, battleContext.BattleState, battleContext.Player1Name, battleContext.Player2Name);
            return combatManager.ExecuteUnitTurn(unitActionContext);
        }

        private void ConsumeTurnIfNeeded(BattleContext battleContext)
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

        private bool HandleBattleConclusion(BattleContext battleContext)
        {
            if (!HasBattleEnded(battleContext.BattleState))
            {
                return false;
            }

            AnnounceWinnerIfNecessary(battleContext);
            return true;
        }

        private bool HasBattleEnded(BattleState battleState)
        {
            return combatManager.HasBattleEnded(battleState);
        }

        private void AnnounceWinnerIfNecessary(BattleContext battleContext)
        {
            if (battleContext.BattleState.WinnerSide != null)
            {
                return;
            }

            try
            {
                AnnounceWinner(battleContext.BattleState, battleContext.Player1Name, battleContext.Player2Name);
            }
            catch (GameEndedException)
            {
                // Los flujos que establecen un ganador interrumpen el bucle mediante GameEndedException.
            }
        }

        private void AnnounceWinner(BattleState battleState, string player1Name, string player2Name)
        {
            var winnerName = combatManager.GetWinner(battleState, player1Name, player2Name);
            var winnerNumber = combatManager.GetWinnerNumber(battleState);
            battleView.ShowWinner(winnerName, winnerNumber);

            throw new GameEndedException();
        }

        private void UpdateActionOrder(List<UnitInstanceContext> actionOrder, TeamState actingTeam, UnitInstanceContext actingUnit, bool battleEnded)
        {
            actionOrder.RemoveAt(FirstUnitIndex);

            if (battleEnded)
            {
                return;
            }

            var aliveUnits = actingTeam.AliveUnits.ToList();

            if (aliveUnits.Contains(actingUnit))
            {
                actionOrder.Add(actingUnit);
            }

            SynchronizeOrderWithAliveUnits(actionOrder, aliveUnits, actingUnit);
        }

        private void SynchronizeOrderWithAliveUnits(List<UnitInstanceContext> actionOrder, List<UnitInstanceContext> aliveUnits, UnitInstanceContext actingUnit)
        {
            var aliveSet = new HashSet<UnitInstanceContext>(aliveUnits);

            var filteredOrder = BuildFilteredOrder(actionOrder, aliveSet, out var vacantSlots);

            actionOrder.Clear();
            foreach (var unit in filteredOrder)
            {
                actionOrder.Add(unit);
            }

            InsertMissingUnits(actionOrder, aliveUnits, actingUnit, vacantSlots);
        }

        private static List<UnitInstanceContext> BuildFilteredOrder(IEnumerable<UnitInstanceContext> actionOrder, HashSet<UnitInstanceContext> aliveSet, out Queue<int> vacantSlots)
        {
            var filteredOrder = new List<UnitInstanceContext>();
            vacantSlots = new Queue<int>();

            foreach (var unit in actionOrder)
            {
                if (aliveSet.Contains(unit))
                {
                    filteredOrder.Add(unit);
                    continue;
                }

                vacantSlots.Enqueue(filteredOrder.Count);
            }

            return filteredOrder;
        }

        private void InsertMissingUnits(List<UnitInstanceContext> actionOrder, List<UnitInstanceContext> aliveUnits, UnitInstanceContext actingUnit, Queue<int> vacantSlots)
        {
            foreach (var unit in aliveUnits)
            {
                if (actionOrder.Contains(unit))
                {
                    continue;
                }

                InsertUnitAtAvailableSlot(actionOrder, unit, actingUnit, vacantSlots);
            }
        }

        private static void InsertUnitAtAvailableSlot(List<UnitInstanceContext> actionOrder, UnitInstanceContext unit, UnitInstanceContext actingUnit, Queue<int> vacantSlots)
        {
            if (vacantSlots.Count > 0)
            {
                var slotIndex = Math.Min(vacantSlots.Dequeue(), actionOrder.Count);
                actionOrder.Insert(slotIndex, unit);
                return;
            }

            var actingIndex = actionOrder.IndexOf(actingUnit);
            if (actingIndex >= 0)
            {
                actionOrder.Insert(actingIndex, unit);
                return;
            }

            actionOrder.Add(unit);
        }

        private bool IsBattleAlreadyDecided(BattleContext battleContext)
        {
            return battleContext.BattleState.IsBattleFinished || battleContext.HasBattleEnded(combatManager);
        }
    }
}
