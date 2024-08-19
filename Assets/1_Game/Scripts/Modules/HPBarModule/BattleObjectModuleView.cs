using System;
using Game.UI.Pool;
using UnityEngine;

namespace Game.Level.Modules
{
    public class BattleObjectModuleView : MonoBehaviour, IDisposable
    {
        [SerializeField] private ComponentPoolFactory _dmgTextPool;
        [SerializeField] private ComponentPoolFactory _hpBarPool;
        [SerializeField] private ComponentPoolFactory _minePool;

        public ComponentPoolFactory HPBarPool => _hpBarPool;
        public ComponentPoolFactory DmgTextPool => _dmgTextPool;
        public ComponentPoolFactory MinePool => _minePool;

        public void Dispose()
        {
            _dmgTextPool.Dispose();
            _hpBarPool.Dispose();
            _minePool.Dispose();
        }
    }
}