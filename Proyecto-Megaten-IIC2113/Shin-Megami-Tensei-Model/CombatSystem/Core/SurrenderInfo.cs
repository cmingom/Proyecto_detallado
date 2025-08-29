namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class SurrenderInfo
    {
        public string SurrenderingPlayerName { get; }
        public string SurrenderingPlayerNumber { get; }
        public string WinnerName { get; }
        public string WinnerNumber { get; }

        public SurrenderInfo(string surrenderingPlayerName, string surrenderingPlayerNumber, 
                           string winnerName, string winnerNumber)
        {
            SurrenderingPlayerName = surrenderingPlayerName;
            SurrenderingPlayerNumber = surrenderingPlayerNumber;
            WinnerName = winnerName;
            WinnerNumber = winnerNumber;
        }
    }
}
