using Game.Core;
using Game.Level.Unit;
using Injection;

namespace Game.Level.Player.PlayerState
{
    public abstract class PlayerState : State
    {
        [Inject] protected PlayerController _player;
    }
}