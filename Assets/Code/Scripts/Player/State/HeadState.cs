using System;

namespace PlayerSM
{
    public class HeadState : IPlayerState
    {
        private PlayerController _player;

        public static Action<bool> OnHeadState;

        public HeadState(PlayerController player)
        {
            _player = player;
        }

        public void Enter()
        {
            OnHeadState?.Invoke(true);
        }

        public void Exit()
        {
            OnHeadState?.Invoke(false);
        }

        public void HandleInput() { }

        public void Update() { }

        public void Reset() { }
    }
}
