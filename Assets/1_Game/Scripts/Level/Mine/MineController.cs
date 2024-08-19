using System;
using System.Collections;
using System.Collections.Generic;
using Game.Core;
using Game.Level.Modules;
using Injection;
using UnityEngine;

namespace Game.Level.Mine
{
    [Serializable]
    public class MineModel
    {
        public float Radius;
        public float Damage;
        public float TimeToExplode;
        public float Force;
    }


    public class MineController : IDisposable
    {
        [Inject] private BattleObjectModule _battleObjectModule;

        public readonly MineView View;
        public readonly MineModel Model;

        private Timer _timer;

        public MineController(MineView view, MineModel model, Vector3 position, Context context)
        {
            Model = model;
            View = view;
            View.transform.position = position;

            _timer = context.Get<Timer>();
            _timer.TICK += OnTick;
        }

        private float _timeToExplode;
        private void OnTick()
        {
            _timeToExplode += Time.deltaTime;
            if (_timeToExplode >= Model.TimeToExplode)
            {
                Explode();
                _battleObjectModule.ReleaseMine(this);
            }
        }

        private void Explode()
        {
            View.ExplodeVFX();

            var colliders = Physics.OverlapSphere(View.transform.position, Model.Radius);
            foreach (var collider in colliders)
            {
                var enemyView = collider.GetComponent<EnemyView>();
                if (enemyView != null && enemyView.UnitController != null)
                {
                    var rb = enemyView.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.isKinematic = false;
                        rb.AddExplosionForce(Model.Force, View.transform.position, Model.Radius);
                    }

                    enemyView.UnitController.TakeDamage(Model.Damage);
                }
            }
            
        }


        public void Dispose()
        {
            _timer.TICK -= OnTick;
        }
    }
}