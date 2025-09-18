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
            System.Console.WriteLine("DEBUG Game: Play() iniciado");
            var file = teamFileCoordinator.GetTeamsFile();
            System.Console.WriteLine($"DEBUG Game: Archivo obtenido: {file}");
            if (IsNullOrEmpty(file))
            {
                ShowInvalidFileMessage();
                return;
            }
            
            System.Console.WriteLine("DEBUG Game: Creando BattleState");
            var battleState = battleStateFactory.GetBattleState(file);
            System.Console.WriteLine("DEBUG Game: Obteniendo nombres de jugadores");
            var playerNames = playerNameResolver.GetPlayerNames(file);
            System.Console.WriteLine($"DEBUG Game: Nombres: {playerNames.player1Name} vs {playerNames.player2Name}");
            
            System.Console.WriteLine("DEBUG Game: Iniciando batalla");
            StartBattle(battleState, playerNames);
            System.Console.WriteLine("DEBUG Game: Play() completado");
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
            System.Console.WriteLine("DEBUG Game: StartBattle() iniciado");
            if (IsNull(battleState))
            {
                ShowInvalidFileMessage();
                return;
            }
            
            System.Console.WriteLine("DEBUG Game: Creando BattleEngine");
            var battleEngine = CreateBattleEngine();
            System.Console.WriteLine("DEBUG Game: Llamando battleEngine.StartBattle()");
            battleEngine.StartBattle(battleState, playerNames.player1Name, playerNames.player2Name);
            System.Console.WriteLine("DEBUG Game: StartBattle() completado");
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