using Shin_Megami_Tensei_Model.Data.Components;
using Shin_Megami_Tensei_Model.SetUp.Loader;
using Shin_Megami_Tensei_Model.SetUp.Validator;
using Shin_Megami_Tensei_View;

namespace Shin_Megami_Tensei;

public class GameSetUp(IShinMegamiTenseiView view)
{
    public Adversaries LoadPlayers(string teamsFolder)
    {
        // Configurar la ruta base para los archivos JSON
        var basePath = GetBasePathFromTeamsFolder(teamsFolder);
        UnitInfoLoader.SetBasePath(basePath);
        SkillFactory.SetBasePath(basePath);
        
        var fileContent = view.GetTeamFileContent(teamsFolder);
        var adversaries = GameLoader.LoadPlayersTextLines(fileContent);
        TeamsValidator.ValidateTeams(adversaries.Attacker.Team, adversaries.Defender.Team);
        return adversaries;
    }

    private string GetBasePathFromTeamsFolder(string teamsFolder)
    {
        // Si teamsFolder es "data/E1-BasicCombat", la ruta base es "data"
        // Si teamsFolder es "data/E2-AffinityAndBasicSkills", la ruta base es "data"
        var directory = Path.GetDirectoryName(teamsFolder);
        return directory ?? ".";
    }

    public Battle NewBattle(Adversaries adversaries, int roundNumber)
    {
        var atkUnit = view.SelectPlayerUnit(adversaries.Attacker);
        var defUnit = view.SelectPlayerUnit(adversaries.Defender);
        return new Battle(atkUnit, defUnit, roundNumber);
    }
}