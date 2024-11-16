using UnityEngine;
using UnityEngine.AI;

public class SegwayPitchControl : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float rotationAmplitude = 10f;  // Massima inclinazione
    [SerializeField] private Transform body;  // Riferimento al corpo del GameObject
    [SerializeField] private float smoothingFactor = 5f;  // Velocità di interpolazione
    [SerializeField] private bool invertPitchEffect = false;  // Inverte l'effetto di inclinazione

    private NavMeshAgent agent;
    private float previousVelocity = 0f;

    private void Start()
    {
        // Ottieni il componente NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        MotionVFX();
    }

    private void MotionVFX()
    {
        // Calcola la velocità corrente del navmesh agent
        float velocity = agent.velocity.magnitude;

        // Arrotonda la velocità per evitare oscillazioni a causa di piccoli cambiamenti
        float velocityRound = Mathf.Round(velocity * 100f) / 100f;

        // Calcola la variazione di velocità (acceleration/deceleration)
        float speed = (velocityRound - previousVelocity) / Time.deltaTime;

        // Imposta l'angolo di inclinazione in base alla velocità
        float rotationAngle = 0f;

        if (speed > 0) // Se si sta accelerando
        {
            rotationAngle = invertPitchEffect ? rotationAmplitude : -rotationAmplitude;
        }
        else if (speed < 0) // Se si sta decelerando
        {
            rotationAngle = invertPitchEffect ? -rotationAmplitude : rotationAmplitude;
        }

        // Aggiorna la velocità precedente
        previousVelocity = velocityRound;

        // Crea una rotazione target sull'asse X (beccheggio)
        Quaternion targetRotation = Quaternion.Euler(rotationAngle, 0f, 0f);

        // Interpola la rotazione locale per rendere l'effetto più fluido
        body.localRotation = Quaternion.Lerp(body.localRotation, targetRotation, smoothingFactor * Time.deltaTime);
    }
}
