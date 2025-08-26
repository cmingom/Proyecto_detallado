namespace Shin_Megami_Tensei_Model.Data.Modifiers;

public class StatHp
{
    private readonly int _maxValue;
    private int _currentValue;

    public StatHp(int maxValue)
    {
        _maxValue = maxValue;
        _currentValue = maxValue;
    }

    public int Value => _currentValue;
    public int MaxValue => _maxValue;

    public void TakeDamage(int damage)
    {
        _currentValue = Math.Max(0, _currentValue - damage);
    }

    public void Heal(int amount)
    {
        _currentValue = Math.Min(_maxValue, _currentValue + amount);
    }

    public void ResetEffects()
    {
        // En E1 no hay efectos especiales para HP
    }
}
