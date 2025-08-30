using System;
using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei_Model.CombatSystem.Core;
using Shin_Megami_Tensei_Model.Domain.States;

namespace Shin_Megami_Tensei
{
    public class Game
    {
        private readonly View view;
        private readonly TeamFileCoordinator teamFileManager;
        private readonly BattleStateFactory battleStateFactory;
        private readonly PlayerNameResolver playerNameExtractor;
        private readonly GameManager gameService;

        public Game(View view, string teamsPath)
        {
            this.view = view;
            this.teamFileManager = new TeamFileCoordinator(view);
            this.gameService = new GameManager();
            this.battleStateFactory = new BattleStateFactory(gameService);
            this.playerNameExtractor = new PlayerNameResolver(gameService);
            
            this.teamFileManager.InitializeTeamsPath(teamsPath);
            this.gameService.LoadReferenceData();
        }

        public void Play()
        {
            var file = teamFileManager.GetTeamsFile();
            if (IsInvalid(file))
            {
                ShowInvalidFileMessage();
                return;
            }
            
            var battleState = battleStateFactory.CreateBattleState(file);
            var playerNames = playerNameExtractor.GetPlayerNames(file);
            
            StartBattle(battleState, playerNames);
        }

        private bool IsInvalid<T>(T? item) where T : class
        {
            return item == null;
        }

        private bool IsInvalid(string? item)
        {
            return string.IsNullOrEmpty(item);
        }

        private void ShowInvalidFileMessage()
        {
            view.WriteLine("Archivo de equipos inválido");
        }
        
        private void StartBattle(BattleState battleState, (string player1Name, string player2Name) playerNames)
        {
            if (IsInvalid(battleState))
            {
                ShowInvalidFileMessage();
                return;
            }
            
            var battleEngine = CreateBattleEngine();
            battleEngine.StartBattle(battleState, playerNames.player1Name, playerNames.player2Name);
        }

        private BattleEngine CreateBattleEngine()
        {
            return new BattleEngine(view, gameService.GetSkillData());
        }
    }
}