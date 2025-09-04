using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Core;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;

namespace Shin_Megami_Tensei_View.ConsoleLib
{
    public class BattleView : IBattleView
    {
        private readonly BattlefieldDisplayService battlefieldDisplayService;
        private readonly ActionMenuDisplayService actionMenuDisplayService;
        private readonly SkillDisplayService skillDisplayService;
        private readonly BattleResultDisplayService battleResultDisplayService;

        public BattleView(View view)
        {
            this.battlefieldDisplayService = new BattlefieldDisplayService(view);
            this.actionMenuDisplayService = new ActionMenuDisplayService(view);
            this.skillDisplayService = new SkillDisplayService(view);
            this.battleResultDisplayService = new BattleResultDisplayService(view);
        }

        public void ShowRoundHeader(string playerName, string playerNumber)
        {
            battlefieldDisplayService.ShowRoundHeader(playerName, playerNumber);
        }

        public void ShowBattlefield(BattleState battleState, string player1Name, string player2Name)
        {
            battlefieldDisplayService.ShowBattlefield(battleState, player1Name, player2Name);
        }

        public void ShowTurnCounters(BattleState battleState)
        {
            battlefieldDisplayService.ShowTurnCounters(battleState);
        }

        public void ShowActionOrderBySpeed(List<UnitInstanceContext> actionOrder)
        {
            battlefieldDisplayService.ShowActionOrderBySpeed(actionOrder);
        }

        public void ShowActionMenu(UnitInstanceContext actingUnit, List<string> actions)
        {
            actionMenuDisplayService.ShowActionMenu(actingUnit, actions);
        }

        public int GetActionChoice(int maxActions)
        {
            return actionMenuDisplayService.GetActionChoice(maxActions);
        }

        public void ShowTargetSelection(UnitInstanceContext attacker, List<UnitInstanceContext> targets)
        {
            actionMenuDisplayService.ShowTargetSelection(attacker, targets);
        }

        public int GetTargetChoice(int maxTargets)
        {
            return actionMenuDisplayService.GetTargetChoice(maxTargets);
        }

        public void ShowAttackResult(AttackResultContext context)
        {
            actionMenuDisplayService.ShowAttackResult(context);
        }

        public void ShowSkillSelection(UnitInstanceContext unit, List<Skill> availableSkills)
        {
            skillDisplayService.ShowSkillSelection(unit, availableSkills);
        }

        public int GetSkillChoice(int maxSkills)
        {
            return skillDisplayService.GetSkillChoice(maxSkills);
        }

        public void ShowSurrender(SurrenderContext context)
        {
            battleResultDisplayService.ShowSurrender(context);
        }

        public void ShowTurnConsumption()
        {
            battleResultDisplayService.ShowTurnConsumption();
        }

        public void ShowWinner(string winnerName, string winnerNumber)
        {
            battleResultDisplayService.ShowWinner(winnerName, winnerNumber);
        }
    }
}
