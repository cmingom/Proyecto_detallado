using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Enums;

namespace Shin_Megami_Tensei_Model.CombatSystem.Contexts
{
    public class AttackResultContext
    {
        public AttackResultContext(
            UnitInstanceContext attacker,
            UnitInstanceContext target,
            string actionName,
            DamageElement element,
            AffinityReaction reaction,
            int damageToTarget,
            int damageToAttacker,
            int hitNumber,
            int totalHits,
            int targetHpAfter,
            int attackerHpAfter,
            bool isCritical)
        {
            Attacker = attacker;
            Target = target;
            ActionName = actionName;
            Element = element;
            Reaction = reaction;
            DamageToTarget = damageToTarget;
            DamageToAttacker = damageToAttacker;
            HitNumber = hitNumber;
            TotalHits = totalHits;
            TargetHpAfter = targetHpAfter;
            AttackerHpAfter = attackerHpAfter;
            IsCritical = isCritical;
        }

        public UnitInstanceContext Attacker { get; }
        public UnitInstanceContext Target { get; }
        public string ActionName { get; }
        public DamageElement Element { get; }
        public AffinityReaction Reaction { get; }
        public int DamageToTarget { get; }
        public int DamageToAttacker { get; }
        public int HitNumber { get; }
        public int TotalHits { get; }
        public int TargetHpAfter { get; }
        public int AttackerHpAfter { get; }
        public bool IsCritical { get; }
        public bool IsFinalHit => HitNumber >= TotalHits;
        public bool IsFirstHit => HitNumber == 1;
    }
}
