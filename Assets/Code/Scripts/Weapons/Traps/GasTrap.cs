using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Traps
{
    public class GasTrap : MonoBehaviour
    {
        [Title("Settings")]
        [SerializeField, Min(0), Tooltip("Damage per interval")] private float _damage = 10f;
        [SerializeField, Tooltip("Damage interval in seconds")] private float _interval = 1f;
        [SerializeField, Tooltip("Damage type")] private AttackType _attackType;

        [Title("Debug")]
        [SerializeField] private bool _debug = false;
        [SerializeField] private bool _drawGizmos = false;

        private float _timer = 0f;

        private void Start() { }

        private void OnTriggerStay(Collider other)
        {
            if (_debug) Debug.Log($"Gas trap triggered by {other.name}");

            ApplyGasDamage(other);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_debug) Debug.Log($"Gas trap entered by {other.name}");

            _timer = 0f;
        }

        private void ApplyGasDamage(Collider collider)
        {
            if (_timer > _interval)
            {
                _timer = 0f;
                if (collider.TryGetComponent(out ITakeDamage damageable))
                    damageable.TakeDamage(_damage, _attackType);
            }
            else
            {
                _timer += Time.deltaTime;
            }
        }

        private void OnDrawGizmos()
        {
            if (!_drawGizmos) return;

            Gizmos.color = Color.green;

            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                if (collider is SphereCollider sphereCollider)
                {
                    Gizmos.DrawWireSphere(collider.bounds.center, sphereCollider.radius);
                }
                else if (collider is BoxCollider boxCollider)
                {
                    Gizmos.DrawWireCube(collider.bounds.center, boxCollider.size);
                }
                else if (collider is CapsuleCollider capsuleCollider)
                {
                    Gizmos.DrawWireSphere(collider.bounds.center, capsuleCollider.radius);
                }
            }
        }
    }
}
