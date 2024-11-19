using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Mine : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, Tooltip("The VFX of the explosion")] private GameObject _activateEffect;
    [SerializeField, Min(0), Tooltip("The force of the explosion")] private float _explosionForce;
    [SerializeField, Min(0), Tooltip("The radius of the explosion")] private float _explosionRadius;
    [SerializeField, Tooltip("The layer of the obstacles")] private LayerMask _obstacleLayer;

    [Title("Debug")]
    [SerializeField, Tooltip("Debug mode")] private bool _debug;
    [SerializeField] private bool _drawGizmos;

    private void OnTriggerEnter(Collider other)
    {
        if (_debug) Debug.Log($"Mine triggered by {other.name}");

        if (IsVisible(other))
        {
            Explosion();
        }
    }

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

        if (_activateEffect != null)
            Instantiate(_activateEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private bool IsVisible(Collider target)
    {
        Vector3 directionToTarget = target.bounds.center - transform.position;
        float distanceToTarget = directionToTarget.magnitude;

        if (_debug) Debug.DrawRay(transform.position, directionToTarget, Color.red, 1f);

        return !Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacleLayer);
    }

    private void OnDrawGizmos()
    {
        if (!_drawGizmos) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}
