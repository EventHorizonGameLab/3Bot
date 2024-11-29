using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;

namespace Game.Traps
{
    public abstract class BaseExplosive : BaseAudioHandler
    {
        [Title("Settings")]
        [SerializeField, Tooltip("Prefab of the explosion VFX effect")] protected string _explosionPrefab;
        [SerializeField, Tooltip("Force of the explosion"), MinValue(0)] protected float _explosionForce = 10f;
        [SerializeField, Tooltip("Damage of the explosion"), MinValue(0)] protected float _explosionDamage = 10f;
        [SerializeField, Tooltip("Radius of the explosion"), Range(0, 10)] protected float _explosionRadius = 5f;
        [SerializeField, Tooltip("LayerMask for detecting obstacles")] protected LayerMask _obstacleLayer;

        [Title("Debug")]
        [SerializeField, Tooltip("Enable debug logs"), PropertyOrder(2)] protected bool _debug = false;
        [SerializeField, Tooltip("Show debug gizmos"), PropertyOrder(2)] protected bool _showGizmos = false;

        /// <summary>
        /// Handles the explosion, applying force and triggering effects for nearby objects.
        /// </summary>
        protected virtual void Explosion()
        {
            foreach (var obj in Physics.OverlapSphere(transform.position, _explosionRadius))
            {
                if (!IsVisible(obj)) continue;

                if (_debug) Debug.Log($"Explosion hit: {obj.name}");

                if (obj.TryGetComponent(out ITakeDamage explosionAffected))
                    explosionAffected.TakeDamage(_explosionDamage, AttackType.Explosive);

                if (obj.TryGetComponent(out Rigidbody rb))
                {
                    rb.AddExplosionForce(_explosionForce, transform.position, _explosionRadius);
                }
            }

            if (_explosionPrefab != null && _explosionPrefab.Length > 0)
            {
                if (ObjectPooler.Instance == null) return;
                GameObject obj = ObjectPooler.Instance.Get(_explosionPrefab, 5f);
                obj.transform.position = transform.position;
            }

            Play(_audioClipName);

            //Destroy(gameObject); // to change
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Checks if the given target is visible from the explosion point, considering obstacles.
        /// </summary>
        /// <param name="target">The collider of the target object.</param>
        /// <returns>True if the target is visible, false otherwise.</returns>
        protected bool IsVisible(Collider target)
        {
            Vector3 directionToTarget = target.bounds.center - transform.position;
            float distanceToTarget = directionToTarget.magnitude;

            if (_debug) Debug.DrawRay(transform.position, directionToTarget, Color.red, 1f);

            return !Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacleLayer);
        }

        protected void OnDrawGizmos()
        {
            if (!_showGizmos) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _explosionRadius);
        }
    }
}
