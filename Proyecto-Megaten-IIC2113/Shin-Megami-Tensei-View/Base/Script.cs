namespace Shin_Megami_Tensei_View.ConsoleLib;

class Script
{
    private const string InputKeyword = "INPUT: ";
    private string scriptContent = "";
    
    public void AddInput(string inputFromUser)
        => AddToScript($"{InputKeyword}{inputFromUser}\n");
    
    public void AddToScript(string message)
        => scriptContent += message;

    public string GetScript()
        => scriptContent;
    
    public void ExportScript(string outputPath) 
        => File.WriteAllText(outputPath, scriptContent);
}