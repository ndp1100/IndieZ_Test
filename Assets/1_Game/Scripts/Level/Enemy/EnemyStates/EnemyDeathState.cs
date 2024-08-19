using System;
using Cysharp.Threading.Tasks;
using Game.Level.Modules;
using Injection;
using UnityEngine;

namespace Game.Level.Unit.UnitState
{
    public class EnemyDeathState : EnemyState
    {
        [Inject] SpawnEnemyModule _spawnEnemyModule;
        public override void Initialize()
        {
            Enemy.View.NavMeshAgent.enabled = false;

            UniTask.Delay(TimeSpan.FromSeconds(3f)).ContinueWith(() =>
            {
                Enemy.View.Rigidbody.velocity = Vector3.zero;
                Enemy.View.Rigidbody.angularVelocity = Vector3.zero;
                _spawnEnemyModule.ReleaseEnemy(Enemy.View);
            }).Forget();
        }

        public override void Dispose()
        {
            Enemy.Dispose();
        }
    }
}