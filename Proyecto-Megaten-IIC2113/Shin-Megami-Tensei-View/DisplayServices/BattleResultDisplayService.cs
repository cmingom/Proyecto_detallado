namespace Shin_Megami_Tensei_View.ConsoleLib
{
    public class BattleResultDisplayService
    {
        private const string SEPARATOR = "----------------------------------------";
        private const string SURRENDER_MESSAGE_FORMAT = "{0} ({1}) se rinde";
        private const string WINNER_MESSAGE_FORMAT = "Ganador: {0} ({1})";
        private const string TURN_CONSUMPTION_MESSAGE = "Se han consumido 1 Full Turn(s) y 0 Blinking Turn(s)";
        private const string TURN_OBTAINED_MESSAGE = "Se han obtenido 0 Blinking Turn(s)";

        private readonly View view;

        public BattleResultDisplayService(View view)
        {
            this.view = view;
        }

        public void ShowSurrender(string playerName, string playerNumber, string winnerName, string winnerNumber)
        {
            ShowSeparator();
            ShowSurrenderMessage(playerName, playerNumber);
            ShowSeparator();
            ShowWinnerMessage(winnerName, winnerNumber);
        }

        private void ShowSeparator()
        {
            view.WriteLine(SEPARATOR);
        }

        private void ShowSurrenderMessage(string playerName, string playerNumber)
        {
            view.WriteLine(string.Format(SURRENDER_MESSAGE_FORMAT, playerName, playerNumber));
        }

        private void ShowWinnerMessage(string winnerName, string winnerNumber)
        {
            view.WriteLine(string.Format(WINNER_MESSAGE_FORMAT, winnerName, winnerNumber));
        }

        public void ShowTurnConsumption()
        {
            ShowSeparator();
            ShowTurnConsumptionMessage();
            ShowTurnObtainedMessage();
        }

        private void ShowTurnConsumptionMessage()
        {
            view.WriteLine(TURN_CONSUMPTION_MESSAGE);
        }

        private void ShowTurnObtainedMessage()
        {
            view.WriteLine(TURN_OBTAINED_MESSAGE);
        }

        public void ShowWinner(string winnerName, string winnerNumber)
        {
            ShowSeparator();
            ShowWinnerMessage(winnerName, winnerNumber);
        }
    }
}
