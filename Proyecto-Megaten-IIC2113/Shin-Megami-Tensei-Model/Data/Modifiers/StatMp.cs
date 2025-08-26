namespace Shin_Megami_Tensei_Model.Data.Modifiers;

public class StatMp
{
    private readonly int _maxValue;
    private int _currentValue;

    public StatMp(int maxValue)
    {
        _maxValue = maxValue;
        _currentValue = maxValue;
    }

    public int Value => _currentValue;
    public int MaxValue => _maxValue;

    public void Consume(int amount)
    {
        _currentValue = Math.Max(0, _currentValue - amount);
    }

    public void Restore(int amount)
    {
        _currentValue = Math.Min(_maxValue, _currentValue + amount);
    }

    public void ResetEffects()
    {
        // En E1 no hay efectos especiales para MP
    }
}
