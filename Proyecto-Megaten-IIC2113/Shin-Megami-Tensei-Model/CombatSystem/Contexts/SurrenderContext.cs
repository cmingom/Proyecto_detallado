namespace Shin_Megami_Tensei_Model.CombatSystem.Contexts
{
    public class SurrenderContext
    {
        public string PlayerName { get; }
        public string PlayerNumber { get; }
        public string WinnerName { get; }
        public string WinnerNumber { get; }

        public SurrenderContext(string playerName, string playerNumber, string winnerName, string winnerNumber)
        {
            PlayerName = playerName;
            PlayerNumber = playerNumber;
            WinnerName = winnerName;
            WinnerNumber = winnerNumber;
        }
    }
}
