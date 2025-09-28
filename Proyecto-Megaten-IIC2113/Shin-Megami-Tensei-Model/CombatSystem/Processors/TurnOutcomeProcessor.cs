using System;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;
using Shin_Megami_Tensei_Model.CombatSystem.Enums;
using Shin_Megami_Tensei_Model.Domain.States;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class TurnOutcomeProcessor
    {
        private readonly IBattleView battleView;

        public TurnOutcomeProcessor(IBattleView battleView)
        {
            this.battleView = battleView;
        }

        public void ProcessAffinityOutcome(BattleState battleState, AffinityReaction reaction)
        {
            var outcome = CalculateOutcome(battleState, reaction);
            DisplayOutcome(battleState, outcome);
        }

        public void ProcessHealOutcome(BattleState battleState)
        {
            var outcome = CalculateHealOutcome(battleState);
            DisplayOutcome(battleState, outcome);
        }

        public void ProcessSummonOutcome(BattleState battleState)
        {
            var outcome = CalculateSummonOutcome(battleState);
            DisplayOutcome(battleState, outcome);
        }

        private void DisplayOutcome(BattleState battleState, TurnOutcome outcome)
        {
            battleState.MarkTurnConsumptionMessageShown();
            battleView.ShowTurnConsumptionWithBlinking(outcome.FullTurnsConsumed, outcome.BlinkingTurnsConsumed, outcome.BlinkingTurnsGranted);
        }

        private TurnOutcome CalculateOutcome(BattleState battleState, AffinityReaction reaction)
        {
            return reaction switch
            {
                AffinityReaction.Weak => ApplyWeakOutcome(battleState),
                AffinityReaction.Null => ApplyNullOutcome(battleState),
                AffinityReaction.Repel => ApplyConsumeAllOutcome(battleState),
                AffinityReaction.Drain => ApplyConsumeAllOutcome(battleState),
                AffinityReaction.Miss => ApplyNullOutcome(battleState),
                _ => ApplyNeutralOutcome(battleState)
            };
        }

        private TurnOutcome ApplyWeakOutcome(BattleState battleState)
        {
            var fullConsumed = 0;
            var blinkingConsumed = 0;
            var blinkingGranted = 0;

            if (battleState.FullTurns > 0)
            {
                battleState.ConsumeTurn();
                fullConsumed = 1;
                battleState.GrantBlinkingTurn();
                blinkingGranted = 1;
            }
            else if (battleState.BlinkingTurns > 0)
            {
                battleState.ConsumeBlinkingTurn();
                blinkingConsumed = 1;
            }

            return new TurnOutcome(fullConsumed, blinkingConsumed, blinkingGranted);
        }

        private TurnOutcome ApplyNullOutcome(BattleState battleState)
        {
            var blinkingConsumed = 0;
            var fullConsumed = 0;

            blinkingConsumed += ConsumeBlinkingTurns(battleState, 2, ref fullConsumed);
            return new TurnOutcome(fullConsumed, blinkingConsumed, 0);
        }

        private TurnOutcome ApplyConsumeAllOutcome(BattleState battleState)
        {
            var fullConsumed = battleState.FullTurns;
            var blinkingConsumed = battleState.BlinkingTurns;
            battleState.SetFullTurns(0);
            battleState.ResetBlinkingTurns();
            return new TurnOutcome(fullConsumed, blinkingConsumed, 0);
        }

        private TurnOutcome ApplyNeutralOutcome(BattleState battleState)
        {
            if (battleState.BlinkingTurns > 0)
            {
                battleState.ConsumeBlinkingTurn();
                return new TurnOutcome(0, 1, 0);
            }

            if (battleState.FullTurns > 0)
            {
                battleState.ConsumeTurn();
                return new TurnOutcome(1, 0, 0);
            }

            return new TurnOutcome(0, 0, 0);
        }

        private int ConsumeBlinkingTurns(BattleState battleState, int requiredBlinking, ref int fullConsumed)
        {
            var blinkingConsumed = 0;

            while (requiredBlinking > 0 && battleState.BlinkingTurns > 0)
            {
                battleState.ConsumeBlinkingTurn();
                blinkingConsumed++;
                requiredBlinking--;
            }

            while (requiredBlinking > 0 && battleState.FullTurns > 0)
            {
                battleState.ConsumeTurn();
                fullConsumed++;
                requiredBlinking--;
            }

            return blinkingConsumed;
        }

        private TurnOutcome CalculateHealOutcome(BattleState battleState)
        {
            if (battleState.BlinkingTurns > 0)
            {
                battleState.ConsumeBlinkingTurn();
                return new TurnOutcome(0, 1, 0);
            }

            if (battleState.FullTurns > 0)
            {
                battleState.ConsumeTurn();
                return new TurnOutcome(1, 0, 0);
            }

            return new TurnOutcome(0, 0, 0);
        }

        private TurnOutcome CalculateSummonOutcome(BattleState battleState)
        {
            if (battleState.BlinkingTurns > 0)
            {
                battleState.ConsumeBlinkingTurn();
                return new TurnOutcome(0, 1, 0);
            }

            if (battleState.FullTurns > 0)
            {
                battleState.ConsumeTurn();
                return new TurnOutcome(1, 0, 0);
            }

            return new TurnOutcome(0, 0, 0);
        }
    }
}
