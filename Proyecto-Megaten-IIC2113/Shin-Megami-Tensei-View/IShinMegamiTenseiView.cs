using Shin_Megami_Tensei_Model.Data.Components;
using Shin_Megami_Tensei_Model.Data.Log;

namespace Shin_Megami_Tensei_View;

public interface IShinMegamiTenseiView
{
    public string[] GetTeamFileContent(string teamsFolder);

    public void ShowTeamFileNotValid();

    public void ShowTeams(Adversaries adversaries);

    public Unit SelectPlayerUnit(Player player);

    public void ShowStartBattle(Battle battle);

    public void ShowAttack(Unit unit, int damage, GameLog gameLog);

    public void ShowFinishBattleMsg(Battle battle);

    public void ShowGameResult(Adversaries adversaries);
}