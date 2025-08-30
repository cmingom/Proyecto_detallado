using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class GunAttackExecutor : IActionExecutor
    {
        private readonly DamageCalculator damageCalculator;
        private readonly IBattleView battleView;

        public GunAttackExecutor(DamageCalculator damageCalculator, IBattleView battleView)
        {
            this.damageCalculator = damageCalculator;
            this.battleView = battleView;
        }

        public bool Execute(UnitInstance actingUnit, BattleState battleState, string player1Name, string player2Name)
        {
            var attackContext = new AttackContext(actingUnit, battleState, AttackType.Gun);
            return ProcessAttack(attackContext);
        }

        private bool ProcessAttack(AttackContext attackContext)
        {
            var availableTargets = GetAvailableTargetsForAttack(attackContext);
            if (!availableTargets.Any()) 
                return false;

            var selectedTarget = SelectTargetForAttack(attackContext, availableTargets);
            if (selectedTarget == null) 
                return false;

            ExecuteAttackOnTarget(attackContext, selectedTarget);
            return true;
        }

        private List<UnitInstance> GetAvailableTargetsForAttack(AttackContext attackContext)
        {
            var enemyTeam = GetEnemyTeam(attackContext.BattleState);
            return GetAvailableTargets(enemyTeam);
        }

        private TeamState GetEnemyTeam(BattleState battleState)
        {
            return battleState.IsPlayer1Turn ? battleState.Team2 : battleState.Team1;
        }

        private List<UnitInstance> GetAvailableTargets(TeamState enemyTeam)
        {
            return enemyTeam.AliveUnits.ToList();
        }

        private UnitInstance SelectTargetForAttack(AttackContext attackContext, List<UnitInstance> availableTargets)
        {
            battleView.ShowTargetSelection(attackContext.Attacker, availableTargets);

            var targetChoice = battleView.GetTargetChoice(availableTargets.Count);
            if (IsInvalidTargetChoice(targetChoice, availableTargets.Count))
                return null;

            return availableTargets[targetChoice - 1];
        }

        private bool IsInvalidTargetChoice(int targetChoice, int targetCount)
        {
            return targetChoice == -1 || targetChoice == targetCount + 1;
        }

        private void ExecuteAttackOnTarget(AttackContext attackContext, UnitInstance selectedTarget)
        {
            var damage = CalculateAttackDamage(attackContext);
            ApplyDamageToTarget(selectedTarget, damage);
            ShowAttackResult(attackContext, selectedTarget, damage);
        }

        private int CalculateAttackDamage(AttackContext attackContext)
        {
            return damageCalculator.CalculateGunDamage(attackContext.Attacker.Skl);
        }

        private void ApplyDamageToTarget(UnitInstance target, int damage)
        {
            target.HP = Math.Max(0, target.HP - damage);
        }

        private void ShowAttackResult(AttackContext attackContext, UnitInstance target, int damage)
        {
            battleView.ShowAttackResult(attackContext.Attacker, target, damage, true);
        }
    }
}
