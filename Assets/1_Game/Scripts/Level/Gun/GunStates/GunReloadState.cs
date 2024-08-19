using System;
using Cysharp.Threading.Tasks;
using Game.Core;
using Injection;
using UnityEngine;

namespace Game.Level.Gun.GunStates
{
    public class GunReloadState : GunState
    {
        [Inject] private Timer _timer;

        private float _reloadTime;
        private float _reloadTimeRemain;

        public GunReloadState(float reloadTime)
        {
            _reloadTime = reloadTime;
            _reloadTimeRemain = reloadTime;
        }

        public override void Initialize()
        {
            _gun.Model.IsCanShoot = false;
            _gun.Model.SetChanged();

            _gun.View.PlayReload();
            _gun.GunHudMediator.View.ShowReloadImage(true);

            _gun.PlayerController.View.SetReloadAnimation(true);
            SoundManager.Instance?.PlaySoundFX(SoundDataScriptableObject.SoundFXID.SOUNDFX_Reload);

            _timer.TICK += OnTick;
        }
        
        private void OnTick()
        {
            _reloadTimeRemain -= _timer.DeltaTime;
            float fillAmount = _reloadTimeRemain / _reloadTime;
            fillAmount = Mathf.Clamp01(fillAmount);
            _gun.GunHudMediator.View.SetReloadImage(fillAmount);

            if (_reloadTimeRemain <= 0)
            {
                _gun.PlayerController.View.SetReloadAnimation(false);
                _gun.Model.Reload();
                _gun.SwitchToState(new GunReadyState());
            }
        }

        public override void Dispose()
        {
            _timer.TICK -= OnTick;

            _gun.GunHudMediator.View.ShowReloadImage(false);
        }
    }
}