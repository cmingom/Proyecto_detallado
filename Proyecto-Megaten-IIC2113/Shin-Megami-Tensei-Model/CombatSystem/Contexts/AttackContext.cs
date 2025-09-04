using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Enums;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class AttackContext
    {
        public UnitInstanceContext Attacker { get; }
        public BattleState BattleState { get; }
        public AttackType AttackType { get; }

        public AttackContext(UnitInstanceContext attacker, BattleState battleState, AttackType attackType)
        {
            Attacker = attacker;
            BattleState = battleState;
            AttackType = attackType;
        }
    }
}
