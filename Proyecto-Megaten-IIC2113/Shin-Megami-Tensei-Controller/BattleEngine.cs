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
            battleView = new BattleView(view);
            combatManager = new CombatManager(skillData, battleView);
            turnManager = new TurnManager(battleView, combatManager);
        }

        public void StartBattle(BattleState battleState, string player1Name, string player2Name)
        {
            RunBattleLoop(battleState, player1Name, player2Name);
        }

        private void RunBattleLoop(BattleState battleState, string player1Name, string player2Name)
        {
            while (IsBattleInProgress(battleState))
            {
                if (ProcessPlayerTurn(battleState, player1Name, player2Name))
                {
                    return;
                }

                if (battleState.IsBattleFinished)
                {
                    return;
                }
            }
        }

        private bool IsBattleInProgress(BattleState battleState)
        {
            return !combatManager.HasBattleEnded(battleState);
        }

        private bool ProcessPlayerTurn(BattleState battleState, string player1Name, string player2Name)
        {
            return turnManager.ProcessTurn(battleState, player1Name, player2Name);
        }
    }
}
