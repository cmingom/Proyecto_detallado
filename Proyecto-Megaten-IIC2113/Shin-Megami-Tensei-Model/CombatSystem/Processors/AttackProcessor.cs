using Shin_Megami_Tensei_Model.CombatSystem.Contexts;
using Shin_Megami_Tensei_Model.CombatSystem.Enums;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.Domain.States;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class AttackProcessor
    {
        private const string PhysicalAttackName = "Atacar";
        private const string GunAttackName = "Disparar";

        private readonly IBattleView battleView;
        private readonly TargetSelector targetSelector;
        private readonly DamageCalculator damageCalculator;
        private readonly TurnOutcomeProcessor turnOutcomeProcessor;

        public AttackProcessor(IBattleView battleView, TargetSelector targetSelector, DamageCalculator damageCalculator, TurnOutcomeProcessor turnOutcomeProcessor)
        {
            this.battleView = battleView;
            this.targetSelector = targetSelector;
            this.damageCalculator = damageCalculator;
            this.turnOutcomeProcessor = turnOutcomeProcessor;
        }

        public bool CanExecutePhysicalAttack(UnitInstanceContext attacker, BattleState battleState)
        {
            var attackContext = new AttackContext(attacker, battleState, AttackType.Physical);
            return CanProcessAttack(attackContext);
        }

        public bool CanExecuteGunAttack(UnitInstanceContext attacker, BattleState battleState)
        {
            var attackContext = new AttackContext(attacker, battleState, AttackType.Gun);
            return CanProcessAttack(attackContext);
        }

        private bool CanProcessAttack(AttackContext attackContext)
        {
            var availableTargets = GetValidTargets(attackContext);
            if (!availableTargets.Any())
            {
                return false;
            }

            var selectedTarget = targetSelector.SelectTargetForAttack(attackContext.Attacker, availableTargets);
            if (selectedTarget == null)
            {
                return false;
            }

            ExecuteAttackOnTarget(attackContext, selectedTarget);
            return true;
        }

        private List<UnitInstanceContext> GetValidTargets(AttackContext attackContext)
        {
            return targetSelector.GetAvailableTargetsForAttack(attackContext.BattleState);
        }

        private void ExecuteAttackOnTarget(AttackContext attackContext, UnitInstanceContext selectedTarget)
        {
            var element = attackContext.AttackType == AttackType.Gun ? DamageElement.Gun : DamageElement.Phys;
            var abilityName = attackContext.AttackType == AttackType.Gun ? GunAttackName : PhysicalAttackName;
            var baseDamage = damageCalculator.GetBasicAttackBaseDamage(attackContext.Attacker, element);
            var resolution = damageCalculator.ResolveDamage(attackContext.Attacker, selectedTarget, element, baseDamage);
            var context = BuildAttackResultContext(attackContext.Attacker, selectedTarget, abilityName, element, resolution, 1, 1);

            // Ejecutar acción como bloque atómico
            battleView.StartActionBuffer();
            battleView.ShowAttackResult(context);
            turnOutcomeProcessor.ApplyOutcome(attackContext.BattleState, resolution.Reaction);
            
            // Incrementar contador del jugador después de completar la acción
            attackContext.BattleState.IncrementCurrentPlayerActionCounter();
            
            battleView.FlushActionBuffer();
        }

        private AttackResultContext BuildAttackResultContext(
            UnitInstanceContext attacker,
            UnitInstanceContext target,
            string actionName,
            DamageElement element,
            DamageResolutionResult resolution,
            int hitNumber,
            int totalHits)
        {
            return new AttackResultContext(
                attacker,
                target,
                actionName,
                element,
                resolution.Reaction,
                resolution.DamageToTarget,
                resolution.DamageToAttacker,
                hitNumber,
                totalHits,
                resolution.TargetHpAfter,
                resolution.AttackerHpAfter,
                resolution.IsCritical);
        }
    }
}

