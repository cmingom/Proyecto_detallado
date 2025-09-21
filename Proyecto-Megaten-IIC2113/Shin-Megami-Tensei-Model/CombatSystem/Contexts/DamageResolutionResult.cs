using Shin_Megami_Tensei_Model.CombatSystem.Enums;

namespace Shin_Megami_Tensei_Model.CombatSystem.Contexts
{
    public readonly struct DamageResolutionResult
    {
        public DamageResolutionResult(
            int damageToTarget,
            int damageToAttacker,
            int targetHpAfter,
            int attackerHpAfter,
            AffinityReaction reaction,
            bool isCritical)
        {
            DamageToTarget = damageToTarget;
            DamageToAttacker = damageToAttacker;
            TargetHpAfter = targetHpAfter;
            AttackerHpAfter = attackerHpAfter;
            Reaction = reaction;
            IsCritical = isCritical;
        }

        public int DamageToTarget { get; }
        public int DamageToAttacker { get; }
        public int TargetHpAfter { get; }
        public int AttackerHpAfter { get; }
        public AffinityReaction Reaction { get; }
        public bool IsCritical { get; }
    }
}
