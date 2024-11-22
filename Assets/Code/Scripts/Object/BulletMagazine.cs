using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class BulletMagazine : MonoBehaviour, IInteractable
{
    [Title("Settings")]
    [SerializeField, MinValue(0), Tooltip("Number of bullets in the magazine")] private int bulletCount;

    public static Action<int> OnReload;

    public void Interact()
    {
        OnReload?.Invoke(bulletCount);
    }
}
