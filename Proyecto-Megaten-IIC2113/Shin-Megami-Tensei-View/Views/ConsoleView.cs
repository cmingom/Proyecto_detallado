namespace Shin_Megami_Tensei_View.ConsoleLib;


public class ConsoleView : AbstractView
{
    private const string INPUT_PROMPT = "INPUT: ";
    
    public ConsoleView()
    {
    }

    protected override void Write(object text)
    {
        Console.Write(text);
    }

    protected override string GetNextInput()
    {
        Console.Write(INPUT_PROMPT);
        var input = Console.ReadLine() ?? string.Empty;
        return input;
    }
}
