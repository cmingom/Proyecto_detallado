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

            // Iniciar buffer atómico para Pasar Turno
            battleView.StartActionBuffer();

            if (battleState.BlinkingTurns > 0)
            {
                battleState.ConsumeBlinkingTurn();
                blinkingTurnsConsumed = 1;
            }
            else
            {
                battleState.ConsumeTurn();
                battleState.GrantBlinkingTurn();
                fullTurnsConsumed = 1;
                blinkingTurnsGranted = 1;
            }

            battleState.MarkTurnConsumptionMessageShown();
            
            // Usar buffering atómico para pasar turno
            battleView.StartActionBuffer();
            battleView.ShowTurnConsumptionWithBlinking(fullTurnsConsumed, blinkingTurnsConsumed, blinkingTurnsGranted);
            
            // Incrementar contador del jugador después de completar la acción
            battleState.IncrementCurrentPlayerActionCounter();
            
            battleView.FlushActionBuffer();
        }
    }
}
