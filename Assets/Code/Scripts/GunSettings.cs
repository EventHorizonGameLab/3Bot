using UnityEngine;

public class GunSettings : MonoBehaviour
{
    [SerializeField] private int _damage;
    public int Damage => _damage;
}
