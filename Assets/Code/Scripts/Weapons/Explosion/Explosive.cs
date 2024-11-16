using UnityEngine;
using Sirenix.OdinInspector;
using Unity.VisualScripting;

public class Explosive : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, Tooltip("Prefab of the explosion VFX effect")] private GameObject _explosionPrefab;
    [SerializeField, Tooltip("Force of the explosion"), Min(0)] private float _explosionForce = 10f;
    [SerializeField, Tooltip("Radius of the explosion"), Range(0, 10)] private float _explosionRadius = 5f;
    [SerializeField, Tooltip("Force of the trigger"), Min(0)] private float _triggerForce = 0.5f;
    [SerializeField] private bool _haveDetectionArea = true;

    [ShowIf("_haveDetectionArea"), Title("Detection Settings")]
    [ShowIf("_haveDetectionArea"), SerializeField, Tooltip("Radius for detecting enemies"), PropertyRange(0, "_explosionRadius")] private float _detectionRadius = 6f;
    [ShowIf("_haveDetectionArea"), SerializeField, Tooltip("LayerMask for detecting enemies")] private LayerMask _enemyLayer;
    [ShowIf("_haveDetectionArea"), SerializeField, Tooltip("LayerMask for detecting obstacles")] private LayerMask _obstacleLayer;

    [Title("Debug")]
    [SerializeField] private bool _debug;
    [SerializeField] private bool _showGizmos;

    private Collider[] _detectedEnemies;

    private void Update()
    {
        DetectEnemies();
    }

    /// <summary>
    /// Continuously detects enemies in the detection area, considering obstacles.
    /// </summary>
    private void DetectEnemies()
    {
        if(!_haveDetectionArea) return;

        Explosion();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_debug) Debug.Log(collision.impulse.magnitude);

        if (collision.impulse.magnitude < _triggerForce) return;

        Explosion(false);
    }

    private void Explosion(bool detectionArea = true)
    {
        bool trigger = true;

        foreach (var obj in Physics.OverlapSphere(transform.position, _detectionRadius, _haveDetectionArea ? _enemyLayer : ~0))
        {
            if (IsVisible(obj))
            {
                if (obj.TryGetComponent(out Rigidbody rb))
                {
                    rb.AddExplosionForce(_explosionForce, transform.position, _explosionRadius);
                }

                if (obj.TryGetComponent(out IExplosionAffected explosionAffected))
                {
                    explosionAffected.OnExplosion(transform.position, _explosionForce);
                }

                trigger = false;
            }
        }

        if (detectionArea && trigger) return;

        if (_explosionPrefab != null) Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    /// <summary>
    /// Checks if the given target is visible from the explosion point, considering obstacles.
    /// </summary>
    /// <param name="target">The collider of the target object.</param>
    /// <returns>True if the target is visible, false otherwise.</returns>
    private bool IsVisible(Collider target)
    {
        Vector3 directionToTarget = target.bounds.center - transform.position;
        float distanceToTarget = directionToTarget.magnitude;

        return !Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacleLayer);
    }

    private void OnDrawGizmos()
    {
        if (!_showGizmos) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);

        if (!_haveDetectionArea) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}
