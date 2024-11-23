using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider)), RequireComponent(typeof(Rigidbody))]
public class TriggerEnter : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, Tooltip("The LayerMask to filter which objects can trigger this event.")] private LayerMask _layer;
    [SerializeField, Tooltip("Whether to trigger the event only once.")] private bool _triggerOnce = true;

    [Title("Events")]
    [Serializable]
    public class TriggerEnterEvent : UnityEvent { }

    [SerializeField, Tooltip("Event triggered when the player enters the trigger zone.")]
    private TriggerEnterEvent _onTriggerEnter = new();

    [Title("Debug")]
    [SerializeField] private bool _debug = false;

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
}
