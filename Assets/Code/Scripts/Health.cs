using System;
using System.Collections;
using UnityEngine;
using static GunSettings;

[RequireComponent(typeof(Collider))]
public class Health : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;

    [Header("Flags")]
    [SerializeField] private bool _isPlayer = false;
    [SerializeField] private bool _friendlyFire = false;
    [SerializeField] private bool _isInvincible = false;

    public event Action<int> OnHealthChange;  // Player UI
    public event Action OnDeath;              // Player Death

    [Header("Debug")]
    [SerializeField] private int _currentHealth;

    private void Start()
    {
        _currentHealth = _maxHealth;

        if (_isPlayer) OnHealthChange?.Invoke(_currentHealth);
    }

    private void OnParticleCollision(GameObject other)
    {
        if (_isInvincible || (_friendlyFire && other.CompareTag("Enemy"))) return;


        if (!other.TryGetComponent<GunSettings>(out var gunSettings)) return;

        int damage = gunSettings.Damage;
        if (gunSettings.CausesDamageOverTime)
        {
            var damageOverTimeData = gunSettings.GetDamageOverTime();

            if (damageOverTimeData != null) StartCoroutine(DamageOverTime(damageOverTimeData));
        }

        ApplyDamage(damage);
    }

    private void ApplyDamage(int damage)
    {
        _currentHealth -= damage;

        if (_isPlayer) OnHealthChange?.Invoke(_currentHealth);

        if (_currentHealth <= 0) Die();
    }

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

    private IEnumerator DamageOverTime(DamageOverTime data)
    {
        float elapsedTime = 0f;

        while (elapsedTime < data.damageDuration)
        {
            yield return new WaitForSeconds(data.interval);

            ApplyDamage(data.damagePerInterval);

            elapsedTime += data.interval;
        }
    }

}
