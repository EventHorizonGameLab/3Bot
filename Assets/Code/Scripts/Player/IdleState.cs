using UnityEngine;

namespace PlayerSM
{
    public class IdleState : IPlayerState
    {
        private PlayerController _player;

        public IdleState(PlayerController player)
        {
            _player = player;
        }

        public void Enter()
        {
            Debug.Log("Entering Idle State");
            //_player.SetAnimation("Idle");  // Imposta l'animazione "Idle"
        }

        public void Exit()
        {
            Debug.Log("Exiting Idle State");
        }

        public void HandleInput()
        {
            
        }

        public void Update()
        {
            // Logica quando il player è fermo (ad esempio nessun movimento)
        }
    }
}
