using System;
using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Core;

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

        public bool ProcessUseSkill(UnitInstance unit, BattleState battleState)
        {
            var availableSkills = GetAvailableSkills(unit);
            ShowSkillSelection(unit, availableSkills);

            var skillChoice = GetSkillChoice(availableSkills.Count);
            return IsValidSkillChoice(skillChoice, availableSkills.Count);
        }

        public List<Skill> GetAvailableSkills(UnitInstance unit)
        {
            var availableSkills = CreateEmptySkillList();
            PopulateAffordableSkills(availableSkills, unit);
            return availableSkills;
        }

        private List<Skill> CreateEmptySkillList()
        {
            return new List<Skill>();
        }

        private void PopulateAffordableSkills(List<Skill> availableSkills, UnitInstance unit)
        {
            foreach (var skillName in unit.Skills)
            {
                AddSkillIfAffordable(availableSkills, skillName, unit.MP);
            }
        }

        private void AddSkillIfAffordable(List<Skill> availableSkills, string skillName, int unitMP)
        {
            if (skillData.TryGetValue(skillName, out var skill) && skill.Cost <= unitMP)
            {
                availableSkills.Add(skill);
            }
        }

        private void ShowSkillSelection(UnitInstance unit, List<Skill> availableSkills)
        {
            battleView.ShowSkillSelection(unit, availableSkills);
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
