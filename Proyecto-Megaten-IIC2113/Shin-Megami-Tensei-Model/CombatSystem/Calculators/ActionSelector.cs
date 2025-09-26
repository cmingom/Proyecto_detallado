using Shin_Megami_Tensei_Model.CombatSystem.Exceptions;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class ActionSelector
    {
        private const string ATTACK_ACTION = "Atacar";
        private const string GUN_ACTION = "Disparar";
        private const string SKILL_ACTION = "Usar Habilidad";
        private const string SUMMON_ACTION = "Invocar";
        private const string PASS_TURN_ACTION = "Pasar Turno";
        private const string SURRENDER_ACTION = "Rendirse";

        private readonly SurrenderProcessor surrenderHandler;
        private readonly AttackProcessor attackExecutor;
        private readonly SkillProcessor skillManager;
        private readonly PassTurnProcessor passTurnProcessor;
        private readonly SummonProcessor summonProcessor;

        public ActionSelector(ActionSelectorConfig config)
        {
            surrenderHandler = config.SurrenderHandler;
            attackExecutor = new AttackProcessor(config.BattleView, config.TargetSelector, config.DamageCalculator, config.TurnOutcomeProcessor);
            skillManager = new SkillProcessor(config.BattleView, config.SkillData, config.TargetSelector, config.DamageCalculator, config.TurnOutcomeProcessor);
            passTurnProcessor = config.PassTurnProcessor;
            summonProcessor = new SummonProcessor(config.BattleView);
        }

        public bool CanProcessSelectedAction(ActionContext actionContext, string selectedAction)
        {
            return selectedAction switch
            {
                ATTACK_ACTION => attackExecutor.CanExecutePhysicalAttack(actionContext.ActingUnit, actionContext.BattleState),
                GUN_ACTION => attackExecutor.CanExecuteGunAttack(actionContext.ActingUnit, actionContext.BattleState),
                SKILL_ACTION => skillManager.CanProcessUseSkill(actionContext.ActingUnit, actionContext.BattleState),
                SUMMON_ACTION => summonProcessor.CanProcessSummon(actionContext.ActingUnit, actionContext.BattleState),
                PASS_TURN_ACTION => CanPassTurn(actionContext),
                SURRENDER_ACTION => CanProcessSurrenderAction(actionContext),
                _ => IsValidAction()
            };
        }

        private bool CanPassTurn(ActionContext actionContext)
        {
            passTurnProcessor.ProcessPassTurn(actionContext.BattleState);
            return true;
        }

        private bool CanProcessSurrenderAction(ActionContext actionContext)
        {
            try
            {
                surrenderHandler.HasSurrender(actionContext.BattleState, actionContext.Player1Name, actionContext.Player2Name);
                return true;
            }
            catch (GameEndedException)
            {
                // La excepción se maneja aquí - la batalla terminó por rendirse
                // Retornar true para indicar que la acción se completó exitosamente
                return true;
            }
        }

        private bool IsValidAction()
        {
            return false;
        }
    }
}
