using Sirenix.OdinInspector;
using UnityEngine;

public class BaseAudioHandler : MonoBehaviour
{
    [Title("Audio Settings")]
    [SerializeField, PropertyOrder(1)] private AudioClip _audioClip;
    [SerializeField, PropertyOrder(1)] private GameObject _audioSourcePrefab;

    /// <summary>
    /// Plays the audio clip and returns the prefab to the pool after completion.
    /// </summary>
    protected virtual void Play()
    {
        // Ottieni il prefab dall'ObjectPooler
        var audioSourceObject = ObjectPooler.Instance.Get(_audioSourcePrefab);
        
        if (!audioSourceObject.TryGetComponent<AudioSource>(out var audioSource))
        {
            Debug.LogError("AudioSource component missing from the prefab.");
            return;
        }

        // Imposta la clip audio
        audioSource.clip = _audioClip;

        // Avvia la riproduzione
        audioSource.Play();
    }
}
