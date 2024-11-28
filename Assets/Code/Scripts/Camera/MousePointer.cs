using Sirenix.OdinInspector;
using UnityEngine;

public class MousePointer : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, MinValue(0f)] private float _fixedHeight = 2f;
    [SerializeField, Required] private Transform _referenceObject;
    [SerializeField, MinValue(0f)] private float _maxDistance = 100f;

    [Title("Debug")]
    [SerializeField] private bool _onDrawGizmos = false;

    private Camera _mainCamera;
    private Vector3 _targetPosition;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        Plane plane = new (Vector3.up, _referenceObject.position + Vector3.up * _fixedHeight);
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float distance))
        {
            _targetPosition = ray.GetPoint(distance);

            Vector3 clampedPosition = _referenceObject.position + Vector3.ClampMagnitude(_targetPosition - _referenceObject.position, _maxDistance);
            transform.position = clampedPosition;
        }
    }

    private void OnDrawGizmos()
    {
        if (!_onDrawGizmos || _referenceObject == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_referenceObject.position, _maxDistance);
    }
}
