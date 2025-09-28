using System;
using System.IO;
using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei_Model.CombatSystem.Core;
using Shin_Megami_Tensei_Model.Domain.States;

namespace Shin_Megami_Tensei
{
    public class Game
    {
        private readonly View view;
        private readonly TeamFileCoordinator teamFileCoordinator;
        private readonly BattleStateFactory battleStateFactory;
        private readonly PlayerNameResolver playerNameResolver;
        private readonly GameManager gameManager;

        public Game(View view, string teamsPath)
        {
            this.view = view;
            teamFileCoordinator = new TeamFileCoordinator(view);
            gameManager = new GameManager();
            battleStateFactory = new BattleStateFactory(gameManager);
            playerNameResolver = new PlayerNameResolver(gameManager);

            teamFileCoordinator.InitializeTeamsPath(teamsPath);
            gameManager.LoadReferenceData();
        }

        public void Play()
        {
            if (!TryPrepareBattle(out var battleSetup))
            {
                ShowInvalidFileMessage();
                return;
            }

            RunBattle(battleSetup);
        }

        private bool TryPrepareBattle(out BattleSetup battleSetup)
        {
            battleSetup = default!;

            var teamsFile = teamFileCoordinator.GetTeamsFile();
            if (string.IsNullOrWhiteSpace(teamsFile))
            {
                return false;
            }

            var battleState = battleStateFactory.GetBattleState(teamsFile);
            if (battleState == null)
            {
                return false;
            }

            var (player1Name, player2Name) = playerNameResolver.GetPlayerNames(teamsFile);
            battleSetup = new BattleSetup(battleState, player1Name, player2Name);
            return true;
        }

        private void ShowInvalidFileMessage()
        {
            view.WriteLine("Archivo de equipos inválido");
        }

        private void RunBattle(BattleSetup battleSetup)
        {
            var battleEngine = CreateBattleEngine();
            try
            {
                battleEngine.StartBattle(battleSetup.BattleState, battleSetup.Player1Name, battleSetup.Player2Name);
            }
            finally
            {
                DumpScriptIfRequested();
            }
        }

        private BattleEngine CreateBattleEngine()
        {
            return new BattleEngine(view, gameManager.GetSkillData());
        }

        private void DumpScriptIfRequested()
        {
            var dumpPath = Environment.GetEnvironmentVariable("DUMP_SCRIPT_PATH");
            if (string.IsNullOrWhiteSpace(dumpPath))
            {
                return;
            }

            try
            {
                File.WriteAllLines(dumpPath, view.GetScript());
            }
            catch
            {
                // Ignorar errores de escritura opcionales de depuracion.
            }
        }

        private sealed record BattleSetup(BattleState BattleState, string Player1Name, string Player2Name);
    }
}
