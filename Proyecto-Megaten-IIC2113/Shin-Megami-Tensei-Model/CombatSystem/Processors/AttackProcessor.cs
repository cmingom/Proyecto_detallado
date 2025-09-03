using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;

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

        // verbo auxiliar
        public bool ExecutePhysicalAttack(UnitInstance attacker, BattleState battleState)
        {
            var attackContext = new AttackContext(attacker, battleState, AttackType.Physical);
            return ProcessAttack(attackContext);
        }

        // verbo auxiliar
        public bool ExecuteGunAttack(UnitInstance attacker, BattleState battleState)
        {
            var attackContext = new AttackContext(attacker, battleState, AttackType.Gun);
            return ProcessAttack(attackContext);
        }

        // verbo auxiliar
        // separar en 3
        private bool ProcessAttack(AttackContext attackContext)
        {
            var availableTargets = GetValidTargets(attackContext);
            if (HasNoAvailableTargets(availableTargets))
                return false;

            var selectedTarget = SelectTarget(attackContext, availableTargets);
            if (IsInvalidTarget(selectedTarget))
                return false;

            ExecuteAttackOnTarget(attackContext, selectedTarget);
            return true;
        }

        private List<UnitInstance> GetValidTargets(AttackContext attackContext)
        {
            return targetSelector.GetAvailableTargetsForAttack(attackContext.BattleState, attackContext.BattleState.IsPlayer1Turn);
        }

        // get
        private UnitInstance SelectTarget(AttackContext attackContext, List<UnitInstance> availableTargets)
        {
            return targetSelector.SelectTargetForAttack(attackContext.Attacker, availableTargets);
        }

        private bool HasNoAvailableTargets(List<UnitInstance> availableTargets)
        {
            return !availableTargets.Any();
        }

        private bool IsInvalidTarget(UnitInstance selectedTarget)
        {
            return selectedTarget == null;
        }

        // hace dos cosas
        private void ExecuteAttackOnTarget(AttackContext attackContext, UnitInstance selectedTarget)
        {
            var damage = damageCalculator.CalculateAttackDamage(attackContext);
            damageCalculator.ApplyDamageToTarget(selectedTarget, damage);
            ShowAttackResult(attackContext, selectedTarget, damage); // separar
        }

        private void ShowAttackResult(AttackContext attackContext, UnitInstance target, int damage)
        {
            var isGunAttack = IsGunAttack(attackContext.AttackType);
            ShowAttackResultToView(attackContext.Attacker, target, damage, isGunAttack);
        }

        // recibe 4 y un bool
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
