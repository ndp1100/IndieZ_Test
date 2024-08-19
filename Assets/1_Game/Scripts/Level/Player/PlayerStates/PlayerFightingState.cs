using Game.Config;
using Game.Core;
using Game.Level.Unit;
using Injection;

namespace Game.Level.Player.PlayerState
{
    public class PlayerFightingState : PlayerState
    {
        [Inject] private GameConfig _gameConfig;
        [Inject] private Timer _timer;
        [Inject] private GameManager _gameManager;

        private EnemyController _currentEnemyController;
        private WeaponStat _currentWeaponStat;
        private WeaponId _currentWeaponId;


        public PlayerFightingState(WeaponId currentWeaponId)
        {
            _currentWeaponId = currentWeaponId;
        }


        public override void Initialize()
        {
            _currentWeaponStat = _gameConfig.GetWeaponStat(_currentWeaponId);

            _nextTickFindEnemy = 0;
            _timer.TICK += OnTick;
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

        public override void Dispose()
        {
            _timer.TICK -= OnTick;
        }
    }
}