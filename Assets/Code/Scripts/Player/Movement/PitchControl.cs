using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class SegwayPitchControl : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, Tooltip("The amplitude of the pitch rotation"), MinValue(0)] private float rotationAmplitude = 10f;
    [SerializeField, Required, Tooltip("The Mesh of the Player")] private Transform body;
    [SerializeField, Tooltip("The smoothing factor of the pitch rotation"), Range(0, 30)] private float smoothingFactor = 5f;
    [SerializeField] private bool invertPitchEffect = false;

    private NavMeshAgent agent;
    private float previousVelocity = 0f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        MotionVFX();
    }

    private void MotionVFX()
    {
        float velocity = agent.velocity.magnitude;

        float velocityRound = Mathf.Round(velocity * 100f) / 100f;

        float speed = (velocityRound - previousVelocity) / Time.deltaTime;

        float rotationAngle = 0f;

        if (speed > 0)
        {
            rotationAngle = invertPitchEffect ? rotationAmplitude : -rotationAmplitude;
        }
        else if (speed < 0)
        {
            rotationAngle = invertPitchEffect ? -rotationAmplitude : rotationAmplitude;
        }

        previousVelocity = velocityRound;

        Quaternion targetRotation = Quaternion.Euler(rotationAngle, 0f, 0f);

        body.localRotation = Quaternion.Lerp(body.localRotation, targetRotation, smoothingFactor * Time.deltaTime);
    }
}
