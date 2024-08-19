using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Level.Unit.UnitState
{
    public class EnemyChasingState : EnemyState
    {
        protected float reachDistance = 0.1f;
        private UnitController _target;

        public EnemyChasingState(UnitController target)
        {
            _target = target;
        }

        public override void Initialize()
        {
            if(_target == null || _target.IsDead())
            {
                Enemy.SwitchToState(new EnemyIdleState());
                return;
            }

            Enemy.View.NavMeshAgent.enabled = true;
            Enemy.View.NavMeshAgent.autoBraking = false;
            Enemy.View.NavMeshAgent.stoppingDistance = Enemy.Model.AttackRange / 2f;

            _timer.POST_TICK += OnPostTick;
        }

        public override void Dispose()
        {
            _timer.POST_TICK -= OnPostTick;
        }

        private void OnPostTick()
        {
            if (_target == null || _target.IsDead())
            {
                Enemy.SwitchToState(new EnemyIdleState());
                return;
            }

            if (NavMesh.SamplePosition(_target.View.transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                Enemy.View.NavMeshAgent.SetDestination(hit.position);
            }

            Enemy.View.SetSpeedParameter(Enemy.View.NavMeshAgent.velocity.magnitude);

            if (ReachedDestinationOrGaveUp())
            {
                Vector3 targetPosition = _target.View.transform.position;
                Vector3 myPos = _target.View.transform.position;
                myPos.y = targetPosition.y;
                float distance = Vector3.Distance(targetPosition, myPos);

                if (distance <= Enemy.Model.AttackRange)
                {
                    YOLogger.LogTemporaryChannel("Enemy", $"Attack");
                    Enemy.SwitchToState(new EnemyAttackingState(_target));
                }
            }
        }


        public bool ReachedDestinationOrGaveUp()
        {
            if (!Enemy.View.NavMeshAgent.pathPending)
            {
                if (Enemy.View.NavMeshAgent.remainingDistance <= Enemy.View.NavMeshAgent.stoppingDistance)
                {
                    if (!Enemy.View.NavMeshAgent.hasPath || Enemy.View.NavMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}