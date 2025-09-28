using System.Linq;
using Shin_Megami_Tensei_View.ConsoleLib;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.CombatSystem.Core;

namespace Shin_Megami_Tensei
{
    public class TurnManager
    {
        private const string PLAYER_1_LABEL = "J1";
        private const string PLAYER_2_LABEL = "J2";

        private readonly BattleView battleView;
        private readonly CombatManager combatManager;
        private readonly BattleActionController actionController;

        public TurnManager(BattleView battleView, CombatManager combatManager)
        {
            this.battleView = battleView;
            this.combatManager = combatManager;
            actionController = new BattleActionController(battleView, combatManager);
        }

        public bool ProcessTurn(BattleState battleState, string player1Name, string player2Name)
        {
            var turnContext = new TurnContext(battleState, battleState.GetCurrentTeam(), player1Name, player2Name);

            ShowPlayerTurnHeader(turnContext);

            var battleEnded = ExecutePlayerActions(turnContext);
            if (turnContext.BattleState.IsBattleFinished)
            {
                return true;
            }

            FinalizeTurn(turnContext);
            return battleEnded;
        }

        private void ShowPlayerTurnHeader(TurnContext turnContext)
        {
            var currentPlayerName = GetCurrentPlayerName(turnContext);
            var playerNumber = GetCurrentPlayerNumber(turnContext.BattleState);
            battleView.ShowRoundHeader(currentPlayerName, playerNumber);
        }

        private string GetCurrentPlayerName(TurnContext turnContext)
        {
            return turnContext.BattleState.IsPlayer1Turn ? turnContext.Player1Name : turnContext.Player2Name;
        }

        private string GetCurrentPlayerNumber(BattleState battleState)
        {
            return battleState.IsPlayer1Turn ? PLAYER_1_LABEL : PLAYER_2_LABEL;
        }

        private bool ExecutePlayerActions(TurnContext turnContext)
        {
            var actionOrder = combatManager.GetCalculatedActionOrder(turnContext.CurrentTeam);
            var battleContext = CreateBattleContext(turnContext);
            return actionController.ResolveActionPhase(battleContext, actionOrder, turnContext.CurrentTeam);
        }

        private static BattleContext CreateBattleContext(TurnContext turnContext)
        {
            return new BattleContext
            {
                BattleState = turnContext.BattleState,
                Player1Name = turnContext.Player1Name,
                Player2Name = turnContext.Player2Name
            };
        }

        private void FinalizeTurn(TurnContext turnContext)
        {
            if (HasAnyTeamLost(turnContext))
            {
                return;
            }

            SwitchPlayer(turnContext.BattleState);
            var newCurrentTeam = turnContext.BattleState.GetCurrentTeam();
            UpdateTurnCounters(turnContext.BattleState, newCurrentTeam);
        }

        private bool HasAnyTeamLost(TurnContext turnContext)
        {
            return IsTeamDefeated(turnContext.CurrentTeam) ||
                   IsTeamDefeated(turnContext.BattleState.GetOpponentTeam());
        }

        private static bool IsTeamDefeated(TeamState team)
        {
            return !team.AliveUnits.Any();
        }

        private static void SwitchPlayer(BattleState battleState)
        {
            battleState.SwitchPlayer();
        }

        private void UpdateTurnCounters(BattleState battleState, TeamState newCurrentTeam)
        {
            SetFullTurns(battleState, newCurrentTeam);
            ResetBlinkingTurns(battleState);
        }

        private void SetFullTurns(BattleState battleState, TeamState newCurrentTeam)
        {
            battleState.SetFullTurns(combatManager.GetCalculatedNextTurnCount(newCurrentTeam));
        }

        private static void ResetBlinkingTurns(BattleState battleState)
        {
            battleState.ResetBlinkingTurns();
        }
    }
}
