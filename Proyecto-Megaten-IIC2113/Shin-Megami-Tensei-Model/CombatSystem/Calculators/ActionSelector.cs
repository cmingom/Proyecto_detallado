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
            this.surrenderHandler = config.SurrenderHandler;
            this.attackExecutor = new AttackProcessor(config.BattleView);
            this.skillManager = new SkillProcessor(config.BattleView, config.SkillData);
            this.passTurnProcessor = config.PassTurnProcessor;
            this.summonProcessor = new SummonProcessor(config.BattleView);
        }

        public bool CanProcessSelectedAction(ActionContext actionContext, string selectedAction)
        {
            return CanProcessSelectedAction(selectedAction, actionContext);
        }

        private bool CanProcessSelectedAction(string selectedAction, ActionContext actionContext)
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
            return true; // Pasar Turno siempre es exitoso
        }

        private bool CanProcessSurrenderAction(ActionContext actionContext)
        {
            surrenderHandler.HasSurrender(actionContext.BattleState, actionContext.Player1Name, actionContext.Player2Name);
            return true;
        }

        private bool IsValidAction()
        {
            return false;
        }
    }
}
