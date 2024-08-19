namespace Game.Level.Gun.GunStates
{
    public class GunReadyState : GunState
    {
        public override void Initialize()
        {
            _gun.Model.IsCanShoot = true;
            _gun.Model.SetChanged();
        }

        public override void Dispose()
        {
        }
    }
}