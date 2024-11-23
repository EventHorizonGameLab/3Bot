using UnityEngine;
using UnityEngine.AI;

namespace PlayerSM
{
    public class MovementState : IPlayerState
    {
        private PlayerController _player;
        private NavMeshAgent _agent;
        private LineRenderer _lineRenderer;

        public MovementState(PlayerController player)
        {
            _player = player;
            _agent = _player.GetComponent<NavMeshAgent>();
        }

        public void Enter()
        {
            Debug.Log("Movement State");

            _lineRenderer = _player.GetComponent<LineRenderer>();
            if (_lineRenderer == null)
            {
                _lineRenderer = _player.gameObject.AddComponent<LineRenderer>();
                _lineRenderer.startWidth = 0.1f;
                _lineRenderer.endWidth = 0.1f;
                _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                _lineRenderer.startColor = Color.red;
                _lineRenderer.endColor = Color.red;
            }
        }

        public void Exit()
        {
            // Disabilita o rimuovi il LineRenderer quando esci dallo stato
            if (_lineRenderer != null)
            {
                //Object.Destroy(_lineRenderer);
            }
        }

        public void HandleInput()
        {
            // Gestisci eventuali input durante lo stato di movimento (se necessario)
        }

        public void Reset() { }

        public void Update()
        {
            Move();
        }

        private void Move()
        {
            if (Input.GetMouseButtonDown(0)) // Quando clicchi con il tasto sinistro del mouse
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Lancia il Raycast e controlla se colpisce un punto valido
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // Imposta la destinazione dell'agent al punto di impatto
                    _agent.SetDestination(hit.point);

                    // Mostra una linea verticale alla posizione di impatto del Raycast
                    ShowLineAtHitPosition(hit.point);
                }
            }

            // Aggiungi un altro Raycast che rappresenti la direzione del target dell'Agent
            if (_agent.pathPending == false && _agent.hasPath)
            {
                // Ottieni la posizione del target dell'Agent
                Vector3 targetPosition = _agent.destination;

                // Crea un Raycast visibile che parta dal punto di origine dell'agent (la posizione del player)
                Ray rayFromAgent = new Ray(_agent.transform.position, targetPosition - _agent.transform.position);

                // Visualizza il Raycast nella scena
                //Debug.DrawRay(rayFromAgent.origin, rayFromAgent.direction * 10f, Color.red, 2f); // Ray rosso
            }
        }

        private void ShowLineAtHitPosition(Vector3 hitPoint)
        {
            // Crea una linea verticale alla posizione di impatto
            if (_lineRenderer != null)
            {
                // Imposta i punti di partenza e arrivo per la LineRenderer
                _lineRenderer.SetPosition(0, new Vector3(hitPoint.x, hitPoint.y - 1f, hitPoint.z));  // Punto inferiore
                _lineRenderer.SetPosition(1, new Vector3(hitPoint.x, hitPoint.y + 1f, hitPoint.z));  // Punto superiore
            }
        }
    }
}
