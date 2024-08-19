using System;
using System.Collections;
using System.Collections.Generic;
using EPOOutline;
using Game;
using Game.Config;
using Game.Level.Unit;
using UnityEngine;
using UnityEngine.AI;


public class EnemyView : UnitView
{
    [SerializeField] private EnemyId _enemyId;
    [SerializeField] protected NavMeshAgent _navMeshAgent;
    [SerializeField] private Rigidbody _rigidbody;

    public NavMeshAgent NavMeshAgent => _navMeshAgent;
    public EnemyId EnemyId => _enemyId;
    public Rigidbody Rigidbody => _rigidbody;

    public Action OnAttackEvent;

    public static int SpeedID = Animator.StringToHash("speed");
    public static int AttackID = Animator.StringToHash("attack");
    public static int MultiplierID = Animator.StringToHash("multiplier");

    internal void SetSpeedParameter(float speed)
    {
        Animator.SetFloat(SpeedID, speed);
    }

    internal void SetAttackTrigger(bool attack, float attackDuration)
    {
        //calculate speed based on attackDuration
        if (attack)
        {
            //tempt : Should get from config or something else, do not hardcode like this
            float animLength = 2.4f;
            float speedRatio = animLength / attackDuration;
            Animator.SetFloat(MultiplierID, speedRatio);
        }

        Animator.SetBool(AttackID, attack);
    }

    //called from animation event
    public void AnimEventAttack()
    {
        OnAttackEvent?.SafeInvoke();
    }

}
