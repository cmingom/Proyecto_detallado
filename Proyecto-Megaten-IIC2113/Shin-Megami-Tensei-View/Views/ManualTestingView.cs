namespace Shin_Megami_Tensei_View.ConsoleLib;

public class ManualTestingView : TestingView
{
    private const string END_OF_FILE_STRING = "[EndOfFile]";
    private const string ERROR_PREFIX = "[ERROR]";
    private const string INPUT_TEST_PREFIX = "[INPUT TEST]: ";
    private const string INPUT_MANUAL_PREFIX = "[INPUT MANUAL]: ";
    private const string INPUT_PREFIX = "INPUT: ";
    private const string ERROR_MESSAGE_TEMPLATE = "el valor esperado acá era: \"{0}\"";
    private const string INVALID_INPUT_MESSAGE = "No se debía pedir un input en este momento";
    private const char NEWLINE_CHAR = '\n';
    private const int MINIMUM_TEXT_LENGTH = 0;
    private const int NEWLINE_OFFSET = 1;
    
    private readonly string[] _expectedScript;
    private int _currentLine;
    private bool _isOutputCorrectSoFar = true;
    
    public ManualTestingView(string pathTestScript) : base(pathTestScript)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        _expectedScript = File.ReadAllLines(pathTestScript);
        _currentLine = 0;
    }

    protected override void Write(object text)
    {
        if(_isOutputCorrectSoFar)
            CheckIfCurrentOutputIsAsExpected(text);
        base.Write(text);
        Console.Write(text);
    }

    private void CheckIfCurrentOutputIsAsExpected(object text)
    {
        string normalizedText = GetNormalizedTest(text.ToString());
        string[] lines = normalizedText.Split(NEWLINE_CHAR);
        CheckThatLinesMatchTheExpectedOutput(lines);
    }

    private string GetNormalizedTest(string text)
    {
        return HasTrailingNewline(text) ? RemoveTrailingNewline(text) : text;
    }

    private bool HasTrailingNewline(string text)
    {
        return text.Length > MINIMUM_TEXT_LENGTH && text[^1] == NEWLINE_CHAR;
    }

    private string RemoveTrailingNewline(string text)
    {
        return text.Remove(text.Length - NEWLINE_OFFSET);
    }
    
    private void CheckThatLinesMatchTheExpectedOutput(string[] lines)
    {
        ProcessEachLine(lines);
    }
    
    private void ProcessEachLine(string[] lines)
    {
        for(int i = 0; i < lines.Length; i++)
        {
            if(IsLineDifferentFromExpected(lines[i]))
            {
                HandleOutputError();
                break;
            }
            AdvanceToNextLine();
        }
    }
    
    private bool IsLineDifferentFromExpected(string line)
        => GetExpectedLine() != line;

    private void HandleOutputError()
    {
        SetOutputAsIncorrect();
        SetConsoleColorToRed();
        DisplayErrorMessage();
    }

    private void SetOutputAsIncorrect()
    {
        _isOutputCorrectSoFar = false;
    }

    private void SetConsoleColorToRed()
    {
        Console.ForegroundColor = ConsoleColor.Red;
    }

    private void DisplayErrorMessage()
    {
        string expectedLine = GetExpectedLine();
        string errorMessage = string.Format(ERROR_MESSAGE_TEMPLATE, expectedLine);
        Console.Write($"{ERROR_PREFIX} {errorMessage}");
    }

    private void AdvanceToNextLine()
    {
        _currentLine++;
    }

    private string GetExpectedLine()
    {
        return IsTheEndOfTheExpectedScript() ? END_OF_FILE_STRING : _expectedScript[_currentLine];
    }
    
    private bool IsTheEndOfTheExpectedScript()
        => _currentLine == _expectedScript.Length;

    protected override string GetNextInput()
    {
        try
        {
            return TryToGetInputFromTest();
        }
        catch (InvalidInputRequestException)
        {
            return GetNextInputFromUser();
        }
    }
    
    private string TryToGetInputFromTest()
    {
        string nextInput = base.GetNextInput();
        ValidateInputRequest(nextInput);
        DisplayTestInput(nextInput);
        WaitForUserInput();
        return nextInput;
    }

    private void ValidateInputRequest(string nextInput)
    {
        CheckIfCurrentOutputIsAsExpected($"{INPUT_PREFIX}{nextInput}");
        if (!_isOutputCorrectSoFar)
            throw new InvalidInputRequestException(INVALID_INPUT_MESSAGE);
    }

    private void DisplayTestInput(string nextInput)
    {
        Console.Write($"{INPUT_TEST_PREFIX}{nextInput}");
    }

    private void WaitForUserInput()
    {
        Console.ReadLine();
    }
    
    private string GetNextInputFromUser()
    {
        Console.Write(INPUT_MANUAL_PREFIX);
        return Console.ReadLine();
    }
}