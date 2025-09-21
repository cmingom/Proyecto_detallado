using System.Collections.Generic;
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
        void ShowGuardAction(UnitInstanceContext unit);
        void ShowSkillSelection(UnitInstanceContext unit, List<Skill> skills);
        int GetSkillChoice(int skillCount);
        void ShowSurrender(SurrenderContext context);
        void ShowTurnConsumption();
        void ShowTurnConsumptionWithBlinking(int fullTurnsConsumed, int blinkingTurnsConsumed, int blinkingTurnsGranted);
        void ShowSummonMenu(List<UnitInstanceContext> availableUnits);
        int GetSummonChoice(int maxOptions);
        void ShowSummonPositionMenu(List<(char Slot, UnitInstanceContext? Unit)> positionOptions);
        int GetSummonPositionChoice(int maxOptions);
        void ShowSummonResult(string unitName);
    }
}

