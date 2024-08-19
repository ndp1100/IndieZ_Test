using Cysharp.Threading.Tasks;
using Game.Config;
using Game.Managers;
using Game.UI.Hud;
using Injection;
using UnityEngine.SceneManagement;

namespace Game.States
{
    public class GameLoadLevelState : GameState
    {
        [Inject] protected GameConfig _config;
        [Inject] protected Context _context;
        [Inject] protected GameStateManager _gameStateManager;
        [Inject] protected HudManager _hudManager;

        private int _level;

        private SplashScreenHudMediator _splashScreenHudMediator;
        public bool IsLoaded { get; private set; }

        public override void Initialize()
        {
            IsLoaded = false;
            _splashScreenHudMediator = _hudManager.ShowAdditional<SplashScreenHudMediator>(new[] { this });

            _level = 1;

            if (_level < 1) _level = 1;
            else if (_level >= SceneManager.sceneCountInBuildSettings)
                _level = SceneManager.sceneCountInBuildSettings - 1;

            SceneManager.sceneLoaded += OnSceneLoaded;

            for (var i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
                if (SceneManager.GetSceneByBuildIndex(i).isLoaded)
                    SceneManager.UnloadSceneAsync(i);
            LoadScene();
        }

        public override void Dispose()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public virtual void OnSceneLoaded(Scene scene, LoadSceneMode arg)
        {
            LevelView level = null;
            var sceneObjects = scene.GetRootGameObjects();
            foreach (var sceneObject in sceneObjects)
            {
                level = sceneObject.GetComponent<LevelView>();

                if (null != level)
                    break;
            }

            _context.Install(level);

            YOLogger.Log("GameLoadLevelState OnSceneLoaded");
            _gameStateManager.SwitchToState<GamePlayState>();
            IsLoaded = true;
        }

        public virtual void LoadScene()
        {
            // SceneManager.LoadScene(_hotel, LoadSceneMode.Additive);

            //load scene async and get progress
            YOLogger.Log($"GameLoadLevelState LoadLevel StartLoadingScene: {_level}");
            var asyncOperation = SceneManager.LoadSceneAsync(_level, LoadSceneMode.Additive);
            UniTask.Create(async () =>
            {
                while (!asyncOperation.isDone)
                {
                    _splashScreenHudMediator.UpdateLoadSceneProgression(asyncOperation.progress);
                    await UniTask.Delay(100);
                }

                YOLogger.Log($"GameLoadLevelState LoadLevel Finished: {_level}");
            }).Forget();
        }
    }
}