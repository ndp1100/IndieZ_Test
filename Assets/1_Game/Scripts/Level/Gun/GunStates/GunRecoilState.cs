using System;
using Cysharp.Threading.Tasks;
using Game.Core;
using Injection;

namespace Game.Level.Gun.GunStates
{
    public class GunRecoilState : GunState
    {
        private float _fireRate;

        public GunRecoilState(float fireRate)
        {
            _fireRate = fireRate;

            if (_fireRate <= 0)
                throw new Exception("Fire rate must be greater than 0");
        }


        public override void Initialize()
        {
            _gun.Model.IsCanShoot = false;
            _gun.Model.SetChanged();

            UniTask.Create(async () =>
            {
                float recoiltime = 1f / _fireRate;
                await UniTask.Delay(TimeSpan.FromSeconds(recoiltime));
                
                _gun.SwitchToState(new GunReadyState());
            });
        }

        public override void Dispose()
        {

        }
    }
}