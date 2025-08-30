using System;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_View.ConsoleLib
{
    public class BattleResultDisplayService
    {
        private readonly View view;

        public BattleResultDisplayService(View view)
        {
            this.view = view;
        }

        public void ShowSurrender(string playerName, string playerNumber, string winnerName, string winnerNumber)
        {
            view.WriteLine("----------------------------------------");
            view.WriteLine($"{playerName} ({playerNumber}) se rinde");
            view.WriteLine("----------------------------------------");
            view.WriteLine($"Ganador: {winnerName} ({winnerNumber})");
        }

        public void ShowTurnConsumption()
        {
            view.WriteLine("----------------------------------------");
            view.WriteLine("Se han consumido 1 Full Turn(s) y 0 Blinking Turn(s)");
            view.WriteLine("Se han obtenido 0 Blinking Turn(s)");
        }

        public void ShowWinner(string winnerName, string winnerNumber)
        {
            view.WriteLine("----------------------------------------");
            view.WriteLine($"Ganador: {winnerName} ({winnerNumber})");
        }
    }
}
