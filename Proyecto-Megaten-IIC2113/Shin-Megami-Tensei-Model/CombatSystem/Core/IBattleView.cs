using System.Collections.Generic;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.Domain.States;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public interface IBattleView
    {
        void ShowActionMenu(UnitInstance unit, List<string> actions);
        int GetActionChoice(int actionCount);
        void ShowTargetSelection(UnitInstance attacker, List<UnitInstance> targets);
        int GetTargetChoice(int targetCount);
        void ShowAttackResult(UnitInstance attacker, UnitInstance target, int damage, bool isGunAttack);
        void ShowSkillSelection(UnitInstance unit, List<Skill> skills);
        int GetSkillChoice(int skillCount);
        void ShowSurrender(string playerName, string playerNumber, string winnerName, string winnerNumber);
        void ShowTurnConsumption();
    }
}
