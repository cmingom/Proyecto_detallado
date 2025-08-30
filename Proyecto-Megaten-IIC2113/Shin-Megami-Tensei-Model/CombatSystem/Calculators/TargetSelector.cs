using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class TargetSelector
    {
        private readonly IBattleView battleView;

        public TargetSelector(IBattleView battleView)
        {
            this.battleView = battleView;
        }

        public List<UnitInstance> GetAvailableTargetsForAttack(BattleState battleState, bool isPlayer1Turn)
        {
            var enemyTeam = GetEnemyTeam(battleState, isPlayer1Turn);
            return GetAvailableTargets(enemyTeam);
        }

        private TeamState GetEnemyTeam(BattleState battleState, bool isPlayer1Turn)
        {
            return isPlayer1Turn ? battleState.Team2 : battleState.Team1;
        }

        private List<UnitInstance> GetAvailableTargets(TeamState enemyTeam)
        {
            return enemyTeam.AliveUnits.ToList();
        }

        public UnitInstance SelectTargetForAttack(UnitInstance attacker, List<UnitInstance> availableTargets)
        {
            ShowTargetSelection(attacker, availableTargets);

            var targetChoice = GetTargetChoice(availableTargets.Count);
            if (IsInvalidTargetChoice(targetChoice, availableTargets.Count))
                return null;

            return GetSelectedTarget(availableTargets, targetChoice);
        }

        private void ShowTargetSelection(UnitInstance attacker, List<UnitInstance> availableTargets)
        {
            battleView.ShowTargetSelection(attacker, availableTargets);
        }

        private int GetTargetChoice(int targetCount)
        {
            return battleView.GetTargetChoice(targetCount);
        }

        private UnitInstance GetSelectedTarget(List<UnitInstance> availableTargets, int targetChoice)
        {
            return availableTargets[targetChoice - 1];
        }

        private bool IsInvalidTargetChoice(int targetChoice, int targetCount)
        {
            return targetChoice == -1 || targetChoice == targetCount + 1;
        }
    }
}
