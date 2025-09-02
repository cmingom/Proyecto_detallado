namespace Shin_Megami_Tensei_View.ConsoleLib;

class Script
{
    private const string INPUT_KEYWORD = "INPUT: ";
    private const string NEWLINE = "\n";
    
    private string scriptContent = "";
    
    public void AddInput(string inputFromUser)
        => AddToScript($"{INPUT_KEYWORD}{inputFromUser}{NEWLINE}");
    
    public void AddToScript(string message)
        => scriptContent += message;

    public string GetScript()
        => scriptContent;
    
    public void ExportScript(string outputPath) 
        => File.WriteAllText(outputPath, scriptContent);
}