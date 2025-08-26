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
            var teamsFolder = "../data/E1-BasicCombat";
            var basePath = Path.GetDirectoryName(teamsFolder) ?? ".";
            Console.WriteLine($"BasePath: {basePath}");
            
            UnitInfoLoader.SetBasePath(basePath);
            SkillFactory.SetBasePath(basePath);
            
            // Debug: Ver qué se está cargando del JSON
            Console.WriteLine("=== DEBUG JSON LOADING ===");
            try
            {
                var samuraiPath = Path.Combine(basePath, "samurai.json");
                var samuraiJson = File.ReadAllText(samuraiPath);
                Console.WriteLine($"Samurai JSON loaded, length: {samuraiJson.Length}");
                
                var samurais = System.Text.Json.JsonSerializer.Deserialize<dynamic[]>(samuraiJson);
                Console.WriteLine($"Samurais deserialized: {samurais?.Length ?? 0}");
                
                if (samurais != null)
                {
                    Console.WriteLine("First few samurais:");
                    for (int i = 0; i < Math.Min(3, samurais.Length); i++)
                    {
                        var samurai = samurais[i];
                        Console.WriteLine($"  [{i}]: {samurai}");
                    }
                    
                    // Buscar específicamente Flynn
                    Console.WriteLine("Searching for Flynn:");
                    bool foundFlynn = false;
                    for (int i = 0; i < samurais.Length; i++)
                    {
                        var samurai = samurais[i];
                        var name = samurai.GetProperty("name").GetString();
                        if (name == "Flynn")
                        {
                            Console.WriteLine($"  Found Flynn at index {i}: {samurai}");
                            foundFlynn = true;
                            break;
                        }
                    }
                    if (!foundFlynn)
                    {
                        Console.WriteLine("  Flynn NOT FOUND in samurai.json!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading samurai JSON: {ex.Message}");
            }
            Console.WriteLine("=== END JSON DEBUG ===");
            
            // 2. Cargar archivo de equipos
            var fileLines = File.ReadAllLines(Path.Combine(teamsFolder, "001.txt"));
            Console.WriteLine($"File lines loaded: {fileLines.Length}");
            Console.WriteLine("File content:");
            for (int i = 0; i < fileLines.Length; i++)
            {
                Console.WriteLine($"  [{i}]: {fileLines[i]}");
            }
            
            // Debug: Ver qué se está parseando
            Console.WriteLine("\n=== DEBUG PARSING ===");
            var player1Line = fileLines[1]; // "[Samurai] Flynn (Agi,Agilao,Agidyne)"
            Console.WriteLine($"Player 1 line: '{player1Line}'");
            
            // Extraer nombre y tipo
            var match = System.Text.RegularExpressions.Regex.Match(player1Line, @"\[(\w+)\]\s+(\w+)\s+\(([^)]+)\)");
            if (match.Success)
            {
                var unitType = match.Groups[1].Value;
                var unitName = match.Groups[2].Value;
                var skills = match.Groups[3].Value;
                Console.WriteLine($"  UnitType: '{unitType}'");
                Console.WriteLine($"  UnitName: '{unitName}'");
                Console.WriteLine($"  Skills: '{skills}'");
                
                // Verificar si es samurai
                bool isSamurai = unitType.Equals("Samurai", StringComparison.OrdinalIgnoreCase);
                Console.WriteLine($"  IsSamurai: {isSamurai}");
            }
            else
            {
                Console.WriteLine("  Failed to parse player line");
            }
            
            // Debug: Simular exactamente FindUnitInfoWithName
            Console.WriteLine("\n=== DEBUG FindUnitInfoWithName ===");
            var allUnits = new List<dynamic>();
            
            // Cargar samurais
            var samuraiPath2 = Path.Combine(basePath, "samurai.json");
            var samuraiJson2 = File.ReadAllText(samuraiPath2);
            var samurais2 = System.Text.Json.JsonSerializer.Deserialize<dynamic[]>(samuraiJson2);
            Console.WriteLine($"Samurais loaded: {samurais2?.Length ?? 0}");
            
            // Simular el proceso de FindUnitInfoWithName
            string searchName = "Flynn";
            bool searchIsSamurai = true;
            Console.WriteLine($"Searching for: Name='{searchName}', IsSamurai={searchIsSamurai}");
            
            bool found = false;
            for (int i = 0; i < samurais2.Length; i++)
            {
                var unit = samurais2[i];
                var unitName = unit.GetProperty("name").GetString();
                bool unitIsSamurai = true; // Los samurais siempre son true
                
                Console.WriteLine($"  Checking unit[{i}]: Name='{unitName}', IsSamurai={unitIsSamurai}");
                
                if (unitName == searchName && unitIsSamurai == searchIsSamurai)
                {
                    Console.WriteLine($"  ✓ MATCH FOUND at index {i}!");
                    found = true;
                    break;
                }
            }
            
            if (!found)
            {
                Console.WriteLine("  ❌ NO MATCH FOUND - This explains the UnitInfoNotFoundException!");
            }
            
            // 3. Procesar línea por línea
            foreach (var line in fileLines)
            {
                Console.WriteLine($"Processing line: '{line}'");
                
                if (line.StartsWith("Player"))
                {
                    Console.WriteLine($"  -> Player line, skipping");
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
            Console.WriteLine("Creating adversaries...");
            var adversaries = GameLoader.LoadPlayersTextLines(fileLines);
            Console.WriteLine($"Adversaries created: Player1 has {adversaries.Player1.Team.Count} units, Player2 has {adversaries.Player2.Team.Count} units");
            
            // 6. Validar equipos
            Console.WriteLine("Validating teams...");
            TeamsValidator.ValidateTeams(adversaries.Attacker.Team, adversaries.Defender.Team);
            Console.WriteLine("Teams validation passed!");
            
            Console.WriteLine("=== SUCCESS ===");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"=== ERROR ===");
            Console.WriteLine($"Type: {ex.GetType().Name}");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}
