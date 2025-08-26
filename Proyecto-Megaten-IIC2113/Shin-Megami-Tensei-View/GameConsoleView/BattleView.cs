using Shin_Megami_Tensei_Model.Data.Components;
using Shin_Megami_Tensei_Model.Data.Log;


namespace Shin_Megami_Tensei_View.GameConsoleView;

public class BattleView(View view)
{
    public void ShowStartBattle(Battle battle)
    {
        view.WriteLine($"{battle.AtkUnit.Name} y {battle.DefUnit.Name} comienzan una batalla");
    }

    public void ShowAttack(Unit unit, int damage, GameLog gameLog)
    {
        var rivalUnit = gameLog.GetUnitCurrentRival(unit);
        view.WriteLine($"{unit.Name} ataca a {rivalUnit.Name} con {damage} de daño");
    }

    public void ShowFinishBattleMsg(Battle battle)
    {
        // En E1 no hay mensaje especial al finalizar el combate
    }
}