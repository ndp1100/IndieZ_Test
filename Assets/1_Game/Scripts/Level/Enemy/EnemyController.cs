using System;
using Game.Config;
using Game.Core;
using Game.Level.Modules;
using Game.Level.Unit.UnitState;
using Injection;
using UnityEngine;

namespace Game.Level.Unit
{
    public class EnemyModel : UnitModel
    {
        public float AttackSpeed;
        public float AttackRange;
        public float AttackDamage;
    }

    public class EnemyController : UnitController
    {
        private GameManager _gameManager;
        private SpawnEnemyModule _spawnEnemyModule;

        private EnemyModel _model;
        private UnitController _currentTarget;

        public EnemyModel Model { get { return _model; } }
        public UnitController CurrentTarget
        {
            get { return _currentTarget; }
            set { _currentTarget = value; }

        }

        public EnemyController(EnemyView view, EnemyModel enemyModel, Context context) : base(view, enemyModel, context)
        {
            View = view;
            view.UnitController = this;
            SubContext.InstallByType(this, typeof(EnemyController));

            _gameManager = context.Get<GameManager>();
            _spawnEnemyModule = context.Get<SpawnEnemyModule>();

            _model = enemyModel;
            _currentTarget = null;

            if (View.NavMeshAgent != null)
                View.NavMeshAgent.speed = UnitModel.WalkSpeed;

            if (View.Rigidbody != null)
            {
                View.Rigidbody.isKinematic = true;
            }

            StateManager = new StateManager<EnemyState>();
            SubInjector.Inject(StateManager);

            SwitchToState(new EnemyChasingState(_gameManager.Player));

        }

        public new EnemyView View { get; }

        public StateManager<EnemyState> StateManager { get; }

        public void SwitchToState<T>(T instance) where T : EnemyState
        {
            StateManager.SwitchToState(instance);

            View.CurrentState = instance.GetType();
        }

        public override void OnDead()
        {
            _gameManager.RemoveEnemy(this);
            SwitchToState(new EnemyDeathState());
            base.OnDead();
        }

        public virtual void Dispose()
        {
            StateManager.Dispose();
        }

        
    }
}