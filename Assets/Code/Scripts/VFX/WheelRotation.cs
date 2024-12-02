using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class WheelRotation : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, Tooltip("The amplitude of the wheel rotation speed"), MinValue(0)] private float rotationAmplitude = 300f;
    [SerializeField, Tooltip("The smoothing factor of the wheel rotation"), Range(0, 10)] private float smoothingFactor = 5f;
    [SerializeField, Required, Tooltip("The NavMeshAgent of the vehicle")] private NavMeshAgent vehicleAgent;
    [SerializeField] private RotationAxis rotationAxis = RotationAxis.Z;

    private float previousVelocity = 0f;

    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    Vector3 axis = Vector3.zero;

    private void Start()
    {
        switch (rotationAxis)
        {
            case RotationAxis.X:
                axis = Vector3.right;
                break;
            case RotationAxis.Y:
                axis = Vector3.up;
                break;
            case RotationAxis.Z:
                axis = Vector3.forward;
                break;
        }
    }

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


        transform.Rotate(-smoothedRotationSpeed * Time.deltaTime * axis);

        previousVelocity = smoothedRotationSpeed;
    }
}
