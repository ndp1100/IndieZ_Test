using System.Collections.Generic;
using Game.Config;
using Game.Core;
using Game.Level.Gun;
using Game.Level.Gun.GunStates;
using Game.Level.Player.PlayerState;
using Injection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Level.Unit
{
    public sealed class PlayerController : UnitController
    {
        private readonly StateManager<PlayerState> _stateManager;
        private Timer _timer;
        private GameManager _gameManager;
        private GameConfig _gameConfig;

        private EnemyController _currentEnemyController;
        private WeaponId _currentWeaponId;
        private WeaponStat _currentWeaponStat;
        private GunController _gunController;
        private Dictionary<WeaponId, GunModel> _gunModelDict;

        public new PlayerView View { get; }
        public PlayerState CurrentState => _stateManager.Current;
        public EnemyController CurrentEnemyController => _currentEnemyController;

        public PlayerController(PlayerView view, UnitModel model, Context context) : base(view, model, context)
        {
            View = view;
            View.UnitController = this;
            View.ThirdPersonController.IsActive = true;

            SubContext.InstallByType(this, typeof(PlayerController));

            _gameManager = context.Get<GameManager>();
            _gameConfig = context.Get<GameConfig>();

            _gunModelDict = new Dictionary<WeaponId, GunModel>();

            _currentWeaponId = WeaponId.Assault4;
            _currentWeaponStat = _gameConfig.GetWeaponStat(_currentWeaponId);

            _nextTickFindEnemy = 0;
            _timer = context.Get<Timer>();
            _timer.TICK += OnTick;
            _timer.POST_TICK += OnPostTick;

            _stateManager = new StateManager<PlayerState>();
            SubInjector.Inject(_stateManager);
            SwitchToState(new PlayerHoldingGunState());

            var gunView = View.GetGunView(_currentWeaponId);
            if (_gunModelDict.TryGetValue(_currentWeaponId, out var gunModel) == false)
            {
                gunModel = new GunModel
                {
                    WeaponId = _currentWeaponId,
                    WeaponStat = _currentWeaponStat,
                    IsCanShoot = true,
                    CurrentClip = _currentWeaponStat.ClipSize,
                    ClipSize = _currentWeaponStat.ClipSize
                };

                _gunModelDict.Add(_currentWeaponId, gunModel);
            }

            _gunController = new GunController(gunView, gunModel, this, context);
        }

        private float _nextTickFindEnemy;
        private void OnTick()
        {
            if (_timer.Time < _nextTickFindEnemy)
                return;

            //find closest enemy in range, can increase interval if needed. 
            var closestEnemy = _gameManager.GetClosestEnemyInRange(_currentWeaponStat.Range);

            if (closestEnemy != null)
            {
                closestEnemy.View.SetOutline();

                if (_currentEnemyController != null && _currentEnemyController != closestEnemy)
                {
                    _currentEnemyController.View.RemoveOutline();
                }

                _currentEnemyController = closestEnemy;
            }
            else
            {
                if (_currentEnemyController != null)
                {
                    _currentEnemyController.View.RemoveOutline();
                    _currentEnemyController = null;
                }
            }

            _nextTickFindEnemy = _timer.Time + _gameConfig.IntervalFindEnemy;
        }

        private void OnPostTick()
        {
            //calculate motion direction 
            float speed = View.ThirdPersonController.AnimationSpeed;
            if (speed <= 1f)
                speed = 0;
            else
                speed = 1;

            if (_currentEnemyController == null || _currentEnemyController.IsDead())
            {
                View.SetMotionDirection(speed);
                return;
            }

            //rotate player to enemy
            var target = _currentEnemyController.View.transform.position;
            target.y = View.transform.position.y;
            View.transform.LookAt(target);

            float dotValue = Vector3.Dot(View.transform.forward, View.ThirdPersonController.TargetDirection);
            if (dotValue > 0.9f)
            {
                dotValue = 1;
            }
            else if (dotValue < -0.9f)
            {
                dotValue = -1;
            }
            else if (dotValue >= 0)
            {
                dotValue = 0.5f;
            }
            else
            {
                dotValue = -0.5f;
            }

            float motionDirection = dotValue * speed;
            View.SetMotionDirection(motionDirection);

        }

        public void Dispose()
        {
            _gunModelDict.Clear();

            _stateManager.Dispose();
            _timer.TICK -= OnTick;
            _timer.POST_TICK -= OnPostTick;
        }

        public override void OnDead()
        {
            View.ThirdPersonController.IsActive = false;
        }

        public void SwitchGun()
        {
            if (_stateManager.Current is not PlayerHoldingGunState)
            {
                YOLogger.LogTemporaryChannel("Player", "Can't switch gun while not in PlayerHoldingGunState state");
                return;
            }

            YOLogger.LogTemporaryChannel("Player", "Switch Gun");
            _currentWeaponId = _currentWeaponId == WeaponId.Assault4 ? WeaponId.Sniper2 : WeaponId.Assault4;
            _currentWeaponStat = _gameConfig.GetWeaponStat(_currentWeaponId);
            _gunController.Dispose();

            var gunView = View.GetGunView(_currentWeaponId);

            if (_gunModelDict.TryGetValue(_currentWeaponId, out var gunModel) == false)
            {
                gunModel = new GunModel
                {
                    WeaponId = _currentWeaponId,
                    WeaponStat = _currentWeaponStat,
                    IsCanShoot = true,
                    CurrentClip = _currentWeaponStat.ClipSize,
                    ClipSize = _currentWeaponStat.ClipSize
                };

                _gunModelDict.Add(_currentWeaponId, gunModel);
            }

            _gunController = new GunController(gunView, gunModel, this, SubContext);

        }

        public void MineBtnClick()
        {
            if (_stateManager.Current is not PlayerHoldingGunState)
            {
                YOLogger.LogTemporaryChannel("Player", "Can't mine while not in PlayerHoldingGunState state");
                return;
            }

            if (_gunController != null && _gunController.StateManager != null)
            {
                if (_gunController.StateManager.Current is not GunReadyState)
                {
                    YOLogger.LogTemporaryChannel("Player", "Can't mine while reloading or shooting");
                    return;
                }
            }

            YOLogger.LogTemporaryChannel("Player", "Mine");
            SwitchToState(new PlayerPlanningMineState());
        }

        public void SwitchToState<T>(T instance) where T : PlayerState
        {
            _stateManager.SwitchToState(instance);
            View.CurrentState = instance.GetType();
        }
    }
}