using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei_Model.CombatSystem.Core;
using Shin_Megami_Tensei_Model.CombatSystem.Setup;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;

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
        if (IsSpecificFile(teamsPath))
        {
            SetSpecificFileAndFolder(teamsPath);
        }
        else
        {
            SetTeamsFolder(teamsPath);
        }
    }

    private bool IsSpecificFile(string teamsPath)
    {
        return teamsPath.EndsWith(".txt");
    }

    private void SetSpecificFileAndFolder(string teamsPath)
    {
        this.specificTeamsFile = teamsPath;
        this.teamsFolder = Path.GetDirectoryName(teamsPath) ?? "";
    }

    private void SetTeamsFolder(string teamsPath)
    {
        this.teamsFolder = teamsPath;
    }
    
    private void ShowTeamFiles(string[] files)
    {
        ShowTeamSelectionHeader();
        ShowFileOptions(files);
    }

    private void ShowTeamSelectionHeader()
    {
        view.WriteLine("Elige un archivo para cargar los equipos");
    }

    private void ShowFileOptions(string[] files)
    {
        for (int i = 0; i < files.Length; i++)
        {
            ShowFileOption(i, files[i]);
        }
    }

    private void ShowFileOption(int index, string filePath)
    {
        view.WriteLine($"{index}: {Path.GetFileName(filePath)}");
    }

    private bool TrySelectFile(string[] files, out string selectedFile)
    {
        selectedFile = string.Empty;
        var input = GetUserInput();
        if (!IsValidFileIndex(input, files.Length))
        {
            return false;
        }
        selectedFile = GetSelectedFile(files, input);
        return true;
    }

    private string GetUserInput()
    {
        return view.ReadLine();
    }

    private string GetSelectedFile(string[] files, string input)
    {
        return files[int.Parse(input)];
    }
    
    private bool IsValidFileIndex(string input, int filesLength)
    {
        return int.TryParse(input, out int index) && index >= 0 && index < filesLength;
    }

    public void Play()
    {
        var file = GetTeamsFile();
        if (IsInvalidFile(file))
        {
            ShowInvalidFileMessage();
            return;
        }
        
        var battleState = CreateBattleState(file);
        var playerNames = GetPlayerNames(file);
        
        StartBattle(battleState, playerNames);
    }

    private bool IsInvalidFile(string file)
    {
        return string.IsNullOrEmpty(file);
    }

    private void ShowInvalidFileMessage()
    {
        view.WriteLine("Archivo de equipos inválido");
    }
    
    private string GetTeamsFile()
    {
        if (HasSpecificFile())
        {
            return GetSpecificFile();
        }
        
        return GetFileFromUserSelection();
    }

    private bool HasSpecificFile()
    {
        return specificTeamsFile != null;
    }

    private string GetSpecificFile()
    {
        return specificTeamsFile;
    }

    private string GetFileFromUserSelection()
    {
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
        var lines = ReadFileLines(file);
        var (team1, team2) = ParseTeamsFromLines(lines);
        
        if (!AreTeamsValid(team1, team2))
        {
            return null;
        }
        
        return CreateBattleStateFromValidTeams(file);
    }

    private string[] ReadFileLines(string file)
    {
        return File.ReadAllLines(file);
    }

    private (List<string> team1, List<string> team2) ParseTeamsFromLines(string[] lines)
    {
        return TeamParser.ParseTeams(lines);
    }

    private bool AreTeamsValid(List<string> team1, List<string> team2)
    {
        return gameService.ValidateTeams(team1, team2);
    }

    private BattleState CreateBattleStateFromValidTeams(string file)
    {
        var (parsedTeam1, parsedTeam2) = gameService.ParseTeamsFromFile(file);
        var gameSetup = new GameSetup(gameService.GetUnitData());
        
        return gameSetup.CreateBattleState(parsedTeam1, parsedTeam2);
    }
    
    private (string player1Name, string player2Name) GetPlayerNames(string file)
    {
        var (parsedTeam1, parsedTeam2) = ParseTeamsFromFile(file);
        var gameSetup = CreateGameSetup();
        
        return gameSetup.GetPlayerNames(parsedTeam1, parsedTeam2);
    }

    private (List<UnitInfo> team1, List<UnitInfo> team2) ParseTeamsFromFile(string file)
    {
        return gameService.ParseTeamsFromFile(file);
    }

    private GameSetup CreateGameSetup()
    {
        return new GameSetup(gameService.GetUnitData());
    }
    
    private void StartBattle(BattleState battleState, (string player1Name, string player2Name) playerNames)
    {
        if (IsInvalidBattleState(battleState))
        {
            ShowInvalidFileMessage();
            return;
        }
        
        var battleEngine = CreateBattleEngine();
        battleEngine.StartBattle(battleState, playerNames.player1Name, playerNames.player2Name);
    }

    private bool IsInvalidBattleState(BattleState battleState)
    {
        return battleState == null;
    }

    private BattleEngine CreateBattleEngine()
    {
        return new BattleEngine(view, gameService.GetSkillData());
    }
}