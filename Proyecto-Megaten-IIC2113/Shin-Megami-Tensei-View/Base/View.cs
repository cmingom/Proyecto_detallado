using Shin_Megami_Tensei_View.ConsoleLib;

namespace Shin_Megami_Tensei_View;

public class View
{
    private readonly AbstractView viewImplementation;
    private readonly List<string> actionBuffer = new List<string>();
    private int bufferDepth = 0;

    public static View BuildConsoleView()
    {
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
        return viewImplementation.ReadLine();
    }
    
    public void WriteLine(string message)
    {
        if (bufferDepth > 0)
        {
            actionBuffer.Add(message);
        }
        else
        {
            viewImplementation.WriteLine(message);
        }
    }

    public void StartActionBuffer()
    {
        bufferDepth++;
        if (bufferDepth == 1)
        {
            actionBuffer.Clear();
        }
    }

    public void FlushActionBuffer()
    {
        if (bufferDepth > 0)
        {
            bufferDepth--;
            if (bufferDepth == 0)
            {
                foreach (var line in actionBuffer)
                {
                    viewImplementation.WriteLine(line);
                }
                actionBuffer.Clear();
            }
        }
    }
    
    public string[] GetScript()
        => viewImplementation.GetScript();
}
