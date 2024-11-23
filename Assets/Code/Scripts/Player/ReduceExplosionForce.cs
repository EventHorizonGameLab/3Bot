using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ReduceExplosionForce : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, Range(0f, 10f)] private float velocity = 0.1f;
    [SerializeField, Range(0f, 10f)] private float angularVelocity = 0.1f;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        rb.constraints = rb.velocity.magnitude > velocity || rb.angularVelocity.magnitude > angularVelocity ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.FreezeRotation;
    }
}
