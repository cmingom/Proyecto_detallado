using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class PassTurnProcessor
    {
        private readonly IBattleView battleView;

        public PassTurnProcessor(IBattleView battleView)
        {
            this.battleView = battleView;
        }

        public void ProcessPassTurn(BattleState battleState)
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
