using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_View.ConsoleLib
{
    public class BattlefieldDisplayService
    {
        private const int MAX_POSITIONS = 4;
        private const int MINIMUM_HP = 0;
        private const int INDEX_OFFSET = 1;
        private const char POSITION_A = 'A';
        private const char POSITION_B = 'B';
        private const char POSITION_C = 'C';
        private const char POSITION_D = 'D';
        private const string SEPARATOR = "----------------------------------------";
        private const string TEAM_HEADER_FORMAT = "Equipo de {0} ({1})";
        private const string UNIT_INFO_FORMAT = "{0}-{1} HP:{2}/{3} MP:{4}/{5}";
        private const string EMPTY_POSITION_FORMAT = "{0}-";
        private const string FULL_TURNS_FORMAT = "Full Turns: {0}";
        private const string BLINKING_TURNS_FORMAT = "Blinking Turns: {0}";
        private const string ORDER_HEADER = "Orden:";
        private const string ORDER_ITEM_FORMAT = "{0}-{1}";
        private const string ROUND_HEADER_FORMAT = "Ronda de {0} ({1})";

        private readonly View view;

        public BattlefieldDisplayService(View view)
        {
            this.view = view;
        }

        public void ShowBattlefield(BattleState battleState, string player1Name, string player2Name)
        {
            ShowSeparator();
            ShowTeamStatus(battleState.Team1, player1Name, "J1");
            ShowTeamStatus(battleState.Team2, player2Name, "J2");
        }

        private void ShowSeparator()
        {
            view.WriteLine(SEPARATOR);
        }

        private void ShowTeamStatus(TeamState team, string playerName, string playerNumber)
        {
            ShowTeamHeader(playerName, playerNumber);
            ShowAllUnitPositions(team);
        }

        private void ShowTeamHeader(string playerName, string playerNumber)
        {
            view.WriteLine(string.Format(TEAM_HEADER_FORMAT, playerName, playerNumber));
        }

        private void ShowAllUnitPositions(TeamState team)
        {
            char[] positions = GetPositions();
            
            for (int i = 0; i < MAX_POSITIONS; i++)
            {
                ShowSingleUnitPosition(team.Units[i], positions[i]);
            }
        }

        private char[] GetPositions()
        {
            return new char[] { POSITION_A, POSITION_B, POSITION_C, POSITION_D };
        }

        private void ShowSingleUnitPosition(GetUnitInstance? unit, char position)
        {
            if (unit == null)
            {
                ShowEmptyPosition(position);
                return;
            }

            if (ShouldShowUnitInfo(unit))
            {
                ShowUnitInfo(unit, position);
            }
            else
            {
                ShowEmptyPosition(position);
            }
        }

        private bool ShouldShowUnitInfo(GetUnitInstance getUnit)
        {
            return getUnit.IsSamurai || getUnit.HP > MINIMUM_HP;
        }

        private void ShowUnitInfo(GetUnitInstance getUnit, char position)
        {
            view.WriteLine(string.Format(UNIT_INFO_FORMAT, position, getUnit.Name, getUnit.HP, getUnit.MaxHP, getUnit.MP, getUnit.MaxMP));
        }

        private void ShowEmptyPosition(char position)
        {
            view.WriteLine(string.Format(EMPTY_POSITION_FORMAT, position));
        }

        public void ShowTurnCounters(BattleState battleState)
        {
            ShowSeparator();
            ShowFullTurns(battleState.FullTurns);
            ShowBlinkingTurns(battleState.BlinkingTurns);
        }

        private void ShowFullTurns(int fullTurns)
        {
            view.WriteLine(string.Format(FULL_TURNS_FORMAT, fullTurns));
        }

        private void ShowBlinkingTurns(int blinkingTurns)
        {
            view.WriteLine(string.Format(BLINKING_TURNS_FORMAT, blinkingTurns));
        }

        public void ShowActionOrderBySpeed(List<GetUnitInstance> actionOrder)
        {
            ShowSeparator();
            ShowOrderHeader();
            ShowOrderItems(actionOrder);
        }

        private void ShowOrderHeader()
        {
            view.WriteLine(ORDER_HEADER);
        }

        private void ShowOrderItems(List<GetUnitInstance> actionOrder)
        {
            for (int i = 0; i < actionOrder.Count; i++)
            {
                ShowOrderItem(i + INDEX_OFFSET, actionOrder[i].Name);
            }
        }

        private void ShowOrderItem(int index, string unitName)
        {
            view.WriteLine(string.Format(ORDER_ITEM_FORMAT, index, unitName));
        }

        public void ShowRoundHeader(string playerName, string playerNumber)
        {
            ShowSeparator();
            ShowRoundHeaderText(playerName, playerNumber);
        }

        private void ShowRoundHeaderText(string playerName, string playerNumber)
        {
            view.WriteLine(string.Format(ROUND_HEADER_FORMAT, playerName, playerNumber));
        }
    }
}

