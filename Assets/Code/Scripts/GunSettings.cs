using System;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(ParticleSystem))]
public class GunSettings : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Tooltip("Max ammo"), Min(1)] private int _maxAmmo;
    [SerializeField, Tooltip("Fire rate"), Min(0f)] private float _fireRate;
    [SerializeField, Tooltip("Reload time"), Min(0f)] private float _reloadTime;
    [SerializeField, Tooltip("Damage"), Min(0)] private int _damage;

    [SerializeField] private bool _is2D = false;

    [Header("Damage Over Time Settings")]
    [SerializeField, Tooltip("Flag indicating if the gun causes damage over time.")] private bool causesDamageOverTime;
    [SerializeField, ShowIf("causesDamageOverTime")] private DamageOverTime _damageOverTime; // qui � stato usato

    [System.Serializable]
    public class DamageOverTime
    {
        [Tooltip("The damage inflicted every interval."), Min(0)]
        public int damagePerInterval = 0;

        [Tooltip("The duration (in seconds) for which the damage is applied."), Min(0)]
        public float damageDuration = 0f;

        [Tooltip("Delay between each damage"), Min(0)]
        public float interval = 0f;
    }

    [Header("Debug")]
    [SerializeField] private bool _debug = true;
    [SerializeField, Tooltip("Current ammo")] private int _currentAmmo;

    public event Action<int> OnAmmoChanged;

    private ParticleSystem _gun;
    private float _timeSinceLastShot = 0f;
    private float _timeSinceReloadStarted = 0f;

    private float _offset = 100f;

    private void Start()
    {
        _gun = GetComponentInChildren<ParticleSystem>();
        _currentAmmo = _maxAmmo;
        _timeSinceLastShot = _fireRate;
    }

    public void Shoot()
    {
        if (_debug) Debug.Log("Shoot Command Received");

        _timeSinceLastShot += Time.deltaTime * _offset;

        if (_debug) Debug.Log($" _timeSinceLastShot: {_timeSinceLastShot} >= _fireRate: {_fireRate}");

        if (_timeSinceLastShot >= _fireRate)
        {
            _timeSinceLastShot = 0f;
            _gun.Emit(1);
            _currentAmmo--;

            if (_debug) Debug.Log("Shooting");

            OnAmmoChanged?.Invoke(_currentAmmo);

            if (_currentAmmo <= 0)
            {
                Reload();
            }
        }
    }

    public void Reload()
    {
        if (_timeSinceReloadStarted >= _reloadTime)
        {
            _timeSinceReloadStarted = 0f;
            StartCoroutine(ReloadCoroutine());
        }
    }

    private IEnumerator ReloadCoroutine()
    {
        if (_debug) Debug.Log("Reloading");

        while (_timeSinceReloadStarted < _reloadTime)
        {
            _timeSinceReloadStarted += Time.deltaTime * _offset;
            yield return null;
        }

        if (_debug) Debug.Log("Reloaded");

        _currentAmmo = _maxAmmo;
        OnAmmoChanged?.Invoke(_currentAmmo);
    }

    public int Damage => _damage;

    public bool CausesDamageOverTime => causesDamageOverTime;

    public DamageOverTime GetDamageOverTime() => _damageOverTime;

    public bool Is2D => _is2D;
}
