using Shin_Megami_Tensei_Model.Data.Components;

namespace Shin_Megami_Tensei_Model.SetUp.Loader;

public static class GameLoader
{
    public static Adversaries LoadPlayersTextLines(string[] fileLines)
    {
        Adversaries adversaries = new();
        var currentPlayerNumber = 0;
        foreach (var line in fileLines)
            if (line.StartsWith("Player"))
                currentPlayerNumber++;
            else
                AddUnitToPlayerTeam(currentPlayerNumber, adversaries, line);
        return adversaries;
    }

    private static void AddUnitToPlayerTeam(int playerNumber, Adversaries adversaries, string line)
    {
        var newUnit = UnitLoader.GetUnitFromTextLine(line);
        var player1Team = adversaries.Player1.Team;
        var player2Team = adversaries.Player2.Team;
        if (playerNumber == 1)
            player1Team.AddUnit(newUnit);
        else if (playerNumber == 2)
            player2Team.AddUnit(newUnit);
    }
}