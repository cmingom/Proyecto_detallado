using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class AttackContext
    {
        public UnitInstance Attacker { get; }
        public BattleState BattleState { get; }
        public AttackType AttackType { get; }

        public AttackContext(UnitInstance attacker, BattleState battleState, AttackType attackType)
        {
            Attacker = attacker;
            BattleState = battleState;
            AttackType = attackType;
        }
    }

    public enum AttackType
    {
        Physical,
        Gun
    }
}
