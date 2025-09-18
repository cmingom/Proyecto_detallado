using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class ActionSelectorConfig
    {
        public IBattleView BattleView { get; set; }
        public SurrenderProcessor SurrenderHandler { get; set; }
        public PassTurnProcessor PassTurnProcessor { get; set; }
        public Dictionary<string, Skill> SkillData { get; set; }

        public ActionSelectorConfig(IBattleView battleView, SurrenderProcessor surrenderHandler, PassTurnProcessor passTurnProcessor, Dictionary<string, Skill> skillData)
        {
            BattleView = battleView;
            SurrenderHandler = surrenderHandler;
            PassTurnProcessor = passTurnProcessor;
            SkillData = skillData;
        }
    }
}
