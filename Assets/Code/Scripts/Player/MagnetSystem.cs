using UnityEngine;
using System;
using Sirenix.OdinInspector;
using PlayerSM;

public class MagnetSystem : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, Tooltip("LayerMask for interactable objects")] private LayerMask interactableLayer;
    [SerializeField, MinValue(0f), Tooltip("Speed at which the object floats to the player")] private float floatSpeed = 5f;
    [SerializeField, MinValue(0f), Tooltip("Distance to check for storing objects (SphereCast radius)")] private float sphereCastRadius = 1f;
    [SerializeField, Tooltip("LayerMask for obstacles")] private LayerMask obstacleLayer;
    [SerializeField, Tooltip("Position offset for storing the object on the head")] private Vector3 headOffset;

    [Title("Offsets")]
    [SerializeField, MinValue(0f), Tooltip("Vertical offset to lift the floating object")]
    private float floatingHeightOffset = 2f;

    [Title("Debug")]
    [SerializeField] private bool _debug = false;
    [SerializeField] private bool _drawGizmos = false;
    [ShowInInspector, ReadOnly] private bool _isEnabled = false;
    [ShowInInspector, ReadOnly] private GameObject _slot;

    public Action<GameObject> OnObjectStored; // Action triggered when an object is stored

    private Camera _mainCamera;
    private Transform _playerHeadTransform;
    private GameObject _currentFloatingObject;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _playerHeadTransform = transform; // Assuming the script is attached to the player
    }

    private void Update()
    {
        if (!_isEnabled) return;

        if (Input.GetMouseButtonDown(1)) HandleMagnetAction();
        HandleFloatingObject();
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

    private void HandleMagnetAction()
    {
        if (_currentFloatingObject != null)
        {
            // Stop floating and release the object
            StopFloatingObject();
            return;
        }

        // Start floating an object
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (_drawGizmos) Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, interactableLayer))
        {
            if (HasLineOfSight(transform.position, hit.collider.transform.position))
            {
                _currentFloatingObject = hit.collider.gameObject;

                if (_currentFloatingObject.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.useGravity = false;
                    rb.velocity = Vector3.zero;
                }

                if (_debug) Debug.Log($"Started floating object: {_currentFloatingObject.name}");
            }
        }
    }

    private void HandleFloatingObject()
    {
        if (_currentFloatingObject == null) return;

        // Check line of sight
        if (!HasLineOfSight(transform.position, _currentFloatingObject.transform.position))
        {
            StopFloatingObject();
            if (_debug) Debug.Log($"Line of sight lost for {_currentFloatingObject.name}");
            return;
        }

        // Move the object towards the player
        Vector3 targetPosition = _playerHeadTransform.position + headOffset;
        targetPosition.y += floatingHeightOffset; // Apply the height offset
        _currentFloatingObject.transform.position = Vector3.MoveTowards(_currentFloatingObject.transform.position, targetPosition, floatSpeed * Time.deltaTime);

        // Check if the object enters the sphere cast
        if (Vector3.Distance(_currentFloatingObject.transform.position, targetPosition) <= sphereCastRadius)
        {
            StoreObject();
        }
    }

    private void StopFloatingObject()
    {
        if (_currentFloatingObject != null)
        {
            // Allow the object to fall back to the ground
            if (_currentFloatingObject.TryGetComponent<Rigidbody>(out var rb)) rb.useGravity = true;

            _currentFloatingObject = null;

            if (_debug) Debug.Log("Stopped floating object.");
        }
    }

    private void StoreObject()
    {
        if (_slot == null)
        {
            // Store the object in the slot
            _slot = _currentFloatingObject;
            _slot.SetActive(false);

            _currentFloatingObject = null;

            OnObjectStored?.Invoke(_slot);

            if (_debug) Debug.Log($"Stored object: {_slot.name}");
        }
        else if (_currentFloatingObject != null)
        {
            // Stop floating the current object
            StopFloatingObject();
        }
    }

    private void DropStoredObject()
    {
        if (_slot != null)
        {
            _slot.SetActive(true);
            _slot = null;

            if (_debug) Debug.Log("Dropped stored object.");
        }
    }

    private bool HasLineOfSight(Vector3 origin, Vector3 target)
    {
        Vector3 direction = target - origin;

        if (_drawGizmos) Debug.DrawRay(origin, direction, Color.red, 1f);

        if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, direction.magnitude, obstacleLayer))
        {
            if (_debug) Debug.Log($"Line of sight blocked by: {hit.collider.name}");
            return false;
        }
        return true;
    }

    private void OnDrawGizmos()
    {
        if (!_drawGizmos) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + headOffset, sphereCastRadius);
    }
}
