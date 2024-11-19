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
        if(!this.enabled) return;

        Debug.Log("Detected");
        transform.position = Vector3.zero;
    }

    public void OnDetectionLost()
    {
        if (!this.enabled) return;

        Debug.Log("Detection Lost");
        transform.position = _origin;
    }
}
