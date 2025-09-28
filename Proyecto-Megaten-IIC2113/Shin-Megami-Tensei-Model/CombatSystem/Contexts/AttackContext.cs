using System;
using Shin_Megami_Tensei_Model.CombatSystem.Enums;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.Domain.States;

namespace Shin_Megami_Tensei_Model.CombatSystem.Contexts
{
    public sealed class AttackContext
    {
        public AttackContext(UnitInstanceContext attacker, BattleState battleState, AttackType attackType)
        {
            Attacker = attacker ?? throw new ArgumentNullException(nameof(attacker));
            BattleState = battleState ?? throw new ArgumentNullException(nameof(battleState));
            AttackType = attackType;
        }

        public UnitInstanceContext Attacker { get; }
        public BattleState BattleState { get; }
        public AttackType AttackType { get; }
    }
}
