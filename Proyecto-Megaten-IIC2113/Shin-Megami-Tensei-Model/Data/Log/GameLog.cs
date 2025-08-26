using Shin_Megami_Tensei_Model.Data.Components;

namespace Shin_Megami_Tensei_Model.Data.Log;

public class GameLog
{
    private readonly Adversaries _adversaries;
    private readonly List<Battle> _battles = [];

    public GameLog(Adversaries adversaries)
    {
        _adversaries = adversaries;
    }

    public Battle CurrentBattle => _battles.Last();

    public void AddBattle(Battle battle)
    {
        _battles.Add(battle);
    }

    public Unit GetUnitCurrentRival(Unit unit)
    {
        if (CurrentBattle.AtkUnit == unit)
            return CurrentBattle.DefUnit;
        if (CurrentBattle.DefUnit == unit)
            return CurrentBattle.AtkUnit;
        throw new InvalidOperationException("Unit not found in current battle");
    }
}
