using System;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(ParticleSystem))]
public class GunSettings : BaseAudioHandler, IReloadable
{
    [Title("Settings")]
    [SerializeField, Tooltip("Max ammo"), MinValue(1)] private int _maxAmmo;
    [SerializeField, Tooltip("Fire rate"), MinValue(0f)] private float _fireRate;
    [SerializeField, Tooltip("Reload time"), MinValue(0f)] private float _reloadTime;
    [SerializeField, Tooltip("Damage"), MinValue(0)] private int _damage;
    [SerializeField, Tooltip("Attack Type")] private AttackType _attackType = AttackType.Null;

    [SerializeField, Tooltip("Flag indicating if the gun has infinite ammo")] private bool _hasInfiniteAmmo = true;

    [Title("Ammo Settings")]
    [SerializeField, HideIf("_hasInfiniteAmmo"), MinValue(1), Tooltip("Magazine size")] private int _magazineSize = 10;
    [ShowInInspector, HideIf("_hasInfiniteAmmo"), Tooltip("Total ammo in the gun"), ReadOnly] private int _totalAmmo;

    [Title("2D Settings")]
    [SerializeField, Tooltip("Flag indicating if the gun is 2D")] private bool _is2D = false;

    [Title("Damage Over Time Settings")]
    [SerializeField, Tooltip("Flag indicating if the gun causes damage over time.")] private bool causesDamageOverTime;
    [SerializeField, ShowIf("causesDamageOverTime")] private DamageOverTime _damageOverTime;

    [Serializable]
    public class DamageOverTime
    {
        [Tooltip("The damage inflicted every interval."), MinValue(0)]
        public int damagePerInterval = 0;

        [Tooltip("The duration (in seconds) for which the damage is applied."), MinValue(0)]
        public float damageDuration = 0f;

        [Tooltip("Delay between each damage"), MinValue(0)]
        public float interval = 0f;
    }

    [Title("Debug")]
    [ShowInInspector, PropertyOrder(2)] private bool _debug = true;
    [Tooltip("Current ammo"), ShowInInspector, PropertyOrder(2)]
    public int CurrentAmmo => _currentAmmo;

    [Button("Reload"), PropertyOrder(2)]
    public void ReloadButton(int amount = 0)
    {
        if (_hasInfiniteAmmo)
        {
            _currentAmmo = _maxAmmo;
        }
        else
        {
            _totalAmmo = amount;
            if (_currentAmmo <= 1) Reload();

            if (_debug) Debug.Log($"Current Ammo: {_currentAmmo} and Total Ammo: {_totalAmmo}");
        }
    }

    private int _currentAmmo;

    public event Action<int> OnAmmoChanged;
    public static Action<bool> OnReload;

    private ParticleSystem _gun;
    private float _timeSinceLastShot = 0f;
    private float _timeSinceReloadStarted = 0f;

    private float _offset = 100f;

    private bool _inRealod = false;

    private void Start()
    {
        _gun = GetComponentInChildren<ParticleSystem>();
        if (_hasInfiniteAmmo)
        {
            _currentAmmo = _maxAmmo;
        }
        else
        {
            _currentAmmo = _magazineSize;
            _totalAmmo = _maxAmmo;
        }

        _timeSinceLastShot = _fireRate;
    }

    private void OnEnable()
    {
        BulletMagazine.OnReload += ReloadButton;
    }

    private void OnDisable()
    {
        BulletMagazine.OnReload -= ReloadButton;
    }

    public void Shoot()
    {
        if (_debug) Debug.Log("Shoot Command Received");

        if (_currentAmmo <= 0 && !_hasInfiniteAmmo) return;

        if (_inRealod) return;

        _timeSinceLastShot += Time.deltaTime * _offset;

        //if (_debug) Debug.Log($" _timeSinceLastShot: {_timeSinceLastShot} >= _fireRate: {_fireRate}");

        if (_timeSinceLastShot >= _fireRate)
        {
            _timeSinceLastShot = 0f;
            _gun.Emit(1);
            Play();
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
        if (_debug) Debug.Log("Reload Command Received");

        Debug.Log($"_timeSinceReloadStarted: {_timeSinceReloadStarted} >= _reloadTime: {_reloadTime}");

        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        if (_debug) Debug.Log("Reloading");

        OnReload?.Invoke(true);
        _inRealod = true;

        if (_timeSinceReloadStarted > 0)
        {
            yield return new WaitForSeconds(_timeSinceReloadStarted);
        }
        _timeSinceReloadStarted = 0f;
        yield return new WaitForSeconds(_reloadTime);

        if (_debug) Debug.Log("Reloaded");

        if (_hasInfiniteAmmo)
        {
            _currentAmmo = _maxAmmo;
        }
        else
        {
            int ammoToLoad = Mathf.Min(_magazineSize, _totalAmmo);
            _currentAmmo = ammoToLoad;
            _totalAmmo -= ammoToLoad;
        }

        OnAmmoChanged?.Invoke(_currentAmmo);
        OnReload?.Invoke(false);
        _inRealod = false;
    }

    public AttackType AttackType => _attackType;

    public int Damage => _damage;

    public bool CausesDamageOverTime => causesDamageOverTime;

    public DamageOverTime GetDamageOverTime() => _damageOverTime;

    public bool Is2D => _is2D;
}

public enum AttackType
{
    Null, //Default
    Heal,
    Physical, //Hurt
    Fire,
    Ice,
    Electric, //EMP Gun
    Water,
    Poison,
    Light,
    Dark,
    Explosive //Grenade Launcher, Molot
}