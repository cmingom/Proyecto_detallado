namespace Shin_Megami_Tensei_View.ConsoleLib;

public abstract class AbstractView
{
    private readonly Script script = new();
    
    public void WriteLine(object text)
        => Write($"{text}\n");

    protected virtual void Write(object text)
        => script.AddToScript(text.ToString());

    public string ReadLine()
    {
        string nextInput = GetNextInput();
        script.AddInput(nextInput);
        return nextInput;
    }
    
    protected abstract string GetNextInput();
    
    public void ExportScript(string path)
        => script.ExportScript(path);

    public string[] GetScript()
        => script.GetScript().Split("\n");
}