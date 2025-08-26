namespace Shin_Megami_Tensei_View.ConsoleLib;

public class ConsoleView : AbstractView
{
    private const string INPUT_PROMPT = "INPUT: ";

    protected override void Write(object text)
    {
        base.Write(text);
        Console.Write(text);
    }

    protected override string GetNextInput()
    {
        Console.Write(INPUT_PROMPT);
        return Console.ReadLine() ?? string.Empty;
    }
}
