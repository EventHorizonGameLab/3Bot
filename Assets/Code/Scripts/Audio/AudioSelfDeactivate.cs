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
        GetComponent<AudioSource>().clip = null;
        ObjectPooler.Instance.ReturnToPool(gameObject);
    }
}
