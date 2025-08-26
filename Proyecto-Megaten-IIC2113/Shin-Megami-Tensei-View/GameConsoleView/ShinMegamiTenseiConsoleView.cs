using Shin_Megami_Tensei_Model.Data.Components;
using Shin_Megami_Tensei_Model.Data.Log;

namespace Shin_Megami_Tensei_View.GameConsoleView;

public class ShinMegamiTenseiConsoleView(View view) : IShinMegamiTenseiView
{
    private readonly LoaderView _loaderView = new(view);
    private readonly GameView _gameView = new(view);
    private readonly BattleView _battleView = new(view);

    public string[] GetTeamFileContent(string teamsFolder)
    {
        return _loaderView.GetTeamFileContent(teamsFolder);
    }

    public void ShowTeamFileNotValid()
    {
        _loaderView.ShowTeamFileNotValid();
    }

    public void ShowTeams(Adversaries adversaries)
    {
        _gameView.ShowTeams(adversaries);
    }

    public Unit SelectPlayerUnit(Player player)
    {
        return _gameView.SelectPlayerUnit(player);
    }

    public void ShowStartBattle(Battle battle)
    {
        _battleView.ShowStartBattle(battle);
    }

    public void ShowAttack(Unit unit, int damage, GameLog gameLog)
    {
        _battleView.ShowAttack(unit, damage, gameLog);
    }

    public void ShowFinishBattleMsg(Battle battle)
    {
        _battleView.ShowFinishBattleMsg(battle);
    }

    public void ShowGameResult(Adversaries adversaries)
    {
        _gameView.ShowGameResult(adversaries);
    }
}