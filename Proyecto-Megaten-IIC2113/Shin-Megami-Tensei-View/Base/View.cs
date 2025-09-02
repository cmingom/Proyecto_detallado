using Shin_Megami_Tensei_View.ConsoleLib;

namespace Shin_Megami_Tensei_View;

public class View
{
    private readonly AbstractView viewImplementation;

    public static View BuildConsoleView()
        => new View(new ConsoleView());

    public static View BuildTestingView(string pathTestScript)
        => new View(new TestingView(pathTestScript));

    public static View BuildManualTestingView(string pathTestScript)
        => new View(new ManualTestingView(pathTestScript));
    
    private View(AbstractView newView)
    {
        viewImplementation = newView;
    }
    
    public string ReadLine() => viewImplementation.ReadLine();
    
    public void WriteLine(string message)
    {
        viewImplementation.WriteLine(message);
    }
    
    public string[] GetScript()
        => viewImplementation.GetScript();
}