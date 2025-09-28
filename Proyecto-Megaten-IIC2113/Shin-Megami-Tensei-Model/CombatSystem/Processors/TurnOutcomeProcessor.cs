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
            DisplayOutcome(battleState, ConsumeSingleTurn(battleState));
        }

        public void ProcessSummonOutcome(BattleState battleState)
        {
            DisplayOutcome(battleState, ConsumeSingleTurn(battleState));
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
                _ => ConsumeSingleTurn(battleState)
            };
        }

        private TurnOutcome ApplyWeakOutcome(BattleState battleState)
        {
            if (battleState.FullTurns > 0)
            {
                return ConsumeFullTurnAndGrantBlinking(battleState);
            }

            if (battleState.BlinkingTurns > 0)
            {
                battleState.ConsumeBlinkingTurn();
                return new TurnOutcome(0, 1, 0);
            }

            return new TurnOutcome(0, 0, 0);
        }

        private TurnOutcome ApplyNullOutcome(BattleState battleState)
        {
            var fullConsumed = 0;
            var blinkingConsumed = ConsumeBlinkingTurns(battleState, 2, ref fullConsumed);
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

        private TurnOutcome ConsumeSingleTurn(BattleState battleState)
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

        private TurnOutcome ConsumeFullTurnAndGrantBlinking(BattleState battleState)
        {
            battleState.ConsumeTurn();
            battleState.GrantBlinkingTurn();
            return new TurnOutcome(1, 0, 1);
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
    }
}
