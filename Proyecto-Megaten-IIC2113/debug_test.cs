using Shin_Megami_Tensei_Model.Data.Components;
using Shin_Megami_Tensei_Model.SetUp.Loader;
using Shin_Megami_Tensei_Model.SetUp.Validator;
using Shin_Megami_Tensei_Model.Exceptions.TeamFileValidation;

class Program
{
    static void Main()
    {
        try
        {
            Console.WriteLine("=== DEBUG TEST ===");
            
            // 1. Configurar rutas base
            var teamsFolder = "data/E1-BasicCombat";
            var basePath = Path.GetDirectoryName(teamsFolder) ?? ".";
            Console.WriteLine($"BasePath: {basePath}");
            
            UnitInfoLoader.SetBasePath(basePath);
            SkillFactory.SetBasePath(basePath);
            
            // 2. Cargar archivo de equipos
            var fileLines = File.ReadAllLines(Path.Combine(teamsFolder, "001.txt"));
            Console.WriteLine($"File lines loaded: {fileLines.Length}");
            Console.WriteLine("File content:");
            for (int i = 0; i < fileLines.Length; i++)
            {
                Console.WriteLine($"  [{i}]: {fileLines[i]}");
            }
            
            // 3. Procesar cada línea
            Console.WriteLine("\n=== DEBUG PARSING ===");
            foreach (var line in fileLines)
            {
                Console.WriteLine($"Processing line: '{line}'");
                
                if (line.StartsWith("Player"))
                {
                    Console.WriteLine("  -> Player line, skipping");
                    continue;
                }
                
                try
                {
                    // 4. Crear unidad
                    Console.WriteLine($"  -> Creating unit from: '{line}'");
                    var unit = UnitLoader.GetUnitFromTextLine(line);
                    Console.WriteLine($"  -> Unit created: {unit.Name}, IsSamurai: {unit.IsSamurai}, Skills: {unit.Skills.Count}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  -> ERROR creating unit: {ex.GetType().Name}: {ex.Message}");
                    return;
                }
            }
            
            // 5. Crear adversaries
            Console.WriteLine("\nCreating adversaries...");
            var adversaries = GameLoader.LoadPlayersTextLines(fileLines);
            Console.WriteLine($"Adversaries created: Player1 has {adversaries.Attacker.Team.Count} units, Player2 has {adversaries.Defender.Team.Count} units");
            
            // 6. Validar equipos
            Console.WriteLine("\nValidating teams...");
            TeamsValidator.ValidateTeams(adversaries.Attacker.Team, adversaries.Defender.Team);
            Console.WriteLine("Teams validation passed!");
            
            Console.WriteLine("\n=== SUCCESS ===");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n=== ERROR ===");
            Console.WriteLine($"Type: {ex.GetType().Name}");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}
