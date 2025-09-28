using System.Collections.Generic;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_View.ConsoleLib
{
    public class BattlefieldDisplayService
    {
        private const int MaxPositions = 4;
        private const int MinimumHp = 0;
        private const int IndexOffset = 1;
        private const char PositionA = 'A';
        private const char PositionB = 'B';
        private const char PositionC = 'C';
        private const char PositionD = 'D';
        private const string Separator = "----------------------------------------";
        private const string TeamHeaderFormat = "Equipo de {0} ({1})";
        private const string UnitInfoFormat = "{0}-{1} HP:{2}/{3} MP:{4}/{5}";
        private const string EmptyPositionFormat = "{0}-";
        private const string FullTurnsFormat = "Full Turns: {0}";
        private const string BlinkingTurnsFormat = "Blinking Turns: {0}";
        private const string OrderHeader = "Orden:";
        private const string OrderItemFormat = "{0}-{1}";
        private const string RoundHeaderFormat = "Ronda de {0} ({1})";

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
            view.WriteLine(Separator);
        }

        private void ShowTeamStatus(TeamState team, string playerName, string playerNumber)
        {
            ShowTeamHeader(playerName, playerNumber);
            ShowAllUnitPositions(team);
        }

        private void ShowTeamHeader(string playerName, string playerNumber)
        {
            view.WriteLine(string.Format(TeamHeaderFormat, playerName, playerNumber));
        }

        private void ShowAllUnitPositions(TeamState team)
        {
            char[] positions = GetPositions();
            
            for (int i = 0; i < MaxPositions; i++)
            {
                ShowSingleUnitPosition(team.Units[i], positions[i]);
            }
        }

        private char[] GetPositions()
        {
            return new char[] { PositionA, PositionB, PositionC, PositionD };
        }

        private void ShowSingleUnitPosition(UnitInstanceContext? unit, char position)
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

        private bool ShouldShowUnitInfo(UnitInstanceContext unit)
        {
            return unit.IsSamurai || unit.HP > MinimumHp;
        }

        private void ShowUnitInfo(UnitInstanceContext unit, char position)
        {
            view.WriteLine(string.Format(UnitInfoFormat, position, unit.Name, unit.HP, unit.MaxHP, unit.MP, unit.MaxMP));
        }

        private void ShowEmptyPosition(char position)
        {
            view.WriteLine(string.Format(EmptyPositionFormat, position));
        }

        public void ShowTurnCounters(BattleState battleState)
        {
            ShowSeparator();
            ShowFullTurns(battleState.FullTurns);
            ShowBlinkingTurns(battleState.BlinkingTurns);
        }

        private void ShowFullTurns(int fullTurns)
        {
            view.WriteLine(string.Format(FullTurnsFormat, fullTurns));
        }

        private void ShowBlinkingTurns(int blinkingTurns)
        {
            view.WriteLine(string.Format(BlinkingTurnsFormat, blinkingTurns));
        }

        public void ShowActionOrderBySpeed(List<UnitInstanceContext> actionOrder)
        {
            ShowSeparator();
            ShowOrderHeader();
            ShowOrderItems(actionOrder);
        }

        private void ShowOrderHeader()
        {
            view.WriteLine(OrderHeader);
        }

        private void ShowOrderItems(List<UnitInstanceContext> actionOrder)
        {
            for (int i = 0; i < actionOrder.Count; i++)
            {
                ShowOrderItem(i + IndexOffset, actionOrder[i].Name);
            }
        }

        private void ShowOrderItem(int index, string unitName)
        {
            view.WriteLine(string.Format(OrderItemFormat, index, unitName));
        }

        public void ShowRoundHeader(string playerName, string playerNumber)
        {
            ShowSeparator();
            ShowRoundHeaderText(playerName, playerNumber);
        }

        private void ShowRoundHeaderText(string playerName, string playerNumber)
        {
            view.WriteLine(string.Format(RoundHeaderFormat, playerName, playerNumber));
        }
    }
}



