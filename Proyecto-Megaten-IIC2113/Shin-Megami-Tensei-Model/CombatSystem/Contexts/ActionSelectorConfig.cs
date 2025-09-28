using System.Collections.Generic;
using Shin_Megami_Tensei_Model.CombatSystem.Core;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Contexts
{
    public sealed class ActionSelectorConfig
    {
        public ActionSelectorConfig(
            IBattleView battleView,
            SurrenderProcessor surrenderHandler,
            PassTurnProcessor passTurnProcessor,
            Dictionary<string, Skill> skillData,
            TargetSelector targetSelector,
            DamageCalculator damageCalculator,
            TurnOutcomeProcessor turnOutcomeProcessor)
        {
            BattleView = battleView;
            SurrenderHandler = surrenderHandler;
            PassTurnProcessor = passTurnProcessor;
            SkillData = skillData;
            TargetSelector = targetSelector;
            DamageCalculator = damageCalculator;
            TurnOutcomeProcessor = turnOutcomeProcessor;
        }

        public IBattleView BattleView { get; }
        public SurrenderProcessor SurrenderHandler { get; }
        public PassTurnProcessor PassTurnProcessor { get; }
        public Dictionary<string, Skill> SkillData { get; }
        public TargetSelector TargetSelector { get; }
        public DamageCalculator DamageCalculator { get; }
        public TurnOutcomeProcessor TurnOutcomeProcessor { get; }
    }
}
