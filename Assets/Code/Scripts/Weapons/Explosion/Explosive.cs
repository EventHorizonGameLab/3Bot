using UnityEngine;
using Sirenix.OdinInspector;

public class Explosive : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, Tooltip("Prefab of the explosion VFX effect")] private GameObject _explosionPrefab;
    [SerializeField, Tooltip("Force of the explosion"), Min(0)] private float _explosionForce = 10f;
    [SerializeField, Tooltip("Radius of the explosion"), Range(0, 10)] private float _explosionRadius = 5f;
    [SerializeField, Tooltip("Force required to trigger explosion"), Min(0)] private float _triggerForce = 0.5f;
    [SerializeField, Tooltip("LayerMask for detecting obstacles")] private LayerMask _obstacleLayer;
    [SerializeField, Tooltip("Enable detection area")] private bool _haveDetectionArea = true;

    [ShowIf(nameof(_haveDetectionArea)), Title("Detection Settings")]
    [ShowIf(nameof(_haveDetectionArea)), SerializeField, Tooltip("Radius for detecting enemies"), PropertyRange(0, nameof(_explosionRadius))] private float _detectionRadius = 6f;
    [ShowIf(nameof(_haveDetectionArea)), SerializeField, Tooltip("LayerMask for detecting objects")] private LayerMask _detectionLayer;

    [Title("Debug")]
    [SerializeField, Tooltip("Enable debug logs")] private bool _debug;
    [SerializeField, Tooltip("Show debug gizmos")] private bool _showGizmos;

    private void Update()
    {
        if (_haveDetectionArea) DetectionArea();
    }

    /// <summary>
    /// Detects enemies in the detection area, considering visibility.
    /// </summary>
    private void DetectionArea()
    {
        if (_debug) Debug.Log($"Detection Radius: {_detectionRadius}");

        if (Physics.OverlapSphere(transform.position, _detectionRadius, _detectionLayer).Length > 0)
        {
            Explosion();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_debug) Debug.Log($"Collision Impulse: {collision.impulse.magnitude}");

        if (collision.impulse.magnitude >= _triggerForce) Explosion();
    }

    /// <summary>
    /// Triggers the explosion effect, applying force and damage to nearby objects.
    /// </summary>
    private void Explosion()
    {
        foreach (var obj in Physics.OverlapSphere(transform.position, _explosionRadius))
        {
            if (!IsVisible(obj)) continue;

            if (obj.TryGetComponent(out Rigidbody rb))
                rb.AddExplosionForce(_explosionForce, transform.position, _explosionRadius);

            if (obj.TryGetComponent(out IExplosionAffected explosionAffected))
                explosionAffected.OnExplosion(transform.position, _explosionForce);
        }

        if (_explosionPrefab != null)
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

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

        if (_debug) Debug.DrawRay(transform.position, directionToTarget, Color.red, 1f);

        return !Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacleLayer);
    }

    private void OnDrawGizmos()
    {
        if (!_showGizmos) return;

        // Draw explosion radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);

        // Draw detection area
        if (_haveDetectionArea)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);
        }
    }
}
