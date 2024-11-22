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

    [Title("Momentum Settings")]
    [SerializeField, MinValue(0f), Tooltip("Multiplier for momentum when the object is released")] private float _momentumMultiplier = 50f;

    [Title("Debug")]
    [SerializeField] private bool _debug = false;
    [SerializeField, Tooltip("Show debug gizmos for the raycast")] private bool _onDrawGizmos = true;

    [ShowInInspector, ReadOnly] private bool _isEnabled = false;
    [ShowInInspector, ReadOnly] private bool _isHolding = false;
    [ShowInInspector, ReadOnly] private Rigidbody _heldObject = null;

    private Vector3 _targetPosition; // Target position for the object's movement
    private Vector3 _lastMousePosition; // To track the last mouse position for momentum calculation
    private Vector3 _releaseVelocity; // Velocity of the object when released
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
            // Continuous raycast from the player to the object
            if (!HasLineOfSight())
            {
                if (_debug) Debug.Log("Line of sight lost. Forced release.");
                ReleaseObject();
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

        if (_debug) Debug.Log("Object picked up");

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (_onDrawGizmos) Debug.DrawRay(ray.origin, ray.direction * _maxDistance, Color.green, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance, _interactableLayer))
        {
            if (_debug) Debug.Log("Object hit: " + hit.collider.name);

            if (hit.collider.TryGetComponent(out Rigidbody rb))
            {
                _heldObject = rb;
                _heldObject.useGravity = false;
                _heldObject.angularVelocity = Vector3.zero;
                _heldObject.freezeRotation = true;

                _isHolding = true;
                _lastMousePosition = Input.mousePosition; // Track the initial position
            }
        }
    }

    private void OnMouseHeld()
    {
        if (!_isHolding || _heldObject == null) return;

        // Raycast to calculate the target position on the XZ plane
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit planeHit, Mathf.Infinity, _obstacleLayer))
        {
            // Position the object while maintaining a fixed height
            _targetPosition = planeHit.point;
            _targetPosition.y = _fixedHeight;

            Vector3 smoothedPosition = Vector3.Lerp(_heldObject.position, _targetPosition, _moveSpeed * Time.deltaTime);
            _heldObject.MovePosition(smoothedPosition);

            // Calculate the velocity (momentum) based on mouse movement in the XZ plane
            Vector3 mouseDelta = Input.mousePosition - _lastMousePosition;

            // If there's movement, update the release velocity
            if (mouseDelta.magnitude > 0f)
            {
                _releaseVelocity = mouseDelta * _moveSpeed * Time.deltaTime;
                if (_debug) Debug.Log("Mouse delta (momentum direction, XZ only): " + mouseDelta);
            }

            _lastMousePosition = Input.mousePosition;
        }
    }

    private void OnMouseReleased()
    {
        if (!_isHolding) return;

        // Apply the release velocity to the object
        if (_heldObject != null)
        {
            _heldObject.useGravity = true;
            _heldObject.freezeRotation = false;

            // Apply the momentum (velocity) to the object when released
            // We apply velocity in the X and Z directions only
            Vector3 releaseMomentum = _releaseVelocity * _momentumMultiplier;
            releaseMomentum = new(releaseMomentum.y, _heldObject.velocity.y, -releaseMomentum.x);

            // Set the velocity to the release momentum
            _heldObject.velocity = releaseMomentum;

            if (_debug)
            {
                Debug.Log("Object released with momentum in XZ plane: " + releaseMomentum);
            }

            ReleaseObject();
        }
    }

    private bool HasLineOfSight()
    {
        if (_heldObject == null) return false;

        // Raycast from the player to the object
        Vector3 direction = _heldObject.position - transform.position;
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, direction.magnitude))
        {
            // If the hit object is not the selected one, the line of sight is blocked
            return hit.collider.attachedRigidbody == _heldObject;
        }

        return true;
    }

    private void ReleaseObject()
    {
        if (_heldObject == null) return;

        // Restore the object's physics settings
        _heldObject.useGravity = true;
        _heldObject.freezeRotation = false;
        _heldObject = null;
        _isHolding = false;

        if (_debug) Debug.Log("Object released.");
    }

    private void OnDrawGizmos()
    {
        if (!_onDrawGizmos || !_isHolding || _heldObject == null) return;

        // Draw a line from the player's position to the held object
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, _heldObject.position);
    }
}
