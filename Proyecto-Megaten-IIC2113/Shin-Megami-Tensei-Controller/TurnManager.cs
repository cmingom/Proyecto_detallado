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

        // dividir en dos
        public bool IsPlayerTurnComplete(BattleState battleState, string player1Name, string player2Name)
        {
            var currentTeam = GetCurrentTeam(battleState);
            ShowPlayerTurnHeader(battleState, player1Name, player2Name);
            
            var shouldEndBattle = ProcessPlayerActions(battleState, currentTeam, player1Name, player2Name);
            HandlePlayerTurnEnd(battleState, currentTeam, shouldEndBattle);
            
            return shouldEndBattle;
        }

        private TeamState GetCurrentTeam(BattleState battleState)
        {
            return battleState.IsPlayer1Turn ? battleState.Team1 : battleState.Team2;
        }

        private void ShowPlayerTurnHeader(BattleState battleState, string player1Name, string player2Name)
        {
            var currentPlayerName = GetCurrentPlayerName(battleState, player1Name, player2Name);
            var playerNumber = GetCurrentPlayerNumber(battleState);
            battleView.ShowRoundHeader(currentPlayerName, playerNumber);
        }

        private string GetCurrentPlayerName(BattleState battleState, string player1Name, string player2Name)
        {
            return battleState.IsPlayer1Turn ? player1Name : player2Name;
        }

        private string GetCurrentPlayerNumber(BattleState battleState)
        {
            return battleState.IsPlayer1Turn ? PLAYER_1_LABEL : PLAYER_2_LABEL;
        }

        // verbo auxiliar, recibe 4 argumentos
        private bool ProcessPlayerActions(BattleState battleState, TeamState currentTeam, string player1Name, string player2Name)
        {
            var actionOrder = combatService.CalculateActionOrder(currentTeam);
            var battleContext = CreateBattleContext(battleState, player1Name, player2Name);
            return actionProcessor.ProcessActionOrder(battleContext, actionOrder, currentTeam);
        }

        private BattleContext CreateBattleContext(BattleState battleState, string player1Name, string player2Name)
        {
            return new BattleContext { BattleState = battleState, Player1Name = player1Name, Player2Name = player2Name };
        }

        // recibe un bool, cambiar
        private void HandlePlayerTurnEnd(BattleState battleState, TeamState currentTeam, bool shouldEndBattle)
        {
            if (ShouldSwitchTurn(shouldEndBattle))
            {
                SwitchPlayerTurn(battleState, currentTeam);
            }
        }

        // recibe un bool
        private bool ShouldSwitchTurn(bool shouldEndBattle)
        {
            return !shouldEndBattle;
        }

        // revisar parametros
        private void SwitchPlayerTurn(BattleState battleState, TeamState currentTeam)
        {
            TogglePlayerTurn(battleState);
            var newCurrentTeam = GetCurrentTeam(battleState);
            UpdateTurnCounters(battleState, newCurrentTeam);
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
            battleState.FullTurns = combatService.CalculateNextTurnCount(newCurrentTeam);
        }

        private void ResetBlinkingTurns(BattleState battleState)
        {
            battleState.BlinkingTurns = ZERO_TURNS;
        }
    }
}
