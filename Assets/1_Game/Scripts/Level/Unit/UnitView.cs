using System;
using Cysharp.Threading.Tasks;
using EPOOutline;
using Injection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Level.Unit
{
    public class UnitView : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _aimTransform;
        [SerializeField] private Transform _uiPivoTransform;
        [SerializeField] private Outlinable _outlinable;
        [SerializeField] private Collider _collider;

        public Animator Animator => _animator;
        public Transform AimTransform => _aimTransform;
        public Transform UIPivotTransform => _uiPivoTransform;

        private UnitController _unitController;
        public UnitController UnitController
        {
            get => _unitController;
            set => _unitController = value;
        }


        [Header("Debug Info")]
        [ReadOnly, ShowInInspector] private string _currentState;

        public Type CurrentState
        {
            set => _currentState = value?.Name;
        }

        private int deathParameterID = Animator.StringToHash("death");

        protected virtual void Awake()
        {
            if (_outlinable == null)
                _outlinable = GetComponent<Outlinable>();

            if (_animator == null)
                _animator = GetComponent<Animator>();

            if (_collider == null)
                _collider = GetComponent<Collider>();

        }

        private void OnEnable()
        {
           _collider.enabled = true;
        }

        public void SetOutline(float time = 0)
        {
            if (time <= 0)
            {
                if (_outlinable)
                {
                    _outlinable.OutlineParameters.Enabled = true;
                }
            }
            else
            {
                UniTask.Create(async () =>
                {
                    if (_outlinable)
                    {
                        _outlinable.OutlineParameters.Enabled = true;
                    }

                    await UniTask.Delay(TimeSpan.FromSeconds(time));

                    if (_outlinable)
                    {
                        _outlinable.OutlineParameters.Enabled = false;
                    }

                });
            }
        }

        public void RemoveOutline()
        {
            if (_outlinable)
            {
                _outlinable.OutlineParameters.Enabled = false;
            }
        }

        public void SetDeath()
        {
            SetAnimatorTrigger(deathParameterID);
            // _collider.enabled = false;
        }

        private void SetAnimatorTrigger(int parameterID)
        {
            _animator.SetTrigger(parameterID);
        }

        #if UNITY_EDITOR

        [Button("Kill")]
        private void Kill()
        {
            if (_unitController != null)
            {
                _unitController.TakeDamage(_unitController.UnitModel.MaxHP);
            }
        }
        #endif

    }
}