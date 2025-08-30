using System;
using System.Collections.Generic;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_View.ConsoleLib
{
    public class BattlefieldDisplayService
    {
        private readonly View view;

        public BattlefieldDisplayService(View view)
        {
            this.view = view;
        }

        public void ShowBattlefield(BattleState battleState, string player1Name, string player2Name)
        {
            view.WriteLine("----------------------------------------");
            ShowTeamStatus(battleState.Team1, player1Name, "J1");
            ShowTeamStatus(battleState.Team2, player2Name, "J2");
        }

        private void ShowTeamStatus(TeamState team, string playerName, string playerNumber)
        {
            view.WriteLine($"Equipo de {playerName} ({playerNumber})");
            
            char[] positions = { 'A', 'B', 'C', 'D' };
            
            for (int i = 0; i < 4; i++)
            {
                var unit = team.Units[i];
                if (unit != null)
                {
                    if (unit.IsSamurai || unit.HP > 0)
                    {
                        view.WriteLine($"{positions[i]}-{unit.Name} HP:{unit.HP}/{unit.MaxHP} MP:{unit.MP}/{unit.MaxMP}");
                    }
                    else
                    {
                        view.WriteLine($"{positions[i]}-");
                    }
                }
                else
                {
                    view.WriteLine($"{positions[i]}-");
                }
            }
        }

        public void ShowTurnCounters(BattleState battleState)
        {
            view.WriteLine("----------------------------------------");
            view.WriteLine($"Full Turns: {battleState.FullTurns}");
            view.WriteLine($"Blinking Turns: {battleState.BlinkingTurns}");
        }

        public void ShowActionOrderBySpeed(List<UnitInstance> actionOrder)
        {
            view.WriteLine("----------------------------------------");
            view.WriteLine("Orden:");
            
            for (int i = 0; i < actionOrder.Count; i++)
            {
                view.WriteLine($"{i + 1}-{actionOrder[i].Name}");
            }
        }

        public void ShowRoundHeader(string playerName, string playerNumber)
        {
            view.WriteLine("----------------------------------------");
            view.WriteLine($"Ronda de {playerName} ({playerNumber})");
        }
    }
}
