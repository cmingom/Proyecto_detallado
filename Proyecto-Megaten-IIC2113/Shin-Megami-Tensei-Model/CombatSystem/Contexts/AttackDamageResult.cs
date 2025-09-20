using Shin_Megami_Tensei_Model.CombatSystem.Enums;

namespace Shin_Megami_Tensei_Model.CombatSystem.Contexts
{
    public readonly struct AttackDamageResult
    {
        public int Damage { get; }
        public AffinityReaction Reaction { get; }

        public AttackDamageResult(int damage, AffinityReaction reaction)
        {
            Damage = damage;
            Reaction = reaction;
        }
    }
}
