using System;
using System.Collections.Generic;
using Game.Config;
using Game.Core;
using Game.Level;
using Game.Level.Modules;
using Game.Level.Unit;
using Game.Managers;
using Game.UI;
using Game.UI.Hud;
using Injection;
using UnityEngine;
using static SoundDataScriptableObject;

namespace Game.States
{
    public sealed class GamePlayState : GameState
    {
        private readonly List<Module> _levelModules;

        [Inject] private GameConfig _config;
        [Inject] private Context _context;


        private GameManager _gameManager;

        private GamePlayHudMediator _gamePlayHudMediator;

        [Inject] private GameView _gameView;
        [Inject] private HudManager _hudManager;
        [Inject] private Injector _injector;
        [Inject] private LevelView _levelView;
        [Inject] private Timer _timer;

        public GamePlayState()
        {
            _levelModules = new List<Module>();
        }

        public override void Initialize()
        {
            var saveGameFileNameOverride = _gameView.IsUseDebugSave ? _gameView.DebugSaveFilename : null;
            _gameManager = new GameManager(_config, saveGameFileNameOverride);

            _context.Install(_gameManager);
            _context.Install(_gameManager.Model);

            _timer.Application_Pause += OnApplication_Pause;
            _timer.Application_Focus += OnApplication_Focus;
            _timer.Application_Quit += OnApplication_Quit;

            InitLevelModules();

#if UNITY_EDITOR
            InitEntities_Test();
#endif
            _gamePlayHudMediator = _hudManager.ShowAdditional<GamePlayHudMediator>();

            // var gameConfig = context.Get<GameConfig>();
            var unitModel = new UnitModel();
            unitModel.UnitType = UnitType.Player;
            unitModel.MaxHP = _config.PlayerMaxHP;
            unitModel.CurrentHP = _config.PlayerMaxHP;
            unitModel.WalkSpeed = _config.PlayerWalkSpeed;
            unitModel.RotateSpeed = _config.PlayerRotationSpeed;
            unitModel.SetChanged();

            _gameManager.Player = new PlayerController(_levelView.PlayerView, unitModel, _context);
            // _gameManager.Player.SwitchToState(new PlayerFightingState(WeaponId.Sniper2));

            // var soundSignalData = new SoundSignalData(SoundActionType.PLAY, SoundType.SOUND_MUSIC,
            //     SoundFXID.NONE, SoundMusicID.BGM_MainPlay);
            // SoundManager.Instance?.DoActionSound(soundSignalData);

            _injector.Inject(_gameManager);
        }

        private void InitEntities_Test()
        {
            if (_levelView != null)
                if (_levelView.Enemies != null && _levelView.Enemies.Count > 0)
                    foreach (var enemy in _levelView.Enemies)
                    {
                        if(enemy == null) continue;
                        if (enemy.isActiveAndEnabled == false) continue;

                        var enemyStat = _config.GetEnemyStat(enemy.EnemyId);

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

                        var enemyController = new EnemyController(enemy, enemyModel, _context);
                        _gameManager.AddEnemy(enemyController);
                    }
        }

        public override void Dispose()
        {
            DisposeLevelModules();
            _timer.Application_Pause -= OnApplication_Pause;
            _timer.Application_Focus -= OnApplication_Focus;
            _timer.Application_Quit -= OnApplication_Quit;

            _gameManager.Player.Dispose();
            _gameManager.Dispose();

            _context.Uninstall(_gameManager);
        }

        private void InitLevelModules()
        {
            AddModule<VFXPoolModule, VFXPoolModuleView>(_levelView);
            AddModule<BattleObjectModule, BattleObjectModuleView>(_levelView);
            AddModule<SpawnEnemyModule, SpawnEnemyModuleView>(_levelView);
        }

        private void AddModule<T, T1>(Component component) where T : Module
        {
            var view = component.transform.GetComponent<T1>();
            var result = (T)Activator.CreateInstance(typeof(T), view);
            _levelModules.Add(result);
            _injector.Inject(result);

            _context.InstallByType(result, typeof(T));
            result.Initialize();
        }

        private void AddModule<T>() where T : Module
        {
            var result = (T)Activator.CreateInstance(typeof(T));
            _levelModules.Add(result);
            _injector.Inject(result);

            _context.InstallByType(result, typeof(T));
            result.Initialize();
        }

        private void DisposeLevelModules()
        {
            foreach (var levelModule in _levelModules) levelModule.Dispose();

            _levelModules.Clear();
        }

        private void OnApplication_Quit()
        {
            YOLogger.LogTemporaryChannel("SaveGame", "GamePlayState.OnApplication_Quit");
            _gameManager.Model.SaveGameData();
        }

        private void OnApplication_Focus(bool focusStatus)
        {
#if UNITY_EDITOR
            if (focusStatus) return;

            YOLogger.LogTemporaryChannel("SaveGame", "GamePlayState.OnApplication_Focus: " + focusStatus);
            _gameManager.Model.SaveGameData();
#endif
        }

        private void OnApplication_Pause(bool pauseStatus)
        {
            if (pauseStatus == false) return;

            YOLogger.LogTemporaryChannel("SaveGame", "GamePlayState.OnApplication_Pause: " + pauseStatus);
            _gameManager.Model.SaveGameData();
        }
    }
}