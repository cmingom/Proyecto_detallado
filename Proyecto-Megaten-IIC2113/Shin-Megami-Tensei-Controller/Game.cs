using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Shin_Megami_Tensei_View;
using System.Text.RegularExpressions;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Rules;
using Shin_Megami_Tensei_Model.CombatSystem.Setup;

namespace Shin_Megami_Tensei;

public class Game
{
    private View view;
    private string teamsFolder;
    private string? specificTeamsFile;
    private Dictionary<string, Unit> unitData = new(StringComparer.Ordinal);
    private Dictionary<string, Skill> skillData = new(StringComparer.Ordinal);
    private HashSet<string> skillSet = new(StringComparer.Ordinal);

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
        LoadReferenceData();
    }

    private void LoadReferenceData()
    {
        unitData.Clear();

        var samurais = LoadUnitsFromJson("data/samurai.json");
        var monsters = LoadUnitsFromJson("data/monsters.json");

        foreach (var unit in samurais.Concat(monsters))
        {
            if (!string.IsNullOrWhiteSpace(unit.Name))
                unitData[unit.Name] = unit;
        }

        var skills = LoadSkillsFromJson("data/skills.json");
        skillSet = new(skills.Where(s => !string.IsNullOrWhiteSpace(s.Name))
                           .Select(s => s.Name), StringComparer.Ordinal);
        
        foreach (var skill in skills)
        {
            if (!string.IsNullOrWhiteSpace(skill.Name))
                skillData[skill.Name] = skill;
        }
    }

    private List<Unit> LoadUnitsFromJson(string filePath)
    {
        return JsonSerializer.Deserialize<List<Unit>>(File.ReadAllText(filePath)) ?? new();
    }

    private List<Skill> LoadSkillsFromJson(string filePath)
    {
        return JsonSerializer.Deserialize<List<Skill>>(File.ReadAllText(filePath)) ?? new();
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

        var validator = new TeamValidator(
            unitName => unitData.ContainsKey(unitName),
            skillName => skillSet.Contains(skillName));

        if (!validator.IsValidTeam(team1) || !validator.IsValidTeam(team2))
        {
            view.WriteLine("Archivo de equipos inválido");
            return;
        }

        var parsedTeam1 = BuildUnitInfoList(team1);
        var parsedTeam2 = BuildUnitInfoList(team2);
        
        var gameSetup = new GameSetup(unitData);
        var battleState = gameSetup.CreateBattleState(parsedTeam1, parsedTeam2);
        var (player1Name, player2Name) = gameSetup.GetPlayerNames(parsedTeam1, parsedTeam2);
        
        var battleEngine = new BattleEngine(view, skillData);
        battleEngine.StartBattle(battleState, player1Name, player2Name);
    }

    private List<UnitInfo> BuildUnitInfoList(List<string> teamLines)
    {
        var units = new List<UnitInfo>();
        
        foreach (var line in teamLines)
        {
            var unitInfo = ParseUnitDefinition(line);
            if (unitInfo != null)
            {
                units.Add(unitInfo);
            }
        }
        
        return units;
    }

    
    private UnitInfo? ParseUnitDefinition(string line)
    {
        bool isSamurai = line.Contains("[Samurai]", StringComparison.Ordinal);
        
        if (isSamurai)
        {
            return ParseSamuraiDefinition(line);
        }
        else
        {
            return ParseMonsterDefinition(line);
        }
    }

    private UnitInfo? ParseSamuraiDefinition(string line)
    {
        var rest = line.Replace("[Samurai]", string.Empty).Trim();
        var skills = new List<string>();

        int openParen = rest.IndexOf('(');
        string name;

        if (openParen >= 0)
        {
            int closeParen = rest.IndexOf(')', openParen + 1);
            if (closeParen < 0) return null;

            name = rest.Substring(0, openParen).Trim();
            var skillsText = rest.Substring(openParen + 1, closeParen - openParen - 1);
            
            if (!string.IsNullOrWhiteSpace(skillsText))
            {
                skills = ParseSkillList(skillsText);
                if (skills == null) return null;
            }

            if (!string.IsNullOrWhiteSpace(rest.Substring(closeParen + 1)))
                return null;
        }
        else
        {
            name = rest;
        }

        return new UnitInfo(name, true, skills);
    }

    private UnitInfo? ParseMonsterDefinition(string line)
    {
        if (HasInvalidCharacters(line))
            return null;
        
        return new UnitInfo(line, false, new List<string>());
    }

    private List<string>? ParseSkillList(string skillsText)
    {
        if (string.IsNullOrWhiteSpace(skillsText))
            return new List<string>();

        var skills = new List<string>();
        var parts = skillsText.Split(',');
        
        foreach (var part in parts)
        {
            var skillName = part.Trim();
            if (string.IsNullOrWhiteSpace(skillName))
                return null;
            skills.Add(skillName);
        }
        
        return skills;
    }

    private static bool HasInvalidCharacters(string line)
        => line.Contains('(') || line.Contains(')') || line.Contains(':');

    public class UnitInfo
    {
        public string Name { get; }
        public bool IsSamurai { get; }
        public List<string> Skills { get; }

        public UnitInfo(string name, bool isSamurai, List<string> skills)
        {
            Name = name;
            IsSamurai = isSamurai;
            Skills = skills;
        }
    }


}