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
        private Queue<Vector3> _waypoints = new();
        private bool _isFollowingPath = false;

        //settings
        private float _offset = 0.5f;

        private bool isResetted = true;

        public MovementState2(PlayerController player)
        {
            _player = player;
            _agent = _player.GetComponent<NavMeshAgent>();
        }

        public void Enter()
        {
            Debug.Log("Movement State | Drawing Path");

            _lineRenderer = _player.GetComponent<LineRenderer>();

            GlobalUpdateManager.Instance.Register(SharedUpdate);
        }

        public void Exit() { }

        public void HandleInput()
        {
            if (Input.GetMouseButtonUp(0) && !_isFollowingPath)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    _waypoints.Enqueue(new(hit.point.x, _offset, hit.point.z));
                    UpdateLineRenderer();
                }
            }

            if (Input.GetMouseButtonUp(0) && _isFollowingPath && _agent.isStopped)
            {
                ClearLineRenderer();
                _waypoints.Clear();
                _isFollowingPath = false;
                _agent.isStopped = false;
                _agent.ResetPath();
            }

            if (Input.GetMouseButtonUp(0) && _isFollowingPath && !_agent.isStopped)
            {
                ClearLineRenderer();
                _waypoints.Clear();
                _agent.ResetPath();

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    _waypoints.Enqueue(new(hit.point.x, _offset, hit.point.z));
                    UpdateLineRenderer();
                }

                MoveToNextWaypoint();
            }

            if (Input.GetMouseButtonDown(1) && _isFollowingPath)
            {
                _agent.isStopped = !_agent.isStopped;
            }

            if (Input.GetMouseButtonDown(1) && _waypoints.Count > 0 && !_isFollowingPath)
            {
                _isFollowingPath = true;
                MoveToNextWaypoint();
            }
        }

        public void Update() { }

        public void SharedUpdate()
        {
            if (_isFollowingPath && !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (_waypoints.Count > 0)
                {
                    MoveToNextWaypoint();
                }
                else
                {
                    _isFollowingPath = false;
                    ClearLineRenderer();
                }
            }

            if (!isResetted)
            {
                Reset();
                isResetted = true;
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
            _lineRenderer.positionCount = _waypoints.Count + 1;

            _lineRenderer.SetPosition(0, new(_player.transform.position.x, _offset, _player.transform.position.z));

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

        public void Reset()
        {
            ClearLineRenderer();
            _isFollowingPath = false;
            _waypoints.Clear();
            _agent.destination = _player.transform.position;
            _agent.ResetPath();

            if (isResetted) isResetted = false;
        }
    }
}
