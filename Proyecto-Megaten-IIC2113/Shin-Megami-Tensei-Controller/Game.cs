using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei_Model.CombatSystem.Core;
using Shin_Megami_Tensei_Model.CombatSystem.Setup;
using Shin_Megami_Tensei_Model.Domain.States;

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
        InitializeTeamsPath(teamsPath);
        this.gameService = new GameService();
        this.gameService.LoadReferenceData();
    }
    
    private void InitializeTeamsPath(string teamsPath)
    {
        if (teamsPath.EndsWith(".txt"))
        {
            this.specificTeamsFile = teamsPath;
            this.teamsFolder = Path.GetDirectoryName(teamsPath) ?? "";
        }
        else
        {
            this.teamsFolder = teamsPath;
        }
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
        var input = view.ReadLine();
        if (!IsValidFileIndex(input, files.Length))
        {
            return false;
        }
        selectedFile = files[int.Parse(input)];
        return true;
    }
    
    private bool IsValidFileIndex(string input, int filesLength)
    {
        return int.TryParse(input, out int index) && index >= 0 && index < filesLength;
    }

    public void Play()
    {
        var file = GetTeamsFile();
        if (string.IsNullOrEmpty(file))
        {
            view.WriteLine("Archivo de equipos inválido");
            return;
        }
        
        var battleState = CreateBattleState(file);
        var playerNames = GetPlayerNames(file);
        
        StartBattle(battleState, playerNames);
    }
    
    private string GetTeamsFile()
    {
        if (specificTeamsFile != null)
        {
            return specificTeamsFile;
        }
        
        var files = GetTeamFiles();
        ShowTeamFiles(files);
        
        if (!TrySelectFile(files, out string selectedFile))
        {
            return string.Empty;
        }
        
        return selectedFile;
    }
    
    private string[] GetTeamFiles()
    {
        return Directory.GetFiles(teamsFolder, "*.txt").OrderBy(f => f).ToArray();
    }
    
    private BattleState CreateBattleState(string file)
    {
        var lines = File.ReadAllLines(file);
        var (team1, team2) = TeamParser.ParseTeams(lines);
        
        if (!gameService.ValidateTeams(team1, team2))
        {
            return null;
        }
        
        var (parsedTeam1, parsedTeam2) = gameService.ParseTeamsFromFile(file);
        var gameSetup = new GameSetup(gameService.GetUnitData());
        
        return gameSetup.CreateBattleState(parsedTeam1, parsedTeam2);
    }
    
    private (string player1Name, string player2Name) GetPlayerNames(string file)
    {
        var (parsedTeam1, parsedTeam2) = gameService.ParseTeamsFromFile(file);
        var gameSetup = new GameSetup(gameService.GetUnitData());
        
        return gameSetup.GetPlayerNames(parsedTeam1, parsedTeam2);
    }
    
    private void StartBattle(BattleState battleState, (string player1Name, string player2Name) playerNames)
    {
        if (battleState == null)
        {
            view.WriteLine("Archivo de equipos inválido");
            return;
        }
        
        var battleEngine = new BattleEngine(view, gameService.GetSkillData());
        battleEngine.StartBattle(battleState, playerNames.player1Name, playerNames.player2Name);
    }
}