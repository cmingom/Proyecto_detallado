using Shin_Megami_Tensei_Model.Data.Components;
using Shin_Megami_Tensei_Model.Data.Log;
using Shin_Megami_Tensei_Model.DataProcessors;
using Shin_Megami_Tensei_Model.Exceptions.TeamFileValidation;
using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei_View.GameConsoleView;
using Shin_Megami_Tensei_View.GameGuiView;


namespace Shin_Megami_Tensei;

public class Game
{
    private readonly string _teamsFolder;
    private readonly IShinMegamiTenseiView _view;
    private Adversaries _adversaries;
    private GameLog _gameLog;
    private BattleExecutor _battleExecutor;
    private GameSetUp _gameSetUp;

    public Game(View view, string teamsFolder)
    {
        _view = new ShinMegamiTenseiConsoleView(view);
        _teamsFolder = teamsFolder;
        InstantiateControllers();
    }

    public Game(ShinMegamiTenseiGuiView view)
    {
        _view = view;
        _teamsFolder = "";
        InstantiateControllers();
    }

    private void InstantiateControllers()
    {
        _battleExecutor = new BattleExecutor(_view);
        _gameSetUp = new GameSetUp(_view);
    }

    public void Play()
    {
        try
        {
            PrepareBattle();
            ExecuteBattle();
        }
        catch (TeamFileValidationException)
        {
            _view.ShowTeamFileNotValid();
        }
    }

    private void PrepareBattle()
    {
        _adversaries = _gameSetUp.LoadPlayers(_teamsFolder);
        _gameLog = new GameLog(_adversaries);
    }

    private void ExecuteBattle()
    {
        for (var roundNumber = 1; !_adversaries.IsOnePlayerDefeated(); roundNumber++)
        {
            _view.ShowTeams(_adversaries);
            var battle = _gameSetUp.NewBattle(_adversaries, roundNumber);
            ExecuteCombat(battle);
        }

        _view.ShowGameResult(_adversaries);
    }

    private void ExecuteCombat(Battle battle)
    {
        _gameLog.AddBattle(battle);
        _battleExecutor.Execute(battle, _gameLog);
        TeamsRefresher.RefreshAdversariesTeams(_adversaries);
        _adversaries.SwapRoles();
    }
}