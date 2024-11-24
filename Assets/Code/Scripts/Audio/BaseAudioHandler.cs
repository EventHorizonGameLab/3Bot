using Sirenix.OdinInspector;
using UnityEngine;
using Audio;

public class BaseAudioHandler : MonoBehaviour
{
    [Title("Audio Settings")]
    [SerializeField, PropertyOrder(1)] protected string _audioClipName;

    /// <summary>
    /// Plays the audio clip and returns the prefab to the pool after completion.
    /// </summary>
    protected virtual void Play(string audioClipName)
    {
        AudioManager.Instance.PlaySFX(audioClipName, transform);
    }
}
