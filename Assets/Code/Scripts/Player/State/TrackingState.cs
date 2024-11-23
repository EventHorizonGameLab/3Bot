using UnityEngine;

namespace PlayerSM
{
    public class TrackingState : IPlayerState
    {
        private PlayerController _player;

        public TrackingState(PlayerController player)
        {
            _player = player;
        }

        public void Enter()
        {
            Debug.Log("Tracking State");
        }

        public void Exit() { }

        public void HandleInput()
        {
            // Logica di input
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }

        public void Update()
        {
            // Logica di tracking
        }
    }

}
