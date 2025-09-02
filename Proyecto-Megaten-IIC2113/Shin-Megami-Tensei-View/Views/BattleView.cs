using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Core;

namespace Shin_Megami_Tensei_View.ConsoleLib
{
    public class BattleView : IBattleView
    {
        private readonly View view;
        private readonly BattlefieldDisplayService battlefieldDisplayService;
        private readonly ActionMenuDisplayService actionMenuDisplayService;
        private readonly SkillDisplayService skillDisplayService;
        private readonly BattleResultDisplayService battleResultDisplayService;

        public BattleView(View view)
        {
            this.view = view;
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

        public void ShowActionOrderBySpeed(List<UnitInstance> actionOrder)
        {
            battlefieldDisplayService.ShowActionOrderBySpeed(actionOrder);
        }

        public void ShowActionMenu(UnitInstance actingUnit, List<string> actions)
        {
            actionMenuDisplayService.ShowActionMenu(actingUnit, actions);
        }

        public int GetActionChoice(int maxActions)
        {
            return actionMenuDisplayService.GetActionChoice(maxActions);
        }

        public void ShowTargetSelection(UnitInstance attacker, List<UnitInstance> targets)
        {
            actionMenuDisplayService.ShowTargetSelection(attacker, targets);
        }

        public int GetTargetChoice(int maxTargets)
        {
            return actionMenuDisplayService.GetTargetChoice(maxTargets);
        }

        public void ShowAttackResult(UnitInstance attacker, UnitInstance target, int damage, bool isGunAttack)
        {
            actionMenuDisplayService.ShowAttackResult(attacker, target, damage, isGunAttack);
        }

        public void ShowSkillSelection(UnitInstance unit, List<Skill> availableSkills)
        {
            skillDisplayService.ShowSkillSelection(unit, availableSkills);
        }

        public int GetSkillChoice(int maxSkills)
        {
            return skillDisplayService.GetSkillChoice(maxSkills);
        }

        public void ShowSurrender(string playerName, string playerNumber, string winnerName, string winnerNumber)
        {
            battleResultDisplayService.ShowSurrender(playerName, playerNumber, winnerName, winnerNumber);
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
