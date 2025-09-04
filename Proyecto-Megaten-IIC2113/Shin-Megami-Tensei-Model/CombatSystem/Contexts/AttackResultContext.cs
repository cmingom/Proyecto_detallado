using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Enums;

namespace Shin_Megami_Tensei_Model.CombatSystem.Contexts
{
    public class AttackResultContext
    {
        public GetUnitInstance Attacker { get; }
        public GetUnitInstance Target { get; }
        public int Damage { get; }
        public AttackType AttackType { get; }

        public AttackResultContext(GetUnitInstance attacker, GetUnitInstance target, int damage, AttackType attackType)
        {
            Attacker = attacker;
            Target = target;
            Damage = damage;
            AttackType = attackType;
        }
    }
}
