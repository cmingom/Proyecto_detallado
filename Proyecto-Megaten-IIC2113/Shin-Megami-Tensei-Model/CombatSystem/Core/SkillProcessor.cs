using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class SkillProcessor : IActionHandler
    {
        private readonly IBattleView battleView;
        private readonly Dictionary<string, Skill> skillData;

        public SkillProcessor(IBattleView battleView, Dictionary<string, Skill> skillData)
        {
            this.battleView = battleView;
            this.skillData = skillData;
        }

        public bool Execute(UnitInstance actingUnit, BattleState battleState, string player1Name, string player2Name)
        {
            return ProcessUseSkill(actingUnit, battleState);
        }

        private bool ProcessUseSkill(UnitInstance unit, BattleState battleState)
        {
            var availableSkills = GetAvailableSkills(unit);
            battleView.ShowSkillSelection(unit, availableSkills);

            var skillChoice = battleView.GetSkillChoice(availableSkills.Count);
            return !IsInvalidSkillChoice(skillChoice, availableSkills.Count);
        }

        private List<Skill> GetAvailableSkills(UnitInstance unit)
        {
            var availableSkills = new List<Skill>();
            foreach (var skillName in unit.Skills)
            {
                AddSkillIfAffordable(availableSkills, skillName, unit.MP);
            }
            return availableSkills;
        }

        private void AddSkillIfAffordable(List<Skill> availableSkills, string skillName, int unitMP)
        {
            if (skillData.TryGetValue(skillName, out var skill) && skill.Cost <= unitMP)
            {
                availableSkills.Add(skill);
            }
        }

        private bool IsInvalidSkillChoice(int skillChoice, int skillCount)
        {
            return skillChoice == -1 || skillChoice == skillCount + 1;
        }
    }
}
