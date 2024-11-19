using UnityEngine;
using UnityEngine.AI;

namespace Doors
{
    [RequireComponent(typeof(BoxCollider))]
    public class Door : MonoBehaviour
    {
        [SerializeField, Min(0)] private float _offset = 5f;

        private BoxCollider _collider;
        private NavMeshAgent _playerAgent;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (other.TryGetComponent<NavMeshAgent>(out _playerAgent))
                {
                    Vector3 directionToMove = (other.transform.position - transform.position).normalized;

                    Vector3 targetPosition = transform.position + directionToMove * -_offset;

                    _playerAgent.SetDestination(targetPosition);

                    Debug.Log($"Moving player to offset position: {targetPosition}");
                }
            }
        }

    }
}
