using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Key : MonoBehaviour, IInteractable
{
    [Title("Settings")]
    [SerializeField, Required] private GameObject _door;
    [SerializeField, MinValue(0)] private float range = 5f;

    [Serializable]
    public class TriggerEnterEvent : UnityEvent { }

    [SerializeField]
    private TriggerEnterEvent _onInteract = new();

    [Title("Debug")]
    [SerializeField] private bool _debug = false;
    [SerializeField] private bool _onDrawGizmos = false;

    private void Start() { }

    public bool Interact()
    {
        if (!_debug) Debug.Log("Interact: Key");

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject == _door)
            {
                _onInteract?.Invoke();
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        if (!_onDrawGizmos) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public GameObject DoorPrefab => _door;
}
