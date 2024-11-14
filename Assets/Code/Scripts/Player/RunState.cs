using UnityEngine;
using UnityEngine.AI;

namespace PlayerSM
{
    public class RunState : IPlayerState
    {
        private PlayerController _player;
        private NavMeshAgent _agent;

        public RunState(PlayerController player)
        {
            _player = player;
            _agent = _player.GetComponent<NavMeshAgent>();
        }

        public void Enter()
        {
            Debug.Log("Entering Run State");
            //_player.SetAnimation("Run"); // Imposta l'animazione "Run"
        }

        public void Exit()
        {
            Debug.Log("Exiting Run State");
            _agent.ResetPath();  // Resetta il percorso quando esce dallo stato
        }

        public void HandleInput()
        {
            
        }

        public void Update()
        {
            Move();
        }

        private void Move()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Lancia il Raycast e controlla se colpisce un punto valido
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // Imposta la destinazione dell'agent al punto di impatto
                    _agent.SetDestination(hit.point);
                }
            }
        }
    }
}
