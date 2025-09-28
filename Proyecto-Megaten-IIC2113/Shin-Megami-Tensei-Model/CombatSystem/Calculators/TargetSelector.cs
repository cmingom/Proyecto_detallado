using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.Domain.States;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class TargetSelector
    {
        private const int InvalidChoice = -1;
        private const int CancelChoiceOffset = 1;

        private readonly IBattleView battleView;

        public TargetSelector(IBattleView battleView)
        {
            this.battleView = battleView;
        }

        public List<UnitInstanceContext> GetAvailableTargetsForAttack(BattleState battleState)
        {
            var enemyTeam = battleState.IsPlayer1Turn ? battleState.Team2 : battleState.Team1;
            return enemyTeam.AliveUnits.ToList();
        }

        public UnitInstanceContext? RequestTargetForAttack(UnitInstanceContext attacker, List<UnitInstanceContext> availableTargets)
        {
            battleView.ShowTargetSelection(attacker, availableTargets);
            var targetChoice = battleView.GetTargetChoice(availableTargets.Count);

            if (IsCancelledSelection(targetChoice, availableTargets.Count))
            {
                return null;
            }

            return availableTargets[targetChoice - 1];
        }

        private static bool IsCancelledSelection(int targetChoice, int targetCount)
        {
            return targetChoice == InvalidChoice || targetChoice == targetCount + CancelChoiceOffset;
        }
    }
}
