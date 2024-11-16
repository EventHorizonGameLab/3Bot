using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Mine : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField] private GameObject _activateEffect;
    [SerializeField] private float _explosionForce;

    [Title("Debug")]
    [SerializeField] private bool _debug;
    [SerializeField] private LayerMask _obstacleLayer;

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void Explosion()
    {

    }

    private bool IsVisible(Collider target)
    {
        Vector3 directionToTarget = target.bounds.center - transform.position;
        float distanceToTarget = directionToTarget.magnitude;

        if (_debug) Debug.DrawRay(transform.position, directionToTarget, Color.red, 1f);

        return !Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacleLayer);
    }
}
