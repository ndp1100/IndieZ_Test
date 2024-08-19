namespace Game.Level.Player.PlayerState
{
    public sealed class PlayerHoldingGunState : PlayerState
    {
        public override void Initialize()
        {
            _player.View.ThirdPersonController.SprintSpeed = _player.UnitModel.WalkSpeed;

        }


        public override void Dispose()
        {
        }
    }
}