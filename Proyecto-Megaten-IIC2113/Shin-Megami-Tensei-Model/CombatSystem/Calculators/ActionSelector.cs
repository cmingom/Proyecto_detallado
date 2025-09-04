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

        public ActionSelector(ActionSelectorConfig config)
        {
            this.surrenderHandler = config.SurrenderHandler;
            this.attackExecutor = new AttackProcessor(config.BattleView);
            this.skillManager = new SkillProcessor(config.BattleView, config.SkillData);
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
                SUMMON_ACTION => CanExecuteSummon(),
                PASS_TURN_ACTION => CanPassTurn(),
                SURRENDER_ACTION => CanProcessSurrenderAction(actionContext),
                _ => IsValidAction()
            };
        }

        private bool CanExecuteSummon()
        {
            return true;
        }

        private bool CanPassTurn()
        {
            return true;
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
