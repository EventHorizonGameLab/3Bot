using System.Collections;
using UnityEngine;

public class DelaySelfDeactivate : MonoBehaviour
{
    [SerializeField, Min(0)] private float delay = 0.5f;

    private void Start()
    {
        StartCoroutine(ReturnToPool(delay));
    }

    private IEnumerator ReturnToPool(float duration)
    {
        yield return new WaitForSeconds(duration);
        ObjectPooler.Instance.ReturnToPool(gameObject);
    }
}
