using Game.Core;
using Injection;

namespace Game.Level.Gun.GunStates
{
    public abstract class GunState : State
    {
        [Inject] protected GunController _gun;
    }
}