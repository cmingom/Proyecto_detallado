using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class TargetSelector
    {
        private const int INVALID_CHOICE = -1;
        private const int CANCEL_CHOICE_OFFSET = 1;
        
        private readonly IBattleView battleView;

        public TargetSelector(IBattleView battleView)
        {
            this.battleView = battleView;
        }

        public List<GetUnitInstance> GetAvailableTargetsForAttack(BattleState battleState)
        {
            var enemyTeam = GetEnemyTeam(battleState);
            return GetAvailableTargets(enemyTeam);
        }

        private TeamState GetEnemyTeam(BattleState battleState)
        {
            return battleState.IsPlayer1Turn ? battleState.Team2 : battleState.Team1;
        }

        private List<GetUnitInstance> GetAvailableTargets(TeamState enemyTeam)
        {
            return enemyTeam.AliveUnits.ToList();
        }

        public GetUnitInstance SelectTargetForAttack(GetUnitInstance attacker, List<GetUnitInstance> availableTargets)
        {
            ShowTargetSelection(attacker, availableTargets);
            return ProcessTargetSelection(availableTargets);
        }

        private GetUnitInstance ProcessTargetSelection(List<GetUnitInstance> availableTargets)
        {
            var targetChoice = GetTargetChoice(availableTargets.Count);
            if (IsInvalidTargetChoice(targetChoice, availableTargets.Count))
                return null;

            return GetSelectedTarget(availableTargets, targetChoice);
        }

        private void ShowTargetSelection(GetUnitInstance attacker, List<GetUnitInstance> availableTargets)
        {
            battleView.ShowTargetSelection(attacker, availableTargets);
        }

        private int GetTargetChoice(int targetCount)
        {
            return battleView.GetTargetChoice(targetCount);
        }

        private bool IsInvalidTargetChoice(int targetChoice, int targetCount)
        {
            return targetChoice == INVALID_CHOICE || targetChoice == targetCount + CANCEL_CHOICE_OFFSET;
        }

        private GetUnitInstance GetSelectedTarget(List<GetUnitInstance> availableTargets, int targetChoice)
        {
            return availableTargets[targetChoice - 1];
        }
    }
}
