using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TripwireMineLaser : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField] private LayerMask _collisionLayer;
    [SerializeField] private float _maxDistance = 10f;
    [SerializeField] private Vector3 _offset;

    [Title("Debug")]
    [SerializeField] private bool _drawGizmos = false;

    private LineRenderer _lineRenderer;

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        Laser();
    }

    private void Laser()
    {
        if (_lineRenderer == null) return;

        Vector3 origin = transform.position + _offset;
        Vector3 direction = -transform.right;

        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, origin);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, _maxDistance, _collisionLayer))
            _lineRenderer.SetPosition(1, hit.point);
        else
            _lineRenderer.SetPosition(1, origin + direction * _maxDistance);
    }

    private void OnDrawGizmos()
    {
        if (!_drawGizmos) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + _offset, _offset + transform.position + -transform.right * _maxDistance);
    }
}
