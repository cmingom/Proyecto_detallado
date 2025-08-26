namespace Shin_Megami_Tensei_Model.Data.Modifiers;

public class NormalStat(string name, int baseValue)
{
    public string Name { get; } = name;
    private readonly int _baseValue = baseValue;
    private int _bonus = 0;
    private int _penalty = 0;

    public int Value => Math.Max(0, _baseValue + _bonus - _penalty);

    public void AddBonus(int bonus)
    {
        _bonus += bonus;
    }

    public void AddPenalty(int penalty)
    {
        _penalty += penalty;
    }

    public void ResetEffects()
    {
        _bonus = 0;
        _penalty = 0;
    }
}
