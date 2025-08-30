using System.Collections.Generic;
using Shin_Megami_Tensei_View.ConsoleLib;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Core;
using Shin_Megami_Tensei_View;

namespace Shin_Megami_Tensei
{
    public class BattleEngine
    {
        private readonly BattleView battleView;
        private readonly CombatManager combatManager;
        private readonly TurnManager turnManager;

        public BattleEngine(View view, Dictionary<string, Skill> skillData)
        {
            this.battleView = new BattleView(view);
            this.combatManager = new CombatManager(skillData, this.battleView);
            this.turnManager = new TurnManager(this.battleView, this.combatManager);
        }

        public void StartBattle(BattleState battleState, string player1Name, string player2Name)
        {
            while (!combatManager.IsBattleOver(battleState))
            {
                if (turnManager.IsPlayerTurnComplete(battleState, player1Name, player2Name))
                {
                    return;
                }
            }
        }
    }
}
