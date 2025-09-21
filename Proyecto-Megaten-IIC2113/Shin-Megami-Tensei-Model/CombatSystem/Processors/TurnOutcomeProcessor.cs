using System;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.CombatSystem.Enums;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class TurnOutcomeProcessor
    {
        private readonly IBattleView battleView;

        public TurnOutcomeProcessor(IBattleView battleView)
        {
            this.battleView = battleView;
        }

        public void ApplyOutcome(BattleState battleState, AffinityReaction reaction)
        {
            var outcome = CalculateOutcome(battleState, reaction);
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
                AffinityReaction.Resist => ApplyNeutralOutcome(battleState),
                _ => ApplyNeutralOutcome(battleState)
            };
        }

        private TurnOutcome ApplyWeakOutcome(BattleState battleState)
        {
            int fullConsumed = 0;
            int blinkingConsumed = 0;
            int blinkingGranted = 0;

            if (battleState.FullTurns > 0)
            {
                // Al golpear Weak con al menos 1 Full Turn disponible: consumes 1 Full y obtienes 1 Blinking
                battleState.ConsumeTurn();
                fullConsumed = 1;
                battleState.GrantBlinkingTurn();
                blinkingGranted = 1;
            }
            else if (battleState.BlinkingTurns > 0)
            {
                // Si no hay Full, consumes 1 Blinking y no se crea otro
                battleState.ConsumeBlinkingTurn();
                blinkingConsumed = 1;
                blinkingGranted = 0;
            }

            return new TurnOutcome(fullConsumed, blinkingConsumed, blinkingGranted);
        }

        private TurnOutcome ApplyNullOutcome(BattleState battleState)
        {
            int blinkingConsumed = 0;
            int fullConsumed = 0;

            blinkingConsumed += ConsumeBlinkingTurns(battleState, 2, ref fullConsumed);
            return new TurnOutcome(fullConsumed, blinkingConsumed, 0);
        }

        private TurnOutcome ApplyConsumeAllOutcome(BattleState battleState)
        {
            int fullConsumed = battleState.FullTurns;
            int blinkingConsumed = battleState.BlinkingTurns;
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
            int blinkingConsumed = 0;
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


