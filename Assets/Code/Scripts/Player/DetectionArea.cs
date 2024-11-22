using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;

public class DetectionArea : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, MinValue(0), Tooltip("Radius of the detection area.")] private float _detectionRadius = 5f;
    [SerializeField, Tooltip("Layer of the detection area.")] private LayerMask _detectionLayer;

    [Title("Debug")]
    [SerializeField] private bool _debug;
    [SerializeField] private bool _showGizmos;

    private HashSet<Collider> _currentlyDetected = new();

    private void Update()
    {
        if(!this.enabled) return;

        Detection();
    }

    private void Detection()
    {
        if (_debug) Debug.Log($"Detection Radius: {_detectionRadius}.");

        // Get the objects currently in the detection area
        Collider[] colliders = Physics.OverlapSphere(transform.position, _detectionRadius, _detectionLayer);
        HashSet<Collider> updatedDetected = new(colliders);

        // Find objects that are no longer detected
        foreach (var obj in _currentlyDetected)
        {
            if (!updatedDetected.Contains(obj) && obj.TryGetComponent<IDetectable>(out var detectable))
            {
                detectable.OnDetectionLost();
                if (_debug) Debug.Log($"Detection Lost: {obj.name}");
            }
        }

        // Find newly detected objects
        foreach (var obj in updatedDetected)
        {
            if (!_currentlyDetected.Contains(obj) && obj.TryGetComponent<IDetectable>(out var detectable))
            {
                detectable.OnDetected();
                if (_debug) Debug.Log($"Detected: {obj.name}");
            }
        }

        // Update the set of currently detected objects
        _currentlyDetected = updatedDetected;
    }

    private void OnDrawGizmos()
    {
        if (!_showGizmos || !this.enabled) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}
