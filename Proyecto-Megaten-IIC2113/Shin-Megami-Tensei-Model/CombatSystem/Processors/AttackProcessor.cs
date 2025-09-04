using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Enums;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class AttackProcessor
    {
        private readonly IBattleView battleView;
        private readonly TargetSelector targetSelector;
        private readonly DamageCalculator damageCalculator;

        public AttackProcessor(IBattleView battleView)
        {
            this.battleView = battleView;
            this.targetSelector = new TargetSelector(battleView);
            this.damageCalculator = new DamageCalculator();
        }

        public bool CanExecutePhysicalAttack(GetUnitInstance attacker, BattleState battleState)
        {
            var attackContext = new AttackContext(attacker, battleState, AttackType.Physical);
            return CanProcessAttack(attackContext);
        }

        public bool CanExecuteGunAttack(GetUnitInstance attacker, BattleState battleState)
        {
            var attackContext = new AttackContext(attacker, battleState, AttackType.Gun);
            return CanProcessAttack(attackContext);
        }

        // ver como separarlo
        private bool CanProcessAttack(AttackContext attackContext)
        {
            if (!CanSelectValidTarget(attackContext))
                return false;

            var selectedTarget = SelectTargetForAttack(attackContext);
            if (IsInvalidTarget(selectedTarget))
                return false;

            ExecuteAttackOnTarget(attackContext, selectedTarget);
            return true;
        }

        private bool CanSelectValidTarget(AttackContext attackContext)
        {
            var availableTargets = GetValidTargets(attackContext);
            return !HasNoAvailableTargets(availableTargets);
        }

        private GetUnitInstance SelectTargetForAttack(AttackContext attackContext)
        {
            var availableTargets = GetValidTargets(attackContext);
            return GetTarget(attackContext, availableTargets);
        }

        private List<GetUnitInstance> GetValidTargets(AttackContext attackContext)
        {
            return targetSelector.GetAvailableTargetsForAttack(attackContext.BattleState);
        }

        private GetUnitInstance GetTarget(AttackContext attackContext, List<GetUnitInstance> availableTargets)
        {
            return targetSelector.SelectTargetForAttack(attackContext.Attacker, availableTargets);
        }

        private bool HasNoAvailableTargets(List<GetUnitInstance> availableTargets)
        {
            return !availableTargets.Any();
        }

        private bool IsInvalidTarget(GetUnitInstance selectedTarget)
        {
            return selectedTarget == null;
        }

        private void ExecuteAttackOnTarget(AttackContext attackContext, GetUnitInstance selectedTarget)
        {
            var damage = CalculateAndApplyDamage(attackContext, selectedTarget);
            ShowAttackResult(attackContext, selectedTarget, damage);
        }

        private int CalculateAndApplyDamage(AttackContext attackContext, GetUnitInstance selectedTarget)
        {
            var damage = damageCalculator.GetCalculatedAttackDamage(attackContext);
            damageCalculator.ApplyDamageToTarget(selectedTarget, damage);
            return damage;
        }

        private void ShowAttackResult(AttackContext attackContext, GetUnitInstance target, int damage)
        {
            var attackResultContext = new AttackResultContext(attackContext.Attacker, target, damage, attackContext.AttackType);
            battleView.ShowAttackResult(attackResultContext);
        }
        
    }
}
