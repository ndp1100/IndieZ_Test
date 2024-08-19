using System;
using UnityEngine;

namespace Core
{
    public abstract class BehaviourComponent : MonoBehaviour
    {
        [NonSerialized]
        private bool _released = false;

        protected virtual void OnEnable()
        {
            
        }

        protected virtual void OnDisable()
        {
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnReleaseResources()
        {
        }

        private void Start()
        {
            if (Application.isEditor && !Application.isPlaying)
                return;

            OnStart();
        }

        protected virtual void OnDestroy()
        {
            if (!_released)
            {
                OnReleaseResources();
            }

            _released = true;
        }
    }
}