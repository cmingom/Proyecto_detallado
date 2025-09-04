using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class SkillProcessor
    {
        private const int INVALID_SKILL_CHOICE = -1;
        private const int CANCEL_SKILL_CHOICE_OFFSET = 1;
        
        private readonly IBattleView battleView;
        private readonly Dictionary<string, Skill> skillData;

        public SkillProcessor(IBattleView battleView, Dictionary<string, Skill> skillData)
        {
            this.battleView = battleView;
            this.skillData = skillData;
        }

        public bool CanProcessUseSkill(GetUnitInstance getUnit, BattleState battleState)
        {
            var availableSkills = GetAvailableSkills(getUnit);
            ShowSkillSelection(getUnit, availableSkills);

            var skillChoice = GetSkillChoice(availableSkills.Count);
            return IsValidSkillChoice(skillChoice, availableSkills.Count);
        }

        public List<Skill> GetAvailableSkills(GetUnitInstance getUnit)
        {
            var availableSkills = CreateEmptySkillList();
            PopulateAffordableSkills(availableSkills, getUnit);
            return availableSkills;
        }

        private List<Skill> CreateEmptySkillList()
        {
            return new List<Skill>();
        }

        private void PopulateAffordableSkills(List<Skill> availableSkills, GetUnitInstance getUnit)
        {
            foreach (var skillName in getUnit.Skills)
            {
                AddSkill(availableSkills, skillName, getUnit.MP);
            }
        }

        private void AddSkill(List<Skill> availableSkills, string skillName, int unitMP)
        {
            if (IsAffordable(skillName, unitMP))
            {
                availableSkills.Add(skillData[skillName]);
            }
        }
        
        private bool IsAffordable(string skillName, int unitMP)
        {
            return skillData.TryGetValue(skillName, out var skill) && skill.Cost <= unitMP;
        }

        private void ShowSkillSelection(GetUnitInstance getUnit, List<Skill> availableSkills)
        {
            battleView.ShowSkillSelection(getUnit, availableSkills);
        }

        private int GetSkillChoice(int skillCount)
        {
            return battleView.GetSkillChoice(skillCount);
        }

        private bool IsValidSkillChoice(int skillChoice, int skillCount)
        {
            return !IsInvalidSkillChoice(skillChoice, skillCount);
        }

        private bool IsInvalidSkillChoice(int skillChoice, int skillCount)
        {
            return skillChoice == INVALID_SKILL_CHOICE || skillChoice == skillCount + CANCEL_SKILL_CHOICE_OFFSET;
        }
    }
}

