using Game.Config;
using Game.Core;
using Game.Core.UI;
using Game.Level.Modules;
using Game.Managers;
using Injection;
using SystemInfo = UnityEngine.Device.SystemInfo;

namespace Game.UI.Hud
{
    public sealed class GamePlayHudMediator : Mediator<GamePlayHudView>
    {
        [Inject] private GameConfig _gameConfig;
        [Inject] private GameManager _gameManager;
        [Inject] private HudManager _hudManager;
        [Inject] private LevelView _levelView;
        [Inject] private Timer _timer;
        [Inject] private SpawnEnemyModule _spawnEnemyModule;

        protected override void Show()
        {
            var deviceID = SystemInfo.deviceUniqueIdentifier;
            YOLogger.Log("Device ID: " + deviceID);

            // _view.RestartButton.gameObject.SetActive(GameConstants.IsDeveloperDevice());

            _view.Model = _spawnEnemyModule.Model;
            _view.RestartButton.onClick.AddListener(OnRestartButtonClick);

            _view.SwitchGunBtn.onClick.AddListener(OnSwitchGunBtnClick);
            _view.MineBtn.onClick.AddListener(OnMineBtnClick);
        }

        private void OnMineBtnClick()
        {
            _gameManager.Player.MineBtnClick();
        }

        private void OnSwitchGunBtnClick()
        {
            _gameManager.Player.SwitchGun();
        }


        protected override void Hide()
        {
            _view.RestartButton.onClick.RemoveListener(OnRestartButtonClick);
            _view.SwitchGunBtn.onClick.RemoveListener(OnSwitchGunBtnClick);
            _view.MineBtn.onClick.RemoveListener(OnMineBtnClick);
        }


        private void OnRestartButtonClick()
        {
            _gameManager.Restart();
        }
    }
}