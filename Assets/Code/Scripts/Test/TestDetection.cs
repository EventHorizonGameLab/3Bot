using UnityEngine;

public class TestDetection : MonoBehaviour, IDetectable
{
    private Vector3 _origin;

    private void Start()
    {
        _origin = transform.position;
    }

    public void OnDetected()
    {
        Debug.Log("Detected");
        transform.position = Vector3.zero;
    }

    public void OnDetectionLost()
    {
        Debug.Log("Detection Lost");
        transform.position = _origin;
    }
}
