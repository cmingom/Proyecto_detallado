using Shin_Megami_Tensei_View.ConsoleLib;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.CombatSystem.Core;

namespace Shin_Megami_Tensei
{
    public class TurnManager
    {
        private const string PLAYER_1_LABEL = "J1";
        private const string PLAYER_2_LABEL = "J2";
        private const int ZERO_TURNS = 0;

        private readonly BattleView battleView;
        private readonly CombatManager combatService;
        private readonly ActionProcessor actionProcessor;

        public TurnManager(BattleView battleView, CombatManager combatService)
        {
            this.battleView = battleView;
            this.combatService = combatService;
            this.actionProcessor = new ActionProcessor(battleView, combatService);
        }

        public bool IsPlayerTurnComplete(BattleState battleState, string player1Name, string player2Name)
        {
            var currentTeam = GetCurrentTeam(battleState);
            var turnContext = new TurnContext(battleState, currentTeam, player1Name, player2Name);
            return ShouldEndBattleAfterTurn(turnContext);
        }

        private bool ShouldEndBattleAfterTurn(TurnContext turnContext)
        {
            ShowPlayerTurnHeader(turnContext);
            return ShouldEndBattleAfterActions(turnContext);
        }

        private bool ShouldEndBattleAfterActions(TurnContext turnContext)
        {
            var shouldEndBattle = ShouldProcessPlayerActions(turnContext);
            HandlePlayerTurnEnd(turnContext);
            return shouldEndBattle;
        }

        private TeamState GetCurrentTeam(BattleState battleState)
        {
            return battleState.IsPlayer1Turn ? battleState.Team1 : battleState.Team2;
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

        private bool ShouldProcessPlayerActions(TurnContext turnContext)
        {
            var actionOrder = combatService.GetCalculatedActionOrder(turnContext.CurrentTeam);
            var battleContext = CreateBattleContext(turnContext);
            return actionProcessor.ShouldProcessActionOrder(battleContext, actionOrder, turnContext.CurrentTeam);
        }

        private BattleContext CreateBattleContext(TurnContext turnContext)
        {
            return new BattleContext { BattleState = turnContext.BattleState, Player1Name = turnContext.Player1Name, Player2Name = turnContext.Player2Name };
        }

        private void HandlePlayerTurnEnd(TurnContext turnContext)
        {
            if (ShouldContinueBattle(turnContext))
            {
                SwitchPlayerTurn(turnContext);
            }
        }

        private bool ShouldContinueBattle(TurnContext turnContext)
        {
            return !IsBattleEnded(turnContext);
        }

        private bool IsBattleEnded(TurnContext turnContext)
        {
            return IsTeamDefeated(turnContext.CurrentTeam) || IsTeamDefeated(GetOpponentTeam(turnContext));
        }

        private bool IsTeamDefeated(TeamState team)
        {
            return !team.AliveUnits.Any();
        }

        private TeamState GetOpponentTeam(TurnContext turnContext)
        {
            return turnContext.BattleState.IsPlayer1Turn ? turnContext.BattleState.Team2 : turnContext.BattleState.Team1;
        }

        private void SwitchPlayerTurn(TurnContext turnContext)
        {
            TogglePlayerTurn(turnContext.BattleState);
            var newCurrentTeam = GetCurrentTeam(turnContext.BattleState);
            UpdateTurnCounters(turnContext.BattleState, newCurrentTeam);
        }

        private void TogglePlayerTurn(BattleState battleState)
        {
            battleState.IsPlayer1Turn = !battleState.IsPlayer1Turn;
        }

        private void UpdateTurnCounters(BattleState battleState, TeamState newCurrentTeam)
        {
            SetFullTurns(battleState, newCurrentTeam);
            ResetBlinkingTurns(battleState);
        }

        private void SetFullTurns(BattleState battleState, TeamState newCurrentTeam)
        {
            battleState.FullTurns = combatService.GetCalculatedNextTurnCount(newCurrentTeam);
        }

        private void ResetBlinkingTurns(BattleState battleState)
        {
            battleState.BlinkingTurns = ZERO_TURNS;
        }
    }
}
