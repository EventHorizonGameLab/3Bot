using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSelfDeactivate : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(ReturnToPool(GetComponent<AudioSource>().clip.length));
    }

    private IEnumerator ReturnToPool(float duration)
    {
        yield return new WaitForSeconds(duration);
        ObjectPooler.Instance.ReturnToPool(gameObject);
    }
}
