using PlayerSM;
using Sirenix.OdinInspector;
using UnityEngine;

public class KineticGun : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, MinValue(0f), Tooltip("Speed at which the selected object follows the mouse")] private float _moveSpeed = 10f;
    [SerializeField, MinValue(0f), Tooltip("Height at which to position the selected object")] private float _fixedHeight = 2f;
    [SerializeField, Tooltip("LayerMask to identify interactable objects")] private LayerMask _interactableLayer;
    [SerializeField, Tooltip("LayerMask to identify obstacle objects")] private LayerMask _obstacleLayer;
    [SerializeField, MinValue(0f), Tooltip("Maximum distance for the raycast")] private float _maxDistance = 10f;
    [SerializeField, MinValue(0f), Tooltip("Multiplier for momentum on release")] private float _momentumMultiplier = 1.5f;

    [Title("Debug")]
    [SerializeField] private bool _debug = false;
    [SerializeField, Tooltip("Show debug gizmos for the raycast")] private bool _onDrawGizmos = true;

    [ShowInInspector, ReadOnly] private bool _isEnabled = false;
    [ShowInInspector, ReadOnly] private bool _isHolding = false;
    [ShowInInspector, ReadOnly] private Rigidbody _heldObject = null;

    private Vector3 _targetPosition;
    private Vector3 _releaseVelocity;
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!_isEnabled) return;

        if (Input.GetMouseButtonDown(0)) OnMousePressed();
        if (Input.GetMouseButton(0)) OnMouseHeld();
        if (Input.GetMouseButtonUp(0)) OnMouseReleased();

        if (_isHolding)
        {
            if (!HasLineOfSightToPlayer())
            {
                if (_debug) Debug.Log("Line of sight with player lost. Checking LoS to mouse.");

                if (!HasLineOfSightToMouse())
                {
                    if (_debug) Debug.Log("Line of sight with both player and mouse lost. Forced release.");
                    ReleaseObject();
                }
            }
        }
    }

    private void OnEnable()
    {
        HeadState.OnHeadState += IsEnable;
    }

    private void OnDisable()
    {
        HeadState.OnHeadState -= IsEnable;
    }

    private void IsEnable(bool state)
    {
        _isEnabled = state;
    }

    private void OnMousePressed()
    {
        if (_isHolding) return;

        if (_debug) Debug.Log("Attempting to pick up an object.");

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (_onDrawGizmos) Debug.DrawRay(ray.origin, ray.direction * _maxDistance, Color.green, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance, _interactableLayer))
        {
            if (_debug) Debug.Log("Object hit: " + hit.collider.name);

            if (hit.collider.TryGetComponent(out Rigidbody rb))
            {
                // Check LoS between player and the object
                if (!HasLineOfSightToGameObject(hit.collider.gameObject))
                {
                    if (_debug) Debug.Log("Cannot pick up object. Line of sight blocked.");
                    return;
                }

                // Object can be picked up
                _heldObject = rb;
                _heldObject.useGravity = false;
                _heldObject.angularVelocity = Vector3.zero;
                _heldObject.freezeRotation = true;

                _isHolding = true;
            }
        }
    }

    private void OnMouseHeld()
    {
        if (!_isHolding || _heldObject == null) return;

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit planeHit, Mathf.Infinity, _obstacleLayer))
        {
            _targetPosition = planeHit.point;
            _targetPosition.y = _fixedHeight;

            Vector3 smoothedPosition = Vector3.Lerp(_heldObject.position, _targetPosition, _moveSpeed * Time.deltaTime);
            _heldObject.MovePosition(smoothedPosition);

            _releaseVelocity = (_targetPosition - _heldObject.position) * _moveSpeed;
        }
    }

    private void OnMouseReleased()
    {
        if (!_isHolding) return;

        if (_heldObject != null)
        {
            Vector3 releaseMomentum = _releaseVelocity * _momentumMultiplier;
            releaseMomentum.y = 0f;
            _heldObject.useGravity = true;
            _heldObject.freezeRotation = false;
            _heldObject.velocity = releaseMomentum;

            if (_debug) Debug.Log($"Object released with momentum: {releaseMomentum}");
        }

        ReleaseObject();
    }

    private bool HasLineOfSightToPlayer()
    {
        if (_heldObject == null) return false;

        Vector3 direction = _heldObject.position - transform.position;
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, direction.magnitude, _obstacleLayer))
        {
            if (_debug) Debug.Log($"LoS to player blocked by: {hit.collider.name}");
            return false;
        }

        return true;
    }

    private bool HasLineOfSightToMouse()
    {
        if (_heldObject == null) return false;

        Plane plane = new Plane(Vector3.up, new Vector3(0, _heldObject.position.y, 0));
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out float enter))
        {
            Vector3 intersectionPoint = ray.GetPoint(enter);
            Vector3 direction = intersectionPoint - _heldObject.position;

            if (Physics.Raycast(_heldObject.position, direction, out RaycastHit hit, direction.magnitude, _obstacleLayer))
            {
                if (_debug) Debug.Log($"LoS to mouse blocked by: {hit.collider.name}");
                return false;
            }

            return true;
        }

        return false;
    }

    private bool HasLineOfSightToGameObject(GameObject target)
    {
        Vector3 direction = target.transform.position - transform.position;
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, direction.magnitude, _obstacleLayer))
        {
            if (_debug) Debug.Log($"LoS to object blocked by: {hit.collider.name}");
            return false;
        }

        return true;
    }

    private void ReleaseObject()
    {
        if (_heldObject == null) return;

        _heldObject.useGravity = true;
        _heldObject.freezeRotation = false;
        _heldObject = null;
        _isHolding = false;

        if (_debug) Debug.Log("Object released.");
    }

    private void OnDrawGizmos()
    {
        if (!_onDrawGizmos || !_isHolding || _heldObject == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, _heldObject.position);
    }
}
