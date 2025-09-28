using Shin_Megami_Tensei_Model.CombatSystem.Contexts;
using Shin_Megami_Tensei_Model.CombatSystem.Exceptions;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class ActionSelector
    {
        private const string AttackActionName = "Atacar";
        private const string GunActionName = "Disparar";
        private const string SkillActionName = "Usar Habilidad";
        private const string SummonActionName = "Invocar";
        private const string PassTurnActionName = "Pasar Turno";
        private const string SurrenderActionName = "Rendirse";

        private readonly SurrenderProcessor surrenderProcessor;
        private readonly AttackProcessor attackProcessor;
        private readonly SkillProcessor skillProcessor;
        private readonly PassTurnProcessor passTurnProcessor;
        private readonly SummonProcessor summonProcessor;

        public ActionSelector(ActionSelectorConfig config)
        {
            surrenderProcessor = config.SurrenderHandler;
            attackProcessor = new AttackProcessor(config.BattleView, config.TargetSelector, config.DamageCalculator, config.TurnOutcomeProcessor);
            skillProcessor = new SkillProcessor(config.BattleView, config.SkillData, config.TargetSelector, config.DamageCalculator, config.TurnOutcomeProcessor);
            passTurnProcessor = config.PassTurnProcessor;
            summonProcessor = new SummonProcessor(config.BattleView);
        }

        public bool ProcessSelectedAction(ActionContext actionContext, string selectedAction)
        {
            return selectedAction switch
            {
                AttackActionName => attackProcessor.ProcessPhysicalAttack(actionContext.ActingUnit, actionContext.BattleState),
                GunActionName => attackProcessor.ProcessGunAttack(actionContext.ActingUnit, actionContext.BattleState),
                SkillActionName => skillProcessor.ProcessSkill(actionContext.ActingUnit, actionContext.BattleState),
                SummonActionName => summonProcessor.ProcessSummon(actionContext.ActingUnit, actionContext.BattleState),
                PassTurnActionName => ProcessPassTurn(actionContext),
                SurrenderActionName => ProcessSurrender(actionContext),
                _ => HandleUnknownAction()
            };
        }

        private bool ProcessPassTurn(ActionContext actionContext)
        {
            passTurnProcessor.ProcessPassTurn(actionContext.BattleState);
            return true;
        }

        private bool ProcessSurrender(ActionContext actionContext)
        {
            try
            {
                surrenderProcessor.HasSurrender(actionContext.BattleState, actionContext.Player1Name, actionContext.Player2Name);
                return true;
            }
            catch (GameEndedException)
            {
                return true;
            }
        }

        private static bool HandleUnknownAction()
        {
            return false;
        }
    }
}

