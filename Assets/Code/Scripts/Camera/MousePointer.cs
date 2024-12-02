using Sirenix.OdinInspector;
using UnityEngine;

public class MousePointer : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, MinValue(0f)] private float _fixedHeight = 2f;
    [SerializeField, Required] private Transform _referenceObject;
    [SerializeField, MinValue(0f)] private Vector2 _rectSize = new(10f, 10f);
    [SerializeField, MinValue(0f)] private float _fixedZ = 10f;

    [Title("Debug")]
    [SerializeField] private bool _onDrawGizmos = false;

    private Camera _mainCamera;
    private Vector3 _targetPosition;
    private Vector3 _position;
    private bool _isEnabled = true;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!_isEnabled) return;

        _position = _referenceObject.position;
        _position.z = _fixedZ;

        Plane plane = new(Vector3.up, _position + Vector3.up * _fixedHeight);
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float distance))
        {
            _targetPosition = ray.GetPoint(distance);

            Vector3 localPoint = _targetPosition - _position;
            localPoint.x = Mathf.Clamp(localPoint.x, -_rectSize.x / 2, _rectSize.x / 2);
            localPoint.z = Mathf.Clamp(localPoint.z, -_rectSize.y / 2, _rectSize.y / 2);
            _targetPosition = _position + localPoint;

            transform.position = _targetPosition;
        }
    }

    private void OnEnable()
    {
        PauseManager.IsPaused += IsPaused;
    }

    private void OnDisable()
    {
        PauseManager.IsPaused -= IsPaused;
    }

    private void IsPaused(bool isPaused)
    {
        _isEnabled = !isPaused;
    }

    private void OnDrawGizmos()
    {
        if (!_onDrawGizmos || _referenceObject == null) return;

        // Disegna il rettangolo
        Gizmos.color = Color.green;
        Vector3 center = _referenceObject.position;
        center.z = _fixedZ;
        Vector3 size = new(_rectSize.x, 0, _rectSize.y);
        Gizmos.DrawWireCube(center + Vector3.up * _fixedHeight, size);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(center, 0.3f);
    }
}
