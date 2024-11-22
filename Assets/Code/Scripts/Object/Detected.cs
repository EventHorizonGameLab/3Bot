using UnityEngine;

[RequireComponent(typeof(Outline))]
public class Detected : MonoBehaviour, IDetectable
{
    private Outline _outline;

    private void Start()
    {
        _outline = GetComponent<Outline>();
        _outline.enabled = false;
    }

    public void OnDetected()
    {
        if (!this.enabled) return;

        Debug.Log("Detected");
        _outline.enabled = true;
    }

    public void OnDetectionLost()
    {
        if (!this.enabled) return;

        Debug.Log("Detection Lost");
        _outline.enabled = false;
    }
}
