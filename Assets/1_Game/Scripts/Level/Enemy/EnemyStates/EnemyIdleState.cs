using Injection;

namespace Game.Level.Unit.UnitState
{
    public sealed class EnemyIdleState : EnemyState
    {
        [Inject] GameManager _gameManager;

        public override void Initialize()
        {
            Enemy.View.NavMeshAgent.enabled = false;
            Enemy.View.SetSpeedParameter(0f);

            _timer.TICK += OnTick;
        }

        private void OnTick()
        {
            if(Enemy.CurrentTarget != null)
            {
                Enemy.SwitchToState(new EnemyChasingState(Enemy.CurrentTarget));
            }
            else
            {
                if (_gameManager.Player != null && _gameManager.Player.IsDead() == false)
                {
                    Enemy.CurrentTarget = _gameManager.Player;
                    Enemy.SwitchToState(new EnemyChasingState(_gameManager.Player));
                }
            }
        }

        public override void Dispose()
        {
            _timer.TICK -= OnTick;
        }
    }
}