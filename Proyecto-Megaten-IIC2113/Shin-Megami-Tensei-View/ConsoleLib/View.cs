using Shin_Megami_Tensei_View.ConsoleLib;

namespace Shin_Megami_Tensei_View;

public class View
{
    private readonly AbstractView _view;

    public static View BuildConsoleView()
        => new View(new ConsoleView());

    public static View BuildTestingView(string pathTestScript)
        => new View(new TestingView(pathTestScript));

    public static View BuildManualTestingView(string pathTestScript)
        => new View(new ManualTestingView(pathTestScript));
    
    private View(AbstractView newView)
    {
        _view = newView;
    }
    
    public string ReadLine() => _view.ReadLine();
    
    public void WriteLine(string message)
    {
        _view.WriteLine(message);
    }
    
    public string[] GetScript()
        => _view.GetScript();
    
    public void ShowRoundHeader(string playerName, string playerNumber)
    {
        WriteLine("----------------------------------------");
        WriteLine($"Ronda de {playerName} ({playerNumber})");
    }
    
    public void ShowBattlefield(string player1Name, string player2Name, List<string> team1Info, List<string> team2Info)
    {
        WriteLine("----------------------------------------");
        ShowTeamStatus(player1Name, "J1", team1Info);
        ShowTeamStatus(player2Name, "J2", team2Info);
    }
    
    private const int MaxTeamPositions = 4;

    public void ShowTeamStatus(string playerName, string playerNumber, List<string> unitInfo)
    {
        WriteLine($"Equipo de {playerName} ({playerNumber})");
        
        char[] positions = { 'A', 'B', 'C', 'D' };
        
        for (int i = 0; i < MaxTeamPositions; i++)
        {
            if (i < unitInfo.Count)
            {
                WriteLine($"{positions[i]}-{unitInfo[i]}");
            }
            else
            {
                WriteLine($"{positions[i]}-");
            }
        }
    }
    
    public void ShowTurnCounters(int fullTurns, int blinkingTurns)
    {
        WriteLine("----------------------------------------");
        WriteLine($"Full Turns: {fullTurns}");
        WriteLine($"Blinking Turns: {blinkingTurns}");
    }
    
    public void ShowActionOrder(List<string> unitNames)
    {
        WriteLine("----------------------------------------");
        WriteLine("Orden:");
        
        for (int i = 0; i < unitNames.Count; i++)
        {
            WriteLine($"{i + 1}-{unitNames[i]}");
        }
    }
    
    public void ShowUnitActionMenu(string unitName, List<string> actions)
    {
        WriteLine("----------------------------------------");
        WriteLine($"Seleccione una acción para {unitName}");
        
        for (int i = 0; i < actions.Count; i++)
        {
            WriteLine($"{i + 1}: {actions[i]}");
        }
    }
    
    public void ShowTargetSelection(string attackerName, List<string> targetInfo)
    {
        WriteLine("----------------------------------------");
        WriteLine($"Seleccione un objetivo para {attackerName}");
        
        for (int i = 0; i < targetInfo.Count; i++)
        {
            WriteLine($"{i + 1}-{targetInfo[i]}");
        }
        WriteLine($"{targetInfo.Count + 1}-Cancelar");
    }
    
    public void ShowSkillSelection(string unitName, List<string> skillInfo)
    {
        WriteLine("----------------------------------------");
        WriteLine($"Seleccione una habilidad para que {unitName} use");
        
        for (int i = 0; i < skillInfo.Count; i++)
        {
            WriteLine($"{i + 1}-{skillInfo[i]}");
        }
        WriteLine($"{skillInfo.Count + 1}-Cancelar");
    }
    
    public void ShowAttackResult(string attackerName, string targetName, int damage, bool isGunAttack)
    {
        WriteLine("----------------------------------------");
        
        string attackType = isGunAttack ? "dispara a" : "ataca a";
        WriteLine($"{attackerName} {attackType} {targetName}");
        WriteLine($"{targetName} recibe {damage} de daño");
    }
    
    public void ShowTargetHealthAfterAttack(string targetName, int currentHp, int maxHp)
    {
        WriteLine($"{targetName} termina con HP:{currentHp}/{maxHp}");
    }
    
    public void ShowSurrender(string playerName, string playerNumber, string winnerName, string winnerNumber)
    {
        WriteLine("----------------------------------------");
        WriteLine($"{playerName} ({playerNumber}) se rinde");
        WriteLine("----------------------------------------");
        WriteLine($"Ganador: {winnerName} ({winnerNumber})");
    }
    
    public void ShowTurnConsumption()
    {
        WriteLine("----------------------------------------");
        const int consumedFullTurns = 1;
        const int consumedBlinkingTurns = 0;
        const int obtainedBlinkingTurns = 0;
        WriteLine($"Se han consumido {consumedFullTurns} Full Turn(s) y {consumedBlinkingTurns} Blinking Turn(s)");
        WriteLine($"Se han obtenido {obtainedBlinkingTurns} Blinking Turn(s)");
    }
    
    public void ShowWinner(string winnerName, string winnerNumber)
    {
        WriteLine("----------------------------------------");
        WriteLine($"Ganador: {winnerName} ({winnerNumber})");
    }
    
    public void ShowSeparator()
    {
        WriteLine("----------------------------------------");
    }
    
    public void ShowTeamFiles(string[] files)
    {
        WriteLine("Elige un archivo para cargar los equipos");
        for (int i = 0; i < files.Length; i++)
        {
            WriteLine($"{i}: {Path.GetFileName(files[i])}");
        }
    }
    
    public string SelectTeamFile(string[] files)
    {
        while (true)
        {
            var input = ReadLine();
            if (int.TryParse(input, out int index) && index >= 0 && index < files.Length)
            {
                return files[index];
            }
            WriteLine("Selección inválida, intenta de nuevo");
        }
    }

    public string[] GetTeamFileContent(string teamsFolder, string? specificTeamsFile)
    {
        if (specificTeamsFile != null)
            return File.ReadAllLines(specificTeamsFile);

        var files = Directory.GetFiles(teamsFolder, "*.txt").OrderBy(f => f).ToArray();
        ShowTeamFiles(files);
        
        var selectedFile = SelectTeamFile(files);
        return File.ReadAllLines(selectedFile);
    }
}