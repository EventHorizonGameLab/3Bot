using UnityEngine;
using UnityEngine.AI;

namespace Player.Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        NavMeshAgent agent;

        void Start()
        {
            // Ottiene il NavMeshAgent attaccato al player
            agent = GetComponent<NavMeshAgent>();

            if (agent == null)
            {
                Debug.LogError("NavMeshAgent non trovato! Assicurati che l'oggetto abbia un NavMeshAgent.");
            }
        }

        void Update()
        {
            Move();
        }

        private void Move()
        {
            // Controlla se viene premuto il tasto sinistro del mouse
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Lancia il Raycast e controlla se colpisce un punto valido
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // Imposta la destinazione dell'agent al punto di impatto
                    agent.SetDestination(hit.point);
                }
            }
        }
    }
}
