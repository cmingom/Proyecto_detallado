using System.Collections.Generic;
using System.Linq;
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

        public bool ProcessPhysicalAttack(UnitInstanceContext attacker, BattleState battleState)
        {
            var attackContext = new AttackContext(attacker, battleState, AttackType.Physical);
            return TryResolveAttack(attackContext);
        }

        public bool ProcessGunAttack(UnitInstanceContext attacker, BattleState battleState)
        {
            var attackContext = new AttackContext(attacker, battleState, AttackType.Gun);
            return TryResolveAttack(attackContext);
        }

        private bool TryResolveAttack(AttackContext attackContext)
        {
            var availableTargets = GetValidTargets(attackContext.BattleState);
            if (!availableTargets.Any())
            {
                return false;
            }

            var selectedTarget = targetSelector.RequestTargetForAttack(attackContext.Attacker, availableTargets);
            if (selectedTarget == null)
            {
                return false;
            }

            ExecuteAttackOnTarget(attackContext, selectedTarget);
            return true;
        }

        private List<UnitInstanceContext> GetValidTargets(BattleState battleState)
        {
            return targetSelector.GetAvailableTargetsForAttack(battleState);
        }

        private void ExecuteAttackOnTarget(AttackContext attackContext, UnitInstanceContext selectedTarget)
        {
            var element = attackContext.AttackType == AttackType.Gun ? DamageElement.Gun : DamageElement.Phys;
            var abilityName = attackContext.AttackType == AttackType.Gun ? GunAttackName : PhysicalAttackName;
            var baseDamage = damageCalculator.GetBasicAttackBaseDamage(attackContext.Attacker, element);
            var resolution = damageCalculator.ResolveDamage(attackContext.Attacker, selectedTarget, element, baseDamage);
            var context = BuildAttackResultContext(attackContext.Attacker, selectedTarget, abilityName, element, resolution, 1, 1);

            battleView.StartActionBuffer();
            battleView.ShowAttackResult(context);
            turnOutcomeProcessor.ProcessAffinityOutcome(attackContext.BattleState, resolution.Reaction);
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
