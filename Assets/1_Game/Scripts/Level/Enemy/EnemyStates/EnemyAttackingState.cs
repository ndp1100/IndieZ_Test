using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Level.Unit.UnitState
{
    public class EnemyAttackingState : EnemyState
    {
        private UnitController _target;
        private float _attackDuration;
        private bool _isAttacked = false;

        public EnemyAttackingState(UnitController target)
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
            
            Enemy.View.OnAttackEvent += OnAttackEvent;
            _isAttacked = false;
            _attackDuration = 1f / Enemy.Model.AttackSpeed;
            Enemy.View.SetAttackTrigger(true, _attackDuration);
            Enemy.View.transform.LookAt(_target.View.transform.position);

            _timer.POST_TICK += OnPostTick;

        }

        private void OnAttackEvent()
        {
            if (_target != null && _target.IsDead() == false)
            {
                //check distance 
                float distance = Vector3.Distance(Enemy.View.transform.position, _target.View.transform.position);
                if (distance <= Enemy.Model.AttackRange)
                    _target.TakeDamage(Enemy.Model.AttackDamage);
                else
                {
                    _target.TakeDamage(-1f); //miss attack
                }
            }
        }

        private float _attackTimer = 0f;

        private void OnPostTick()
        {
            _attackTimer += Time.deltaTime;

            if (!(_attackTimer >= _attackDuration)) return;

            if (_target != null && _target.IsDead() == false)
            {
                float distance = Vector3.Distance(Enemy.View.transform.position, _target.View.transform.position);
                if (distance <= Enemy.Model.AttackRange)
                {
                   _attackTimer = 0f;
                   _isAttacked = false;
                   Enemy.View.SetAttackTrigger(true, _attackDuration);
                   Enemy.View.transform.LookAt(_target.View.transform.position);
                }
                else
                {
                    Enemy.SwitchToState(new EnemyChasingState(_target));
                }
            }
            else
                Enemy.SwitchToState(new EnemyIdleState());
        }


        public override void Dispose()
        {
            _timer.POST_TICK -= OnPostTick;
            Enemy.View.OnAttackEvent -= OnAttackEvent;
        }
    }
}