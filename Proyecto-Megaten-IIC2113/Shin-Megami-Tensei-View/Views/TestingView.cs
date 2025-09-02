namespace Shin_Megami_Tensei_View.ConsoleLib;

public class TestingView : AbstractView
{
    private const string INPUT_KEYWORD = "INPUT: ";
    private const string EMPTY_STRING = "";
    private const string NO_MORE_INPUTS_MESSAGE = "Tu programa pidió un input pero no hay más inputs del usuario en este test case!";
    
    private readonly string[] _expectedScript;
    private readonly Queue<string> _inputsFromUser = new();
    
    public TestingView(string pathTestScript)
    {
        _expectedScript = File.ReadAllLines(pathTestScript);
        AddInputsFromUser();
    }
    
    private void AddInputsFromUser()
    {
        foreach (string line in _expectedScript)
            if(IsInputFromUser(line))
                _inputsFromUser.Enqueue(line.Replace(INPUT_KEYWORD, EMPTY_STRING));
    }
    
    private bool IsInputFromUser(string line)
        => line.StartsWith(INPUT_KEYWORD);

    protected override string GetNextInput()
    {
        if (_inputsFromUser.Any())
            return _inputsFromUser.Dequeue();
        throw new ApplicationException(NO_MORE_INPUTS_MESSAGE);
    }
}