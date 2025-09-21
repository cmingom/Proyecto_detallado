namespace Shin_Megami_Tensei_Model.CombatSystem.Contexts
{
    public readonly struct TurnOutcome
    {
        public TurnOutcome(int fullConsumed, int blinkingConsumed, int blinkingGranted)
        {
            FullTurnsConsumed = fullConsumed;
            BlinkingTurnsConsumed = blinkingConsumed;
            BlinkingTurnsGranted = blinkingGranted;
        }

        public int FullTurnsConsumed { get; }
        public int BlinkingTurnsConsumed { get; }
        public int BlinkingTurnsGranted { get; }
    }
}
