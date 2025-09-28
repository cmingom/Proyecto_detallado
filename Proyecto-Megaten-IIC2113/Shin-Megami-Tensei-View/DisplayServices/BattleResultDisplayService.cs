using Shin_Megami_Tensei_Model.CombatSystem.Contexts;

namespace Shin_Megami_Tensei_View.ConsoleLib
{
    public class BattleResultDisplayService
    {
        private const string Separator = "----------------------------------------";
        private const string SurrenderMessageFormat = "{0} ({1}) se rinde";
        private const string WinnerMessageFormat = "Ganador: {0} ({1})";
        private const string TurnConsumptionMessage = "Se han consumido 1 Full Turn(s) y 0 Blinking Turn(s)";
        private const string TurnObtainedMessage = "Se han obtenido 0 Blinking Turn(s)";

        private readonly View view;

        public BattleResultDisplayService(View view)
        {
            this.view = view;
        }

        public void ShowSurrender(SurrenderContext context)
        {
            ShowSeparator();
            ShowSurrenderMessage(context.PlayerName, context.PlayerNumber);
            ShowSeparator();
            ShowWinnerMessage(context.WinnerName, context.WinnerNumber);
        }

        private void ShowSeparator()
        {
            view.WriteLine(Separator);
        }

        private void ShowSurrenderMessage(string playerName, string playerNumber)
        {
            view.WriteLine(string.Format(SurrenderMessageFormat, playerName, playerNumber));
        }

        private void ShowWinnerMessage(string winnerName, string winnerNumber)
        {
            view.WriteLine(string.Format(WinnerMessageFormat, winnerName, winnerNumber));
        }

        public void ShowTurnConsumption()
        {
            ShowSeparator();
            ShowTurnConsumptionMessage();
            ShowTurnObtainedMessage();
        }

        private void ShowTurnConsumptionMessage()
        {
            view.WriteLine(TurnConsumptionMessage);
        }

        private void ShowTurnObtainedMessage()
        {
            view.WriteLine(TurnObtainedMessage);
        }

        private void ShowTurnConsumptionDetails(int fullTurnsConsumed, int blinkingTurnsConsumed, int blinkingTurnsGranted)
        {
            view.WriteLine($"Se han consumido {fullTurnsConsumed} Full Turn(s) y {blinkingTurnsConsumed} Blinking Turn(s)");
            view.WriteLine($"Se han obtenido {blinkingTurnsGranted} Blinking Turn(s)");
        }

        public void ShowWinner(string winnerName, string winnerNumber)
        {
            ShowSeparator();
            ShowWinnerMessage(winnerName, winnerNumber);
        }

        public void ShowTurnConsumptionWithBlinking(int fullTurnsConsumed, int blinkingTurnsConsumed, int blinkingTurnsGranted)
        {
            ShowSeparator();
            ShowTurnConsumptionDetails(fullTurnsConsumed, blinkingTurnsConsumed, blinkingTurnsGranted);
        }

        public void ShowSummonResult(string unitName)
        {
            ShowSeparator();
            view.WriteLine($"{unitName} ha sido invocado");
        }
    }
}


