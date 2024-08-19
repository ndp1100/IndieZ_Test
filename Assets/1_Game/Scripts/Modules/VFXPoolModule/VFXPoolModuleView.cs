using System;
using Game.UI.Pool;
using UnityEngine;

namespace Game.Level.Modules
{
    [Serializable]
    public class HitEffectDict : SerializableDictionary<string, ComponentPoolFactory>
    {

    }


    public class VFXPoolModuleView : MonoBehaviour
    {
        [SerializeField] private HitEffectDict _hitEffectDict;

        public Transform GetHitEffect(string key)
        {
            if (_hitEffectDict.ContainsKey(key))
            {
                return _hitEffectDict[key].Get<Transform>();
            }
            else
            {
               return _hitEffectDict["Untagged"].Get<Transform>();
            }
        }

        public void ReleaseHitEffect(string key, Transform hitEffect)
        {
            if (_hitEffectDict.ContainsKey(key))
            {
                _hitEffectDict[key].Release(hitEffect);
            }
            else
            {
                _hitEffectDict["Untagged"].Release(hitEffect);
            }
        }
    }
}