namespace Shin_Megami_Tensei_View.ConsoleLib;


public class ConsoleView : AbstractView
{
    private const string INPUT_PROMPT = "INPUT: ";
    
    public ConsoleView()
    {
        System.Console.WriteLine("*** DEBUG: ConsoleView CONSTRUCTOR LLAMADO ***");
        System.Console.WriteLine("*** DEBUG: ConsoleView CONSTRUCTOR LLAMADO ***");
        System.Console.WriteLine("*** DEBUG: ConsoleView CONSTRUCTOR LLAMADO ***");
    }

    protected override void Write(object text)
    {
        System.Console.WriteLine($"DEBUG ConsoleView: Write() - {text}");
        Console.Write(text);
    }

    protected override string GetNextInput()
    {
        System.Console.WriteLine("DEBUG ConsoleView: GetNextInput() - esperando input del usuario");
        Console.Write(INPUT_PROMPT);
        var input = Console.ReadLine() ?? string.Empty;
        System.Console.WriteLine($"DEBUG ConsoleView: GetNextInput() - recibido: '{input}'");
        return input;
    }
}
