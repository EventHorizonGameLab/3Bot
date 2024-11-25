using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class BulletMagazine : MonoBehaviour, IInteractable
{
    [Title("Settings")]
    [SerializeField, MinValue(0), Tooltip("Number of bullets in the magazine")] private int bulletCount;

    public static event Action<int> OnReload;

    private void Start() { }

    public bool Interact()
    {
        OnReload?.Invoke(bulletCount);

        return true;
    }
}
