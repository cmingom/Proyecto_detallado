using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Core;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class AttackExecutor
    {
        private readonly IBattleView battleView;
        private readonly TargetSelector targetSelector;
        private readonly DamageCalculator damageCalculator;

        public AttackExecutor(IBattleView battleView)
        {
            this.battleView = battleView;
            this.targetSelector = new TargetSelector(battleView);
            this.damageCalculator = new DamageCalculator();
        }

        public bool ExecutePhysicalAttack(UnitInstance attacker, BattleState battleState)
        {
            var attackContext = new AttackContext(attacker, battleState, AttackType.Physical);
            return ProcessAttack(attackContext);
        }

        public bool ExecuteGunAttack(UnitInstance attacker, BattleState battleState)
        {
            var attackContext = new AttackContext(attacker, battleState, AttackType.Gun);
            return ProcessAttack(attackContext);
        }

        private bool ProcessAttack(AttackContext attackContext)
        {
            var availableTargets = targetSelector.GetAvailableTargetsForAttack(attackContext.BattleState, attackContext.BattleState.IsPlayer1Turn);
            if (HasNoAvailableTargets(availableTargets))
                return false;

            var selectedTarget = targetSelector.SelectTargetForAttack(attackContext.Attacker, availableTargets);
            if (IsInvalidTarget(selectedTarget))
                return false;

            ExecuteAttackOnTarget(attackContext, selectedTarget);
            return true;
        }

        private bool HasNoAvailableTargets(List<UnitInstance> availableTargets)
        {
            return !availableTargets.Any();
        }

        private bool IsInvalidTarget(UnitInstance selectedTarget)
        {
            return selectedTarget == null;
        }

        private void ExecuteAttackOnTarget(AttackContext attackContext, UnitInstance selectedTarget)
        {
            var damage = damageCalculator.CalculateAttackDamage(attackContext);
            damageCalculator.ApplyDamageToTarget(selectedTarget, damage);
            ShowAttackResult(attackContext, selectedTarget, damage);
        }

        private void ShowAttackResult(AttackContext attackContext, UnitInstance target, int damage)
        {
            var isGunAttack = IsGunAttack(attackContext.AttackType);
            ShowAttackResultToView(attackContext.Attacker, target, damage, isGunAttack);
        }

        private void ShowAttackResultToView(UnitInstance attacker, UnitInstance target, int damage, bool isGunAttack)
        {
            battleView.ShowAttackResult(attacker, target, damage, isGunAttack);
        }

        private bool IsGunAttack(AttackType attackType)
        {
            return attackType == AttackType.Gun;
        }
    }
}
