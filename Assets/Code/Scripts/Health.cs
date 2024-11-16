using System;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using static GunSettings;

[RequireComponent(typeof(Collider))]
public class Health : MonoBehaviour, IExplosionAffected
{
    [SerializeField] private int _maxHealth = 100;

    [Title("Flags")]
    [SerializeField] private bool _isPlayer = false;
    [SerializeField] private bool _friendlyFire = false;
    [SerializeField] private bool _isInvincible = false;

    [HideIf(nameof(_isInvincible))]
    [SerializeField] private List<VulnerabilitiesData> vulnerabilities = new();

    [System.Serializable]
    public struct VulnerabilitiesData
    {
        public AttackType attackType;
        [Range(-2f, 2f)] public float damageMultiplier;
    }

    [Title("Debug")]
    [SerializeField] private bool _debug = false;
    [ShowInInspector, ReadOnly] private int _currentHealth;

    /// <summary>
    /// Event triggered when health changes.
    /// </summary>
    public event Action<int> OnHealthChange;

    /// <summary>
    /// Event triggered when the entity dies.
    /// </summary>
    public event Action OnDeath;

    private void Start()
    {
        // Initialize current health to max health at the start
        _currentHealth = _maxHealth;

        if (_isPlayer) OnHealthChange?.Invoke(_currentHealth);
    }

    /// <summary>
    /// Handles particle collision. Applies damage based on collision with gun settings.
    /// Starts damage-over-time if applicable.
    /// </summary>
    /// <param name="other">The other game object involved in the collision</param>
    private void OnParticleCollision(GameObject other)
    {
        if (_debug) Debug.Log($"OnParticleCollision: {other.name}");

        // Skip if invincible or if friendly fire is disabled and the other object is an enemy
        if (_isInvincible || (_friendlyFire && other.CompareTag("Enemy"))) return;

        if (!other.TryGetComponent<GunSettings>(out var gunSettings)) return;

        int damage = gunSettings.Damage;

        if (gunSettings.CausesDamageOverTime)
        {
            var damageOverTimeData = gunSettings.GetDamageOverTime();

            if (damageOverTimeData != null)
                StartCoroutine(DamageOverTime(damageOverTimeData, gunSettings.AttackType));
        }

        // Apply damage based on the attack type
        ApplyDamage(damage, gunSettings.AttackType);
    }

    /// <summary>
    /// Applies damage to the entity considering vulnerability multipliers.
    /// If health reaches zero, calls Die() to trigger death behavior.
    /// </summary>
    /// <param name="damage">Amount of damage to apply</param>
    /// <param name="type">Attack type associated with the damage</param>
    private void ApplyDamage(int damage, AttackType type)
    {
        if (_debug) Debug.Log($"ApplyDamage: {damage}, {type}");

        // Apply vulnerability multiplier for the given attack type
        var vulnerability = vulnerabilities.Find(v => v.attackType == type);

        float damageMultiplier = vulnerability.attackType != AttackType.Null ? vulnerability.damageMultiplier : 1.0f;

        _currentHealth -= (int)(damage * damageMultiplier);

        if (_isPlayer) OnHealthChange?.Invoke(_currentHealth);

        if (_currentHealth <= 0) Die();
    }

    /// <summary>
    /// Handles the entity's death behavior. Calls OnDeath event for player or disables the object for others.
    /// </summary>
    private void Die()
    {
        if (_isPlayer)
        {
            OnDeath?.Invoke();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Coroutine to handle damage over time. Applies damage periodically based on the defined interval.
    /// </summary>
    /// <param name="data">Damage over time settings</param>
    /// <param name="type">Attack type associated with the damage</param>
    private IEnumerator DamageOverTime(DamageOverTime data, AttackType type)
    {
        float elapsedTime = 0f;

        // Continue applying damage until the specified duration is complete
        while (elapsedTime < data.damageDuration)
        {
            yield return new WaitForSeconds(data.interval);

            ApplyDamage(data.damagePerInterval, type);

            elapsedTime += data.interval;
        }
    }

    public void OnExplosion(Vector3 explosionPosition, float explosionForce)
    {
        ApplyDamage((int)explosionForce, AttackType.Explosive);
    }
}
