using Game.Core;
using Injection;

namespace Game.Level.Unit.UnitState
{
    public abstract class EnemyState : State
    {
        [Inject] protected EnemyController Enemy;
        [Inject] protected Timer _timer;
    }
}