using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Managers;
using Game.States;
using Injection;
using UnityEngine;


namespace Game
{
    public class GameStartBehaviour : MonoBehaviour
    {
        private Timer _timer;
        public Context Context { get; private set; }

        private void Start()
        {
            _timer = new Timer();

            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;

            var context = new Context();

            context.Install(
                new Injector(context),
                new GameStateManager(),
                new HudManager()
            );

            context.Install(GetComponents<Component>());
            context.Install(_timer);
            context.ApplyInstall();

            context.Get<GameStateManager>().SwitchToState(typeof(GameInitializeState));

            Context = context;
        }

        public void Reload()
        {
            Context.Get<GameStateManager>().Dispose();
            Context.Dispose();

            Start();
        }

        private void Update()
        {
            _timer.Update();
        }

        private void LateUpdate()
        {
            _timer.LateUpdate();
        }

        private void FixedUpdate()
        {
            _timer.FixedUpdate();
        }

        private void OnApplicationQuit()
        {
            if (_timer != null)
                _timer.OnApplicationQuit();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if(_timer != null)
                _timer.OnApplicationPause(pauseStatus);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if(_timer != null)
                _timer.OnApplicationFocus(hasFocus);
        }
    }
}