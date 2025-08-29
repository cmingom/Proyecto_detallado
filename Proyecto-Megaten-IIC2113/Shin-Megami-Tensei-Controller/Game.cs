using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei_Model.CombatSystem.Core;
using Shin_Megami_Tensei_Model.CombatSystem.Setup;

namespace Shin_Megami_Tensei;

public class Game
{
    private View view;
    private string teamsFolder;
    private string? specificTeamsFile;
    private GameService gameService;

    public Game(View view, string teamsPath)
    {
        this.view = view;
        if (teamsPath.EndsWith(".txt"))
        {
            this.specificTeamsFile = teamsPath;
            this.teamsFolder = Path.GetDirectoryName(teamsPath) ?? "";
        }
        else
        {
            this.teamsFolder = teamsPath;
        }
        
        this.gameService = new GameService();
        this.gameService.LoadReferenceData();
    }
    
    private void ShowTeamFiles(string[] files)
    {
        view.WriteLine("Elige un archivo para cargar los equipos");
        for (int i = 0; i < files.Length; i++)
        {
            view.WriteLine($"{i}: {Path.GetFileName(files[i])}");
        }
    }

    private bool TrySelectFile(string[] files, out string selectedFile)
    {
        selectedFile = string.Empty;
        if (!int.TryParse(view.ReadLine(), out int index) || index < 0 || index >= files.Length)
        {
            return false;
        }
        selectedFile = files[index];
        return true;
    }

    public void Play()
    {
        string file;
        
        if (specificTeamsFile != null)
        {
            file = specificTeamsFile;
        }
        else
        {
            var files = Directory.GetFiles(teamsFolder, "*.txt").OrderBy(f => f).ToArray();

            ShowTeamFiles(files);

            if (!TrySelectFile(files, out file))
            {
                view.WriteLine("Archivo de equipos inválido");
                return;
            }
        }

        string[] lines = File.ReadAllLines(file);
        var (team1, team2) = TeamParser.ParseTeams(lines);

        if (!gameService.ValidateTeams(team1, team2))
        {
            view.WriteLine("Archivo de equipos inválido");
            return;
        }

        var (parsedTeam1, parsedTeam2) = gameService.ParseTeamsFromFile(file);
        
        var gameSetup = new GameSetup(gameService.GetUnitData());
        var battleState = gameSetup.CreateBattleState(parsedTeam1, parsedTeam2);
        var (player1Name, player2Name) = gameSetup.GetPlayerNames(parsedTeam1, parsedTeam2);
        
        var battleEngine = new BattleEngine(view, gameService.GetSkillData());
        battleEngine.StartBattle(battleState, player1Name, player2Name);
    }
}