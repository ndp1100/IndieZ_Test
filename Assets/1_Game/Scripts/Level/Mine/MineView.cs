using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Level.Mine
{
    public class MineView : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _mineExploseVFX;
        [SerializeField] private ParticleSystem _mineTrigger;
        [SerializeField] private ParticleSystem _mineVolume;

        private void OnEnable()
        {
            _mineTrigger.Play();
            _mineVolume.Play();
        }

        public void ExplodeVFX(float time = 1f)
        {
            _mineExploseVFX.Play();

            _mineTrigger.Stop();
            _mineVolume.Stop();

            SoundManager.Instance?.PlaySoundFX(SoundDataScriptableObject.SoundFXID.SOUNDFX_Explosion);
        }
    }
}