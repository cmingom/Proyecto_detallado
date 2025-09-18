using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Core;

namespace Shin_Megami_Tensei_Model.CombatSystem.Contexts
{
    public class ActionCoordinatorConfig
    {
        public IBattleView BattleView { get; }
        public SurrenderProcessor SurrenderProcessor { get; }
        public PassTurnProcessor PassTurnProcessor { get; }
        public Dictionary<string, Skill> SkillData { get; }
        
        public ActionCoordinatorConfig(IBattleView battleView, SurrenderProcessor surrenderProcessor, PassTurnProcessor passTurnProcessor, Dictionary<string, Skill> skillData)
        {
            BattleView = battleView;
            SurrenderProcessor = surrenderProcessor;
            PassTurnProcessor = passTurnProcessor;
            SkillData = skillData;
        }
    }
}
