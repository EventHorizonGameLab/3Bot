using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace PlayerSM
{
    public class MovementState2 : IPlayerState
    {
        private PlayerController _player;
        private NavMeshAgent _agent;
        private LineRenderer _lineRenderer;
        private Queue<Vector3> _waypoints = new Queue<Vector3>(); // Coda per memorizzare le coordinate
        private bool _isFollowingPath = false;

        public MovementState2(PlayerController player)
        {
            _player = player;
            _agent = _player.GetComponent<NavMeshAgent>();
        }

        public void Enter()
        {
            Debug.Log("Movement State | Drawing Path");

            _lineRenderer = _player.GetComponent<LineRenderer>();
            if (_lineRenderer == null)
            {
                _lineRenderer = _player.gameObject.AddComponent<LineRenderer>();
                _lineRenderer.startWidth = 0.1f;
                _lineRenderer.endWidth = 0.1f;
                _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                _lineRenderer.startColor = Color.red;
                _lineRenderer.endColor = Color.red;
                _lineRenderer.positionCount = 1; // Inizia con un punto alla posizione del player
                _lineRenderer.SetPosition(0, _player.transform.position); // Imposta il primo punto alla posizione attuale
            }
        }

        public void Exit()
        {
            if (_lineRenderer != null)
            {
                //Object.Destroy(_lineRenderer);
            }
        }

        public void HandleInput()
        {
            // Se il pulsante sinistro del mouse viene premuto, salva la coordinata
            if (Input.GetMouseButtonDown(0) && !_isFollowingPath)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    _waypoints.Enqueue(hit.point); // Aggiungi la posizione alla coda
                    UpdateLineRenderer();          // Aggiorna il LineRenderer con il nuovo punto
                }
            }

            // Se il pulsante destro del mouse viene premuto, inizia il movimento lungo il percorso
            if (Input.GetMouseButtonDown(1) && _waypoints.Count > 0 && !_isFollowingPath)
            {
                _isFollowingPath = true;
                MoveToNextWaypoint();
            }

            // Se il pulsante sinistro del mouse viene premuto mentre sta seguendo il percorso ed è fermo, resetta il percorso
            if (Input.GetMouseButtonDown(0) && _isFollowingPath && _agent.isStopped)
            {
                ClearLineRenderer();
                _waypoints.Clear();
                _isFollowingPath = false;
                _agent.isStopped = false;
                _agent.ResetPath();
            }

            // Se il pulsante sinistro del mouse viene premuto mentre sta seguendo il percorso, resetta il percorso
            if (Input.GetMouseButtonDown(0) && _isFollowingPath && !_agent.isStopped)
            {
                ClearLineRenderer();
                _waypoints.Clear();
                _agent.ResetPath();

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    _waypoints.Enqueue(hit.point); // Aggiungi la posizione alla coda
                    UpdateLineRenderer();          // Aggiorna il LineRenderer con il nuovo punto
                }

                MoveToNextWaypoint();
            }

            // Se il pulsante destro del mouse viene premuto mentre sta seguendo il percorso, ferma il movimento viceversa lo riattiva
            if (Input.GetMouseButtonUp(1) && _isFollowingPath)
            {
                _agent.isStopped = !_agent.isStopped;
            }
        }

        public void Update()
        {
            // Controlla se l'agent ha raggiunto l'attuale waypoint
            if (_isFollowingPath && !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (_waypoints.Count > 0)
                {
                    MoveToNextWaypoint();
                }
                else
                {
                    _isFollowingPath = false; // Fine del percorso
                    ClearLineRenderer();
                }
            }
        }

        private void MoveToNextWaypoint()
        {
            if (_waypoints.Count > 0)
            {
                Vector3 nextWaypoint = _waypoints.Dequeue();
                _agent.SetDestination(nextWaypoint);
            }
        }

        private void UpdateLineRenderer()
        {
            // Imposta i punti per il LineRenderer includendo il player e tutti i waypoint
            _lineRenderer.positionCount = _waypoints.Count + 1;

            // Imposta la posizione iniziale come quella del player
            _lineRenderer.SetPosition(0, _player.transform.position);

            int index = 1;
            foreach (var point in _waypoints)
            {
                _lineRenderer.SetPosition(index, point);
                index++;
            }
        }

        private void ClearLineRenderer()
        {
            _lineRenderer.positionCount = 0;
        }
    }
}
