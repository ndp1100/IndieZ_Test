using Game.Config;
using Injection;

namespace Game.States
{
    public class GameInitializeState : GameState
    {
        [Inject] private Context _context;
        [Inject] private GameStateManager _gameStateManager;

        public override void Initialize()
        {
            var config = GameConfig.Load();
            _context.Install(config);
            _context.ApplyInstall();

            _gameStateManager.SwitchToState<GamePlayState>();
        }

        public override void Dispose()
        {
        }
    }
}