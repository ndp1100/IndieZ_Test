using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Game.Config;
using Game.Core;
using Game.Level.Unit;
using Game.Managers;
using Game.UI.Pool;
using Injection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Level.Modules
{
    //This should be in a separate config that can be fetching online.
    //Just temporary for now testing purposes
    [Serializable]
    public struct EnemySpawnData
    {
        public EnemyId EnemyId;
        public int Quantity;
    }

    [Serializable]
    public class EnemySpawnWaveData
    {
        public string WaveName;
        public float TimeToSpawn;
        public List<EnemySpawnData> EnemySpawnDataList;
    }

    [Serializable]
    public class SpawnModel : Observable
    {
        public float Time;
        public string WaveName;
        public float TimeToSpawnNextWave;
    }

    public class SpawnEnemyModule : Module<SpawnEnemyModuleView>
    {
        [Inject] private HudManager _hudManager;
        [Inject] private GameConfig _gameConfig;
        [Inject] private Context _context;
        [Inject] private GameManager _gameManager;
        [Inject] private Timer _timer;

        private readonly Dictionary<EnemyId, ComponentPoolFactory> _enemyPools;
        private SpawnModel _model;
        private EnemySpawnWaveData _nextEnemySpawnWaveData;

        public SpawnModel Model { get { return _model; } }

        public SpawnEnemyModule(SpawnEnemyModuleView view) : base(view)
        {
            _enemyPools = new Dictionary<EnemyId, ComponentPoolFactory>();
        }

        public override void Initialize()
        {
            CreateEnemyPool();

            _timeCounter = 0;
            
            _model = new SpawnModel();
            _model.Time = 0;

            _waveIndex = 0;
            _nextEnemySpawnWaveData = _view.EnemySpawnWaveData[_waveIndex];
            _model.WaveName = _nextEnemySpawnWaveData.WaveName;
            _model.TimeToSpawnNextWave = _nextEnemySpawnWaveData.TimeToSpawn;
            _model.SetChanged();

            _timer.ONE_SECOND_TICK += OnSecondTick;
        }

        private float _timeCounter;
        private int _waveIndex;
        private void OnSecondTick()
        {
            _timeCounter += 1;
            _model.Time = _timeCounter;


            if (_nextEnemySpawnWaveData != null)
            {
                if (_timeCounter >= _nextEnemySpawnWaveData.TimeToSpawn)
                {
                    CreateEnemyWave(_nextEnemySpawnWaveData);

                    _waveIndex++;
                    if (_waveIndex < _view.EnemySpawnWaveData.Count)
                    {
                        _nextEnemySpawnWaveData = _view.EnemySpawnWaveData[_waveIndex];
                        _model.WaveName = _nextEnemySpawnWaveData.WaveName;
                        _model.TimeToSpawnNextWave = _nextEnemySpawnWaveData.TimeToSpawn;
                    }
                    else
                    {
                        _nextEnemySpawnWaveData = null;
                        _model.WaveName = "All waves spawned";
                        _model.TimeToSpawnNextWave = -1;
                        YOLogger.LogTemporaryChannel("SpawnEnemy", "All waves spawned");
                    }
                }
            }
            

            _model.SetChanged();

        }

        private void CreateEnemyPool()
        {
            foreach (var enemySpawnWaveData in _view.EnemySpawnWaveData)
            {
                foreach (var enemySpawnData in enemySpawnWaveData.EnemySpawnDataList)
                {
                    if (_enemyPools.ContainsKey(enemySpawnData.EnemyId))
                    {
                        // Debug.LogError("Enemy pool already exists for enemy id: " + enemySpawnData.EnemyId);
                        continue;
                    }

                    var enemyStat = _gameConfig.GetEnemyStat(enemySpawnData.EnemyId);
                    if (enemyStat.EnemyPrefab != null)
                    {
                        GameObject pool = new GameObject(enemySpawnData.EnemyId.ToString());
                        pool.transform.SetParent(_view.EnemyParent);
                        var poolFactory = pool.AddComponent<ComponentPoolFactory>();
                        if (poolFactory != null)
                        {
                            int count = 5;
                            poolFactory.Setup(enemyStat.EnemyPrefab, count, pool.transform, pool.transform);
                        }

                        _enemyPools.Add(enemySpawnData.EnemyId, poolFactory);
                    }
                    else
                    {
                        Debug.LogError("Enemy prefab is null for enemy id: " + enemySpawnData.EnemyId);
                    }
                }
            }
        }

        private void CreateEnemyWave(EnemySpawnWaveData waveData)
        {
            YOLogger.LogTemporaryChannel("SpawnEnemy", $"Spawn Wave {waveData.WaveName}");

            foreach (var enemySpawnData in waveData.EnemySpawnDataList)
            {
                if (_enemyPools.TryGetValue(enemySpawnData.EnemyId, out var poolFactory))
                {
                    for (int i = 0; i < enemySpawnData.Quantity; i++)
                    {
                        var enemyView = poolFactory.Get<EnemyView>();
                        if (enemyView != null)
                        {
                            enemyView.gameObject.SetActive(true);

                            var enemyStat = _gameConfig.GetEnemyStat(enemySpawnData.EnemyId);
                            var enemyModel = new EnemyModel
                            {
                                UnitType = UnitType.Enemy,
                                MaxHP = enemyStat.HP,
                                CurrentHP = enemyStat.HP,
                                WalkSpeed = enemyStat.WalkSpeed,
                                RotateSpeed = enemyStat.RotationSpeed,
                                AttackDamage = enemyStat.AttackDamage,
                                AttackRange = enemyStat.AttackRange,
                                AttackSpeed = enemyStat.AttackSpeed
                            };

                            enemyModel.SetChanged();

                            var enemyController = new EnemyController(enemyView, enemyModel, _context);
                            _gameManager.AddEnemy(enemyController);

                            //get random spawn point
                            var spawnPoint = _view.GetRandomSpawnPoint();
                            enemyView.transform.position = spawnPoint.position;
                        }
                    }
                }
            }

        }

        public void ReleaseEnemy(EnemyView enemyView)
        {
            if(enemyView == null)
                return;

            if (_enemyPools.TryGetValue(enemyView.EnemyId, out var poolFactory))
            {
                poolFactory.Release<EnemyView>(enemyView);
            }
        }


        public override void Dispose()
        {
            _timer.ONE_SECOND_TICK -= OnSecondTick;
        }
    }
}