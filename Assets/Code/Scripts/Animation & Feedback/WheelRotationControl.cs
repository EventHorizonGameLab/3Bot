using UnityEngine;

public class WheelRotation : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Tooltip("The radius of the wheel in Unity units"), Min(0.01f)] private float wheelRadius = 0.5f;
    [SerializeField, Tooltip("Invert the rotation direction for the wheel")] private bool invertRotation = false;

    private Transform parentTransform;
    private Vector3 lastParentPosition;
    private float totalRotationX = 0f;

    private void Start()
    {
        parentTransform = transform.parent;
        if (parentTransform == null)
        {
            Debug.LogError("The wheel must have a parent object.");
            enabled = false;
            return;
        }

        // Salva la posizione iniziale del padre
        lastParentPosition = parentTransform.position;
    }

    private void Update()
    {
        UpdateWheelRotation();
        SyncWithParentRotation();
    }

    private void UpdateWheelRotation()
    {
        // Calcola la distanza percorsa dal padre
        Vector3 parentMovement = parentTransform.position - lastParentPosition;
        float distanceTraveled = Vector3.Dot(parentMovement, parentTransform.forward);

        // Calcola l'angolo di rotazione sull'asse X in base alla distanza percorsa
        float rotationAngleX = (distanceTraveled / (2 * Mathf.PI * wheelRadius)) * 360f;

        // Inverti la direzione se necessario
        if (invertRotation)
        {
            rotationAngleX = -rotationAngleX;
        }

        // Accumula la rotazione totale sull'asse X
        totalRotationX += rotationAngleX;

        // Applica la rotazione solo sull'asse X
        transform.localRotation = Quaternion.Euler(totalRotationX, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);

        // Aggiorna la posizione del padre per il prossimo frame
        lastParentPosition = parentTransform.position;
    }

    private void SyncWithParentRotation()
    {
        // Sincronizza la rotazione globale Y con il padre, mantenendo l'asse X invariato
        transform.rotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, parentTransform.rotation.eulerAngles.y, 0f);
    }
}
