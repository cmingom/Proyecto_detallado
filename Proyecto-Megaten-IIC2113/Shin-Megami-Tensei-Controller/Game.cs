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
            this.teamFileCoordinator = new TeamFileCoordinator(view);
            this.gameManager = new GameManager();
            this.battleStateFactory = new BattleStateFactory(gameManager);
            this.playerNameResolver = new PlayerNameResolver(gameManager);
            
            this.teamFileCoordinator.InitializeTeamsPath(teamsPath);
            this.gameManager.LoadReferenceData();
        }

        public void Play()
        {
            var file = teamFileCoordinator.GetTeamsFile();
            if (IsNullOrEmpty(file))
            {
                ShowInvalidFileMessage();
                return;
            }
            
            var battleState = battleStateFactory.GetBattleState(file);
            var playerNames = playerNameResolver.GetPlayerNames(file);
            
            StartBattle(battleState, playerNames);
        }

        private bool IsNullOrEmpty(string? item)
        {
            return string.IsNullOrEmpty(item);
        }

        private void ShowInvalidFileMessage()
        {
            view.WriteLine("Archivo de equipos inválido");
        }
        
        private void StartBattle(BattleState battleState, (string player1Name, string player2Name) playerNames)
        {
            if (IsNull(battleState))
            {
                ShowInvalidFileMessage();
                return;
            }
            
            var battleEngine = CreateBattleEngine();
            battleEngine.StartBattle(battleState, playerNames.player1Name, playerNames.player2Name);
        }

        private bool IsNull<T>(T? item) where T : class
        {
            return item == null;
        }

        private BattleEngine CreateBattleEngine()
        {
            return new BattleEngine(view, gameManager.GetSkillData());
        }
    }
}