using Shin_Megami_Tensei_Model.Data.Components;
using Shin_Megami_Tensei_Model.Data.Log;
using Shin_Megami_Tensei_Model.DataProcessors;
using Shin_Megami_Tensei_Model.Enum;
using Shin_Megami_Tensei_View;

namespace Shin_Megami_Tensei;

public class BattleExecutor(IShinMegamiTenseiView view)
{
    private Battle _battle;
    private GameLog _gameLog;

    public void Execute(Battle battle, GameLog gameLog)
    {
        _battle = battle;
        _gameLog = gameLog;
        PrepareAttacks();
        ExecuteAttacks();
        FinishBattle();
    }

    private void PrepareAttacks()
    {
        // En E1 no hay skills para aplicar antes del combate
        view.ShowStartBattle(_battle);
    }

    private void ExecuteAttacks()
    {
        ExecuteFirstAttacks();
        // En E1 no hay follow-ups
    }

    private void ExecuteFirstAttacks()
    {
        if (_battle.AtkUnit.IsAlive())
            Attack(_battle.AtkUnit, _battle.DefUnit);
        if (_battle.DefUnit.IsAlive())
            Attack(_battle.DefUnit, _battle.AtkUnit);
    }

    private void Attack(Unit unit, Unit rival)
    {
        var damage = DamageCalculator.CalculatePhysicalDamage(unit, rival);
        HpBattleAdjuster.AdjustHpAfterDamage(rival, damage);
        view.ShowAttack(unit, damage, _gameLog);
    }

    private void FinishBattle()
    {
        view.ShowFinishBattleMsg(_battle);
    }
}