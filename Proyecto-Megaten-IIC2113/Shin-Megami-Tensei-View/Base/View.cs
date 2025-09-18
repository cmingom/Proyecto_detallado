using Shin_Megami_Tensei_View.ConsoleLib;

namespace Shin_Megami_Tensei_View;

public class View
{
    private readonly AbstractView viewImplementation;

    public static View BuildConsoleView()
    {
        System.Console.WriteLine("DEBUG View: BuildConsoleView() - creando ConsoleView");
        return new View(new ConsoleView());
    }

    public static View BuildTestingView(string pathTestScript)
        => new View(new TestingView(pathTestScript));

    public static View BuildManualTestingView(string pathTestScript)
        => new View(new ManualTestingView(pathTestScript));
    
    private View(AbstractView newView)
    {
        viewImplementation = newView;
    }
    
    public string ReadLine() 
    {
        System.Console.WriteLine("DEBUG View: ReadLine() - delegando a viewImplementation");
        return viewImplementation.ReadLine();
    }
    
    public void WriteLine(string message)
    {
        System.Console.WriteLine($"DEBUG View: WriteLine() - '{message}'");
        viewImplementation.WriteLine(message);
    }
    
    public string[] GetScript()
        => viewImplementation.GetScript();
}