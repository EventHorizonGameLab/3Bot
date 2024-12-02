using Sirenix.OdinInspector;
using UnityEngine;

public class BladeRotation : MonoBehaviour
{
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    [Title("Settings")]
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private RotationAxis rotationAxis = RotationAxis.Z;

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
        transform.Rotate(rotationSpeed * Time.deltaTime * axis);
    }
}
