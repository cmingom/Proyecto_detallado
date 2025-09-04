using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public interface IBattleView
    {
        void ShowActionMenu(UnitInstanceContext unit, List<string> actions);
        int GetActionChoice(int actionCount);
        void ShowTargetSelection(UnitInstanceContext attacker, List<UnitInstanceContext> targets);
        int GetTargetChoice(int targetCount);
        void ShowAttackResult(AttackResultContext context);
        void ShowSkillSelection(UnitInstanceContext unit, List<Skill> skills);
        int GetSkillChoice(int skillCount);
        void ShowSurrender(SurrenderContext context);
        void ShowTurnConsumption();
    }
}
