using Sirenix.OdinInspector;
using UnityEngine;

public class BaseAudioHandler : MonoBehaviour
{
    [Title("Audio Settings")]
    [SerializeField, PropertyOrder(1)] private AudioClip _audioClip;
    [SerializeField, PropertyOrder(1)] private GameObject _audioSourcePrefab;

    // scrivere logica di prendere dal pooler il prefab e settargli la clip e rilevare la fine dell'aduio per riportare il prefab nel pooler
}
