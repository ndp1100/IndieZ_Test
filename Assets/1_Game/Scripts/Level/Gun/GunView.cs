using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Level.Gun
{
    public class GunView : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _muzzleFlashParticleSystem;
        [SerializeField] private Transform _bulletSpawn;


        public Transform BulletSpawn => _bulletSpawn;

        public void PlayMuzzle()
        {
            _muzzleFlashParticleSystem.Play();

        }

        public void PlayReload()
        {
            //TODO: Play reload animation
        }
    }
}