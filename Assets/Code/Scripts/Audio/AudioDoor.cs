using Audio;
using UnityEngine;

public class AudioDoor : MonoBehaviour
{
    public void PlayAudio(string audioClipName)
    {
        AudioManager.Instance.PlaySFX(audioClipName, transform);
    }
}
