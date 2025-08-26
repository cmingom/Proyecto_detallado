using Shin_Megami_Tensei_Model.Data.Components;


namespace Shin_Megami_Tensei_View.GameConsoleView;

public class GameView(View view)
{
    public void ShowTeams(Adversaries adversaries)
    {
        view.WriteLine("Campo de batalla:");
        view.WriteLine($"Player 1 team: {GetTeamNames(adversaries.Attacker.Team)}");
        view.WriteLine($"Player 2 team: {GetTeamNames(adversaries.Defender.Team)}");
    }

    private string GetTeamNames(Team team)
    {
        var names = team.Select(unit => unit.Name);
        return string.Join(", ", names);
    }

    public Unit SelectPlayerUnit(Player player)
    {
        view.WriteLine($"Player {player.Number} selecciona una unidad");
        var aliveUnits = player.Team.Where(unit => unit.IsAlive()).ToList();
        
        for (int i = 0; i < aliveUnits.Count; i++)
        {
            var unit = aliveUnits[i];
            view.WriteLine($"{i}: {unit.Name}");
        }
        
        var selection = Convert.ToInt32(view.ReadLine());
        return aliveUnits[selection];
    }

    public void ShowGameResult(Adversaries adversaries)
    {
        var winner = adversaries.Attacker.IsDefeated() ? 2 : 1;
        view.WriteLine($"Player {winner} ganó");
    }
}