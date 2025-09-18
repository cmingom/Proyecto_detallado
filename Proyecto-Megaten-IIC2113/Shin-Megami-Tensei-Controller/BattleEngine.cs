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
            System.Console.WriteLine("DEBUG BattleEngine: StartBattle() iniciado");
            ExecuteBattleLoop(battleState, player1Name, player2Name);
            System.Console.WriteLine("DEBUG BattleEngine: StartBattle() completado");
        }

        //TO DO:  agregar catch de excepciones
        private void ExecuteBattleLoop(BattleState battleState, string player1Name, string player2Name)
        {
            System.Console.WriteLine("DEBUG BattleEngine: ExecuteBattleLoop() iniciado");
            while (ShouldContinueBattle(battleState))
            {
                System.Console.WriteLine("DEBUG BattleEngine: Procesando turno");
                if (ShouldExitBattle(battleState, player1Name, player2Name))
                {
                    System.Console.WriteLine("DEBUG BattleEngine: Saliendo de la batalla");
                    return;
                }
            }
            System.Console.WriteLine("DEBUG BattleEngine: ExecuteBattleLoop() completado");
        }

        private bool ShouldContinueBattle(BattleState battleState)
        {
            return !combatManager.IsBattleOver(battleState);
        }

        private bool ShouldExitBattle(BattleState battleState, string player1Name, string player2Name)
        {
            System.Console.WriteLine("DEBUG BattleEngine: ShouldExitBattle() - llamando turnManager.IsPlayerTurnComplete()");
            var result = turnManager.IsPlayerTurnComplete(battleState, player1Name, player2Name);
            System.Console.WriteLine($"DEBUG BattleEngine: ShouldExitBattle() - resultado: {result}");
            return result;
        }
    }
}
