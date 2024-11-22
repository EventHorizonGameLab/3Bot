using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class MedKit : MonoBehaviour, IInteractable
{
    [Title("Settings")]
    [SerializeField, MinValue(0), Tooltip("The amount of health restored")] private float _healAmount;

    public static Action<float, AttackType> OnHeal;

    public void Interact()
    {
       OnHeal?.Invoke(-_healAmount, AttackType.Heal);
    }  
}
