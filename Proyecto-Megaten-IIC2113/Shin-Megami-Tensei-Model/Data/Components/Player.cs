using Shin_Megami_Tensei_Model.Enum;

namespace Shin_Megami_Tensei_Model.Data.Components;

public class Player(int number)
{
    public readonly Team Team = new();
    public int Number { get; } = number;
    public bool HasSurrendered { get; private set; } = false;
    private int _fullTurns = 4;
    private int _blinkingTurns = 0;

    public bool IsDefeated()
    {
        return Team.All(unit => !unit.IsAlive());
    }
    
    public void Surrender()
    {
        HasSurrendered = true;
    }
    
    public void ResetTurns()
    {
        _fullTurns = 4;
        _blinkingTurns = 0;
    }
    
    public bool HasTurns()
    {
        return _fullTurns > 0 || _blinkingTurns > 0;
    }
    
    public void ConsumeTurn(TurnType turnType)
    {
        if (turnType == TurnType.Full)
        {
            if (_fullTurns > 0)
                _fullTurns--;
            else if (_blinkingTurns > 0)
                _blinkingTurns--;
        }
        else if (turnType == TurnType.Blinking && _blinkingTurns > 0)
        {
            _blinkingTurns--;
        }
    }
    
    public int GetFullTurns()
    {
        return _fullTurns;
    }
    
    public int GetBlinkingTurns()
    {
        return _blinkingTurns;
    }
    
    public void AddBlinkingTurn()
    {
        _blinkingTurns++;
    }
    
    public string GetSamuraiName()
    {
        var samurai = Team.GetSamurai();
        return samurai?.Name ?? "";
    }
}
