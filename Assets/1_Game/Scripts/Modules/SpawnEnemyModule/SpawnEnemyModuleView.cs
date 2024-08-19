using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Level.Modules
{
    public class SpawnEnemyModuleView : MonoBehaviour
    {
        [SerializeField] private List<Transform> _enemySpawnPoints;
        [SerializeField] private Transform _enemyParent;
        [SerializeField] private List<EnemySpawnWaveData> _enemySpawnWaveData;

        public List<Transform> EnemySpawnPoints => _enemySpawnPoints;
        public Transform EnemyParent => _enemyParent;
        public List<EnemySpawnWaveData> EnemySpawnWaveData => _enemySpawnWaveData;

        public Transform GetRandomSpawnPoint()
        {
            return _enemySpawnPoints[Random.Range(0, _enemySpawnPoints.Count)];
        }
    }
}