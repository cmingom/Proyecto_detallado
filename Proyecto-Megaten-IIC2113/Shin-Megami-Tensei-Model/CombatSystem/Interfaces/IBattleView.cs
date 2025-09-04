using System.Collections.Generic;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public interface IBattleView
    {
        void ShowActionMenu(GetUnitInstance getUnit, List<string> actions);
        int GetActionChoice(int actionCount);
        void ShowTargetSelection(GetUnitInstance attacker, List<GetUnitInstance> targets);
        int GetTargetChoice(int targetCount);
        void ShowAttackResult(AttackResultContext context);
        void ShowSkillSelection(GetUnitInstance getUnit, List<Skill> skills);
        int GetSkillChoice(int skillCount);
        void ShowSurrender(SurrenderContext context);
        void ShowTurnConsumption();
    }
}
