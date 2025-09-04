using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Enums;

namespace Shin_Megami_Tensei_Model.CombatSystem.Contexts
{
    public class AttackResultContext
    {
        public UnitInstanceContext Attacker { get; }
        public UnitInstanceContext Target { get; }
        public int Damage { get; }
        public AttackType AttackType { get; }

        public AttackResultContext(UnitInstanceContext attacker, UnitInstanceContext target, int damage, AttackType attackType)
        {
            Attacker = attacker;
            Target = target;
            Damage = damage;
            AttackType = attackType;
        }
    }
}
