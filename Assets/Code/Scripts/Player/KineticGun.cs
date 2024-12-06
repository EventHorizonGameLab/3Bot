using PlayerSM;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class KineticGun : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, MinValue(0f), Tooltip("Speed at which the selected object follows the mouse")] private float _moveSpeed = 10f;
    [SerializeField, MinValue(0f), Tooltip("Height at which to position the selected object")] private float _fixedHeight = 2f;
    [SerializeField, Tooltip("LayerMask to identify interactable objects")] private LayerMask _interactableLayer;
    [SerializeField, Tooltip("LayerMask to identify obstacle objects")] private LayerMask _obstacleLayer;
    [SerializeField, MinValue(0f), Tooltip("Maximum distance for the raycast")] private float _maxDistance = 10f;
    [SerializeField, MinValue(0f), Tooltip("Multiplier for momentum on release")] private float _momentumMultiplier = 1.5f;
    [SerializeField] private GameObject _crosshair;

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

                if (_heldObject.TryGetComponent<NavMeshAgent>(out var agent))
                {
                    if (_debug) Debug.Log("Agent disabled.");

                    agent.enabled = false;
                }

                _isHolding = true;
            }
        }
    }

    private void OnMouseHeld()
    {
        if (!_isHolding || _heldObject == null) return;

        Plane plane = new(Vector3.up, new Vector3(0, _heldObject.position.y, 0));
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (_onDrawGizmos) Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red);

        if (plane.Raycast(ray, out float enter))
        {
            _targetPosition = ray.GetPoint(enter);
            _targetPosition.y = _fixedHeight;

            if (IsPathClear(_heldObject.position, _targetPosition))
            {
                Vector3 smoothedPosition = Vector3.Lerp(_heldObject.position, _targetPosition, _moveSpeed * Time.deltaTime);
                _heldObject.MovePosition(smoothedPosition);
                _releaseVelocity = (_targetPosition - _heldObject.position) * _moveSpeed;

                if (_crosshair)
                {
                    if (_debug) Debug.Log("Crosshair enabled.");

                    PositionIndicatorBelowHeldObject();
                }
            }
            else
            {
                if (_debug) Debug.Log("Path blocked by an obstacle. Object not moved.");
            }
        }
    }

    private bool IsPathClear(Vector3 start, Vector3 target)
    {
        Vector3 direction = target - start;
        float distance = direction.magnitude;

        if (_onDrawGizmos) Debug.DrawRay(start, direction, Color.yellow);

        if (Physics.Raycast(start, direction.normalized, out RaycastHit hit, distance, _obstacleLayer))
        {
            if (_debug) Debug.Log($"Path blocked by: {hit.collider.name}");
            return false;
        }

        return true;
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

            if (_heldObject.TryGetComponent<NavMeshAgent>(out var agent))
            {
                if (_debug) Debug.Log("Agent enabled.");

                agent.enabled = true;
            }

            _crosshair.SetActive(false);

            if (_debug) Debug.Log($"Object released with momentum: {releaseMomentum}");
        }

        ReleaseObject();
    }

    private bool HasLineOfSightToPlayer()
    {
        if (_heldObject == null) return false;

        Vector3 direction = _heldObject.position - transform.position;

        if (_onDrawGizmos) Debug.DrawRay(transform.position, direction, Color.blue);

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

        Plane plane = new(Vector3.up, new Vector3(0, _heldObject.position.y, 0));

        if (_onDrawGizmos) Debug.DrawRay(_mainCamera.transform.position, Input.mousePosition, Color.blue);

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (_onDrawGizmos) Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.blue);

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

        Vector3 position = transform.position;
        position.y = _fixedHeight;
        if (_onDrawGizmos) Debug.DrawLine(position, direction, Color.blue, 1f);

        if (Physics.Raycast(position, direction, out RaycastHit hit, direction.magnitude, _obstacleLayer))
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

    // Function to position the indicator under the held object
    private void PositionIndicatorBelowHeldObject()
    {
        if (_debug) Debug.Log("Positioning crosshair.");

        if (_heldObject == null || _crosshair == null) return;

        Ray ray = new(_heldObject.position, Vector3.down);

        if (_onDrawGizmos) Debug.DrawRay(_heldObject.position, Vector3.down, Color.green);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _obstacleLayer))
        {
            if (_debug) Debug.Log($"Crosshair position: {hit.point}");

            _crosshair.transform.position = hit.point;
            _crosshair.SetActive(true);
        }
        else
        {
            _crosshair.SetActive(false);
        }
    }

    #region Gizmos

    private void OnDrawGizmos()
    {
        if (!_onDrawGizmos || !_isHolding || _heldObject == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, _heldObject.position);

        if (_onDrawGizmos)
        {
            DrawPlane(_fixedHeight);
        }
    }

    private void DrawPlane(float planeHeight)
    {
        float size = 10f;

        Gizmos.color = Color.green;

        float x, z;
        x = transform.position.x;
        z = transform.position.z;

        for (float i = -size; i <= size; i += 1f)
        {
            Gizmos.DrawLine(new Vector3(x + i, planeHeight, z - size), new Vector3(x + i, planeHeight, x + size));
            Gizmos.DrawLine(new Vector3(x - size, planeHeight, z + i), new Vector3(x + size, planeHeight, z + i));
        }
    }

    #endregion
}
