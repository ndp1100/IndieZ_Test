using Game.Config;
using Game.Core;
using Game.Core.UI;
using Game.States;
using Injection;
using UnityEngine;

namespace Game.UI.Hud
{
    public sealed class SplashScreenHudMediator : Mediator<SplashScreenHudView>
    {
        private float _currentLoadSceneProgression;

        private readonly GameLoadLevelState _gameLoadLevelstate;
        private readonly float _loadSceneProgression = 0.6f;
        [Inject] private Timer _timer;

        private float _totalProgression = 1f;

        public SplashScreenHudMediator(GameLoadLevelState state)
        {
            _gameLoadLevelstate = state;
        }

        public void UpdateLoadSceneProgression(float progression)
        {
            _currentLoadSceneProgression = progression * _loadSceneProgression;
        }

        protected override void Show()
        {
            _view.AppVersionText.text = "v" + Application.version;

            _totalProgression = _loadSceneProgression;
            UpdateBar();

            _timer.TICK += OnTICK;
        }

        protected override void Hide()
        {
            _timer.TICK -= OnTICK;
        }

        private void OnTICK()
        {
            UpdateBar();

            if (_gameLoadLevelstate.IsLoaded) InternalHide();
        }

        private void UpdateBar()
        {
            float value = 0;
            value += _currentLoadSceneProgression;
            _view.FillBarImage.fillAmount = value / _totalProgression;
        }
    }
}