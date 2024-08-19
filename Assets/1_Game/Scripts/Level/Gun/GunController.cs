using System;
using System.Collections.Generic;
using Core;
using Game.Config;
using Game.Core;
using Game.Level.Gun.GunStates;
using Game.Level.Modules;
using Game.Level.Player.PlayerState;
using Game.Level.Unit;
using Game.Managers;
using Injection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Level.Gun
{
    public class GunModel : Observable
    {
        public int ClipSize;
        public int CurrentClip;
        public bool IsCanShoot;
        public WeaponId WeaponId;
        public WeaponStat WeaponStat;

        public void Reload()
        {
            CurrentClip = ClipSize;
            SetChanged();
        }
    }


    public class GunController : IDisposable
    {
        private readonly PlayerController _playerController;
        private readonly StateManager<GunState> _stateManager;
        private readonly VFXPoolModule _vfxPoolModule;
        private readonly List<RaycastHit> _hitList = new();

        private readonly RaycastHit[] _hits = new RaycastHit[10];

        public PlayerController PlayerController { get { return _playerController; } }

        public GunController(GunView view, GunModel model, PlayerController playerController, Context context)
        {
            View = view;
            Model = model;
            _playerController = playerController;

            var subContext = new Context(context);
            subContext.InstallByType(this, typeof(GunController));
            var subInjector = new Injector(subContext);
            subContext.Install(subInjector);

            var hudManager = context.Get<HudManager>();
            GunHudMediator = hudManager.ShowAdditional<GunHudMediator>();
            GunHudMediator.View.Model = Model;
            GunHudMediator.View.FireButton.onClick.AddListener(OnFireBtnClick);

            _vfxPoolModule = context.Get<VFXPoolModule>();

            _stateManager = new StateManager<GunState>();
            subInjector.Inject(_stateManager);

            if (Model.CurrentClip <= 0)
                SwitchToState(new GunReloadState(Model.WeaponStat.ReloadTime));
            else
                SwitchToState(new GunReadyState());
        }

        public GunView View { get; }

        public GunModel Model { get; }

        public GunHudMediator GunHudMediator { get; }

        public StateManager<GunState> StateManager => _stateManager;

        public void Dispose()
        {
            _stateManager.Dispose();
            GunHudMediator.View.FireButton.onClick.RemoveListener(OnFireBtnClick);
        }

        private void OnFireBtnClick()
        {
            if (_playerController.IsDead())
                return;

            if (_playerController.CurrentState is not PlayerHoldingGunState)
                return;

            // if (_playerController.CurrentEnemyController == null)
            //     return;

            Shoot(_playerController.CurrentEnemyController);
        }

        private bool IsCanShoot()
        {
            if (Model.CurrentClip <= 0)
            {
                YOLogger.LogTemporaryChannel("Gun", $"Clip : {Model.CurrentClip}");
                return false;
            }

            if (_stateManager.Current is not GunReadyState)
            {
                YOLogger.LogTemporaryChannel("Gun", $"State : {_stateManager.Current}");
                return false;
            }

            return true;
        }

        public void Shoot(UnitController targetUnit)
        {
            if (IsCanShoot() == false)
                return;

            _playerController.View.SetShootTrigger();
            View.PlayMuzzle();
            Model.CurrentClip--;
            SoundManager.Instance?.PlaySoundFX(Model.WeaponId == WeaponId.Assault4
                ? SoundDataScriptableObject.SoundFXID.SOUNDFX_AssaultShot
                : SoundDataScriptableObject.SoundFXID.SOUNDFX_SniperShot);

            Vector3 direction;
            if(targetUnit != null)
                direction = targetUnit.View.AimTransform.position - View.BulletSpawn.position;
            else
                direction = View.BulletSpawn.transform.forward;

            var hitList = GetHitList(direction);
            foreach (var hit in hitList)
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    var unitView = hit.collider.GetComponent<UnitView>();
                    if (unitView != null && unitView.UnitController != null)
                        unitView.UnitController.TakeDamage(Model.WeaponStat.Damage);
                }

                _vfxPoolModule.CreateHitEffect(hit, 0.5f);
            }

            if (Model.CurrentClip > 0)
                SwitchToState(new GunRecoilState(Model.WeaponStat.FireRate));
            else
                SwitchToState(new GunReloadState(Model.WeaponStat.ReloadTime));

            Model.SetChanged();
        }

        private List<RaycastHit> GetHitList(Vector3 direction)
        {
            _hitList.Clear();
            //raycast to get unit(s) in front of gun
            var hitCount = Physics.RaycastNonAlloc(View.BulletSpawn.position, direction, _hits, Model.WeaponStat.Range);

#if UNITY_EDITOR
            Debug.DrawRay(View.transform.position, direction * Model.WeaponStat.Range, Color.red, 1f);
#endif
            var passThrough = 0;
            if (Model.WeaponStat.BulletStat.BulletType == BulletType.PassThrough)
                passThrough = (int)Model.WeaponStat.BulletStat.Param1;

            var countPassThrough = 0;
            foreach (var hit in _hits)
            {
                if (hit.collider == null)
                    continue;

                if (hit.collider.CompareTag("Enemy"))
                {
                    var unitView = hit.collider.GetComponent<UnitView>();
                    if (unitView != null && unitView.UnitController != null && unitView.UnitController.IsDead())
                        continue;
                }

                if (_hitList.Contains(hit) == false)
                    _hitList.Add(hit);

                countPassThrough++;
                YOLogger.LogTemporaryChannel("Gun", $"Pass through {countPassThrough} object {hit.transform.name}");

                if (countPassThrough >= passThrough) break;
            }

            return _hitList;
        }

        public void SwitchToState<T>(T instance) where T : GunState
        {
            _stateManager.SwitchToState(instance);
        }
    }
}