using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Config
{
    public enum EnemyId
    {
        Zombie1,
        ZombieBoss,
    }

    [Serializable]
    public struct EnemyStat
    {
        public float HP;
        public float WalkSpeed;
        public float RotationSpeed;
        public float AttackSpeed;
        public float AttackRange;
        public float AttackDamage;

        public GameObject EnemyPrefab;
    }

    [Serializable]
    public class EnemyConfigDict : SerializableDictionary<EnemyId, EnemyStat>
    {

    }

    public enum WeaponId : byte
    {
        None = 0,
        Assault4,
        Sniper2
    }

    public enum BulletType : byte
    {
        Normal = 0,
        PassThrough,
    }

    [Serializable]
    public struct BulletStat
    {
        public BulletType BulletType;

        [LabelText("@BulletDataDescription.GetParamDescription(this.BulletType, 1)")]
        [ShowIf("BulletType", BulletType.PassThrough)]
        public float Param1;
        [LabelText("@BulletDataDescription.GetParamDescription(this.BulletType, 2)")]
        [ShowIf("BulletType", BulletType.PassThrough)]
        public float Param2;
        [LabelText("@BulletDataDescription.GetParamDescription(this.BulletType, 3)")]
        [ShowIf("BulletType", BulletType.PassThrough)]
        public float Param3;
    }

    public class BulletDataDescription
    {
        public static string GetParamDescription(BulletType type, int index)
        {
            switch (type)
            {
                case BulletType.Normal:
                    return "Not Using";
                case BulletType.PassThrough:
                    return index switch
                    {
                        1 => "PassThroughCount",
                        2 => "Not Using",
                        3 => "Not Using",
                        _ => "Unknown"
                    };
                default:
                    return "Unknown";
            }
        }
    }


    [Serializable]
    public struct WeaponStat
    {
        public float Damage;
        public float FireRate;
        public float Range;
        public int ClipSize;
        public float ReloadTime;

        public BulletStat BulletStat;
    }

    [Serializable]
    public class WeaponConfigDict : SerializableDictionary<WeaponId, WeaponStat>
    {

    }


    [CreateAssetMenu(menuName = "config/gameconfig")]
    public sealed class GameConfig : ScriptableObject
    {
        public static GameConfig Load()
        {
            var result = Resources.Load<GameConfig>("GameConfig");
            return result;
        }


        [Header("Players")]
        public float PlayerWalkSpeed = 5f;
        public float PlayerRotationSpeed = 10f;
        public float PlayerMaxHP = 100f;
        public float IntervalFindEnemy = 0.15f;

        [Header("Enemies")]
        public EnemyConfigDict EnemyConfig;

        [Header("Weapons")]
        public WeaponConfigDict WeaponConfig;


        public GameConfig()
        {
        }

        public EnemyStat GetEnemyStat(EnemyId id)
        {
            return EnemyConfig[id];
        }

        public WeaponStat GetWeaponStat(WeaponId id)
        {
            return WeaponConfig[id];
        }
    }



}