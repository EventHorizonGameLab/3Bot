using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class WheelRotation : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, Tooltip("The amplitude of the wheel rotation speed"), MinValue(0)] private float rotationAmplitude = 300f;
    [SerializeField, Tooltip("The smoothing factor of the wheel rotation"), Range(0, 10)] private float smoothingFactor = 5f;
    [SerializeField, Tooltip("The maximum speed for the wheel rotation")] private float maxSpeed = 1000f;
    [SerializeField, Required, Tooltip("The NavMeshAgent of the vehicle")] private NavMeshAgent vehicleAgent;

    private float previousVelocity = 0f;

    void Start() { }

    void Update()
    {
        WheelRotationControl();
    }

    private void WheelRotationControl()
    {
        float velocity = vehicleAgent.velocity.magnitude;
        float velocityForRotation = velocity * rotationAmplitude;

        if (velocityForRotation < 0.1f)
        {
            velocityForRotation = 0f;
        }

        float smoothedRotationSpeed = Mathf.Lerp(previousVelocity, velocityForRotation, smoothingFactor * Time.deltaTime);
        transform.Rotate(0f, -smoothedRotationSpeed * Time.deltaTime, 0f);

        previousVelocity = smoothedRotationSpeed;
    }
}
