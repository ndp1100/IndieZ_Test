using System;
using System.Collections.Generic;
using Game.Config;
using Game.Level.Gun;
using Game.Level.Unit;
using Sirenix.OdinInspector;
using StarterAssets;
using UnityEngine;

public sealed class PlayerView : UnitView
{
    [SerializeField] private StarterAssetsInputs _inputs;
    [SerializeField] private GunViewDict _gunViews;
    [SerializeField] private ThirdPersonController _thirdPersonController;


    [Serializable]
    public class GunViewDict : SerializableDictionary<WeaponId, GunView>
    {

    }

    public StarterAssetsInputs Inputs => _inputs;
    public ThirdPersonController ThirdPersonController => _thirdPersonController;

    public int MiningAnimID = UnityEngine.Animator.StringToHash("Mining");
    public int ShootAnimID = UnityEngine.Animator.StringToHash("Shoot");
    public int ReloadAnimID = UnityEngine.Animator.StringToHash("Reload");
    public int MotionDirectionAnimID = UnityEngine.Animator.StringToHash("MotionDirection");

    public GunView GetGunView(WeaponId weaponId)
    {
        foreach (KeyValuePair<WeaponId, GunView> keyValuePair in _gunViews)
        {

            keyValuePair.Value.gameObject.SetActive(keyValuePair.Key == weaponId);
        }

        return _gunViews[weaponId];
    }

    public void SetMiningAnimation(bool value)
    {
        Animator.SetBool(MiningAnimID, value);
    }

    public void SetShootTrigger()
    {
        Animator.SetTrigger(ShootAnimID);
    }

    public void SetReloadAnimation(bool value)
    {
        Animator.SetBool(ReloadAnimID, value);
    }

    public void SetMotionDirection(float value)
    {
        Animator.SetFloat(MotionDirectionAnimID, value);
    }
}