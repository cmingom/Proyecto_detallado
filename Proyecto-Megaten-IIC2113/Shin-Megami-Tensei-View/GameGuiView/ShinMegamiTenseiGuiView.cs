using Shin_Megami_Tensei_Model.Data.Components;
using Shin_Megami_Tensei_Model.Data.Log;

namespace Shin_Megami_Tensei_View.GameGuiView;

public class ShinMegamiTenseiGuiView : IShinMegamiTenseiView
{
    public string[] GetTeamFileContent(string teamsFolder)
    {
        throw new NotImplementedException();
    }

    public void ShowTeamFileNotValid()
    {
        throw new NotImplementedException();
    }

    public void ShowTeams(Adversaries adversaries)
    {
        throw new NotImplementedException();
    }

    public Unit SelectPlayerUnit(Player player)
    {
        throw new NotImplementedException();
    }

    public void ShowStartBattle(Battle battle)
    {
        throw new NotImplementedException();
    }

    public void ShowAttack(Unit unit, int damage, GameLog gameLog)
    {
        throw new NotImplementedException();
    }

    public void ShowFinishBattleMsg(Battle battle)
    {
        throw new NotImplementedException();
    }

    public void ShowGameResult(Adversaries adversaries)
    {
        throw new NotImplementedException();
    }
}