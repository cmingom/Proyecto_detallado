using System.IO;
using System.Text;
using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
Console.InputEncoding  = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
Console.OutputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
Console.SetOut(new StreamWriter(Console.OpenStandardOutput(), Console.OutputEncoding) { AutoFlush = true });

if (args.Length >= 2)
{
    string teamsFolderArg = args[0];
    string testFile = args[1];
    var testingView = View.BuildTestingView(testFile);
    var testGame = new GameController(testingView, teamsFolderArg);
    try
    {
        testGame.Play();
    }
    finally
    {
        foreach (var line in testingView.GetScript())
        {
            Console.WriteLine(line);
        }
    }
    return;
}



/* 
 * Este código permite replicar un test case. Primero pregunta por el grupo de test
 * case a replicar. Luego pregunta por el test case específico que se quiere replicar.
 *
 * Al presionar enter, se ingresa el input del test case en forma automática. Si el
 * color es azul significa que el output de tu programa es el esperado. Si es rojo
 * significa que el output de tu programa es distinto al esperado (i.e., el test falló).
 */

// Verificar si se proporcionó archivo de equipos como argumento de línea de comandos (para tests)
if (args.Length > 0)
{
    string teamsFile = args[0];
    var view = View.BuildConsoleView();
    var gameController = new GameController(view, teamsFile);
    gameController.Play();
}
else
{
    // Modo interactivo
    string testFolder = SelectTestFolder();
    string test = SelectTest(testFolder);
    string teamsFolder = testFolder.Replace("-Tests","");
    AnnounceTestCase(test);

    var view = View.BuildManualTestingView(test);
    var gameController = new GameController(view, teamsFolder);
    gameController.Play();
}

string SelectTestFolder()
{
    Console.WriteLine("¿Qué grupo de test quieres usar?");
    string[] dirs = GetAvailableTestsInOrder();
    ShowArrayOfOptions(dirs);
    return AskUserToSelectAnOption(dirs);
}

string[] GetAvailableTestsInOrder()
{
    string[] dirs = Directory.GetDirectories("data", "*-Tests", SearchOption.TopDirectoryOnly);
    Array.Sort(dirs);
    return dirs;
}

void ShowArrayOfOptions(string[] options)
{
    for(int i = 0; i < options.Length; i++)
        Console.WriteLine($"{i}- {options[i]}");
}

string AskUserToSelectAnOption(string[] options)
{
    int minValue = 0;
    int maxValue = options.Length - 1;
    int selectedOption = AskUserToSelectNumber(minValue, maxValue);
    return options[selectedOption];
}

int AskUserToSelectNumber(int minValue, int maxValue)
{
    Console.WriteLine($"(Ingresa un número entre {minValue} y {maxValue})");
    int value;
    bool wasParsePossible;
    do
    {
        string? userInput = Console.ReadLine();
        wasParsePossible = int.TryParse(userInput, out value);
    } while (!wasParsePossible || IsValueOutsideTheValidRange(minValue, value, maxValue));

    return value;
}

bool IsValueOutsideTheValidRange(int minValue, int value, int maxValue)
    => value < minValue || value > maxValue;

string SelectTest(string testFolder)
{
    Console.WriteLine("¿Qué test quieres ejecutar?");
    string[] tests = Directory.GetFiles(testFolder, "*.txt" );
    Array.Sort(tests);
    return AskUserToSelectAnOption(tests);
}

void AnnounceTestCase(string test)
{
    Console.WriteLine($"----------------------------------------");
    Console.WriteLine($"Replicando test: {test}");
    Console.WriteLine($"----------------------------------------\n");
}
