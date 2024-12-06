using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider)), RequireComponent(typeof(Rigidbody))]
public class TriggerEnter : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, Tooltip("The LayerMask to filter which objects can trigger this event.")]
    private LayerMask _layer;
    [SerializeField, Tooltip("Whether to trigger the event only once.")]
    private bool _triggerOnce = true;
    [SerializeField] Color _gizmoColor = new Color(0, 1, 0, 0.3f);

    [Title("Events")]
    [Serializable]
    public class TriggerEnterEvent : UnityEvent { }

    [SerializeField, Tooltip("Event triggered when the player enters the trigger zone.")]
    private TriggerEnterEvent _onTriggerEnter = new();

    [Title("Debug")]
    [SerializeField] private bool _debug = false;
    [SerializeField] private bool _showColliderInEditor = true; // Toggle to show/hide Gizmos

    private void Start()
    {
        if (TryGetComponent(out Collider collider))
        {
            collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_debug) Debug.Log($"Trigger Enter: {other.name}");

        // Check if the object's layer is in the specified LayerMask
        if ((_layer.value & (1 << other.gameObject.layer)) != 0)
        {
            if (_triggerOnce) gameObject.SetActive(false);
            _onTriggerEnter?.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        if (!_showColliderInEditor) return;

        // Get the Collider component
        Collider collider = GetComponent<Collider>();
        if (collider == null) return;

        // Set Gizmos color
        Gizmos.color = _gizmoColor; // Green with transparency

        // Draw the Collider based on its type
        if (collider is BoxCollider boxCollider)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(boxCollider.center, boxCollider.size);
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
        }
        else if (collider is SphereCollider sphereCollider)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawSphere(sphereCollider.center, sphereCollider.radius);
            Gizmos.DrawWireSphere(sphereCollider.center, sphereCollider.radius);
        }
        else if (collider is CapsuleCollider capsuleCollider)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            DrawCapsuleGizmos(capsuleCollider);
        }
        else if (collider is MeshCollider meshCollider && meshCollider.sharedMesh != null)
        {
            Gizmos.DrawMesh(meshCollider.sharedMesh, transform.position, transform.rotation, transform.lossyScale);
        }

        Gizmos.matrix = Matrix4x4.identity; // Reset matrix
    }

    // Helper to draw a Capsule Collider
    private void DrawCapsuleGizmos(CapsuleCollider capsule)
    {
        Vector3 center = capsule.center;
        float radius = capsule.radius;
        float height = Mathf.Max(0, capsule.height / 2 - radius);

        // Draw main capsule body
        Gizmos.DrawWireSphere(center + Vector3.up * height, radius);
        Gizmos.DrawWireSphere(center - Vector3.up * height, radius);
        Gizmos.DrawLine(center + Vector3.up * height + Vector3.right * radius, center - Vector3.up * height + Vector3.right * radius);
        Gizmos.DrawLine(center + Vector3.up * height - Vector3.right * radius, center - Vector3.up * height - Vector3.right * radius);
        Gizmos.DrawLine(center + Vector3.up * height + Vector3.forward * radius, center - Vector3.up * height + Vector3.forward * radius);
        Gizmos.DrawLine(center + Vector3.up * height - Vector3.forward * radius, center - Vector3.up * height - Vector3.forward * radius);
    }
}
