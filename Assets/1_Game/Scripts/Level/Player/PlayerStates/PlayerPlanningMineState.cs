using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Level.Mine;
using Game.Level.Modules;
using Injection;
using UnityEngine;

namespace Game.Level.Player.PlayerState
{
    public class PlayerPlanningMineState : PlayerState
    {
        [Inject] private BattleObjectModule _battleObjectModule;

        public override void Initialize()
        {
            _player.View.ThirdPersonController.IsActive = false;
            _player.View.SetMiningAnimation(true);

            UniTask.Delay(TimeSpan.FromSeconds(2)).ContinueWith(() =>
            {
                //create mine
                MineModel mineModel = new MineModel
                {
                    Radius = 5,
                    Damage = 100,
                    TimeToExplode = 3,
                    Force = 150
                };
                _battleObjectModule.CreateMine(_player.View.transform.position, mineModel);


                _player.View.SetMiningAnimation(false);
                _player.View.ThirdPersonController.IsActive = true;

                _player.SwitchToState(new PlayerHoldingGunState());
            }).Forget();

        }

        public override void Dispose()
        {
            
        }
    }
}