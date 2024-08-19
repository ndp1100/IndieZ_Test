using System;
using System.Collections.Generic;
using Game.Config;
using Game.Domain;
using Game.Level.Modules;
using Game.Level.Unit;
using Injection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Game
{
    public sealed class GameManager : IDisposable
    {
        [Inject] private SpawnEnemyModule _spawnModule;

        public readonly GameModel Model;
        public PlayerController Player;

        private GameConfig _config;
        private List<EnemyController> _enemies = new List<EnemyController>();


        public GameManager(GameConfig config, string saveGameOverride)
        {
            Model = GameModel.Load(config, saveGameOverride);
            _config = config;

            _enemies.Clear();
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void AddEnemy(EnemyController enemy)
        {
            if (_enemies.Contains(enemy) == false)
                _enemies.Add(enemy);
        }

        public void RemoveEnemy(EnemyController enemy)
        {
            if (_enemies.Contains(enemy))
                _enemies.Remove(enemy);
        }

        public EnemyController GetClosestEnemyInRange(float range)
        {
            EnemyController closestEnemy = null;
            float closestDistance = float.MaxValue;

            foreach (var enemy in _enemies)
            {
                if (enemy.IsDead())
                    continue;

                var distance = Vector3.Distance(Player.View.AimTransform.position, enemy.View.AimTransform.position);
                if (distance <= range && distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }

            return closestEnemy;
        }

        public void Dispose()
        {
        }
    }
}