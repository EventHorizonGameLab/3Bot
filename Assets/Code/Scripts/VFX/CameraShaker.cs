using UnityEngine;
using Cinemachine;
using Sirenix.OdinInspector;
using System.Collections;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraShaker : MonoBehaviour
{
    [Title("Settings")]
    [ShowInInspector, ReadOnly, Tooltip("Cinemachine Noise Profile")] private NoiseSettings _default;
    [SerializeField, MinValue(0f)] private float defaultAmplitude = 1f;

    // Shake Settings
    [BoxGroup("Damage Shake", false)]
    [SerializeField, Required, NoiseSettingsProperty] private NoiseSettings damageNoise;
    [BoxGroup("Damage Shake", false)]
    [SerializeField, MinValue(0f)] private float damageDuration = 0.3f;
    [BoxGroup("Damage Shake", false)]
    [SerializeField, Tooltip("Curve for fade-in effect")] private AnimationCurve damageInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [BoxGroup("Damage Shake", false)]
    [SerializeField, Tooltip("Curve for fade-out effect")] private AnimationCurve damageOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [BoxGroup("Explosion Shake", false)]
    [SerializeField, Required, NoiseSettingsProperty] private NoiseSettings explosionNoise;
    [BoxGroup("Explosion Shake", false)]
    [SerializeField, MinValue(0f)] private float explosionDuration = 0.7f;
    [BoxGroup("Explosion Shake", false)]
    [SerializeField, Tooltip("Curve for fade-in effect")] private AnimationCurve explosionInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [BoxGroup("Explosion Shake", false)]
    [SerializeField, Tooltip("Curve for fade-out effect")] private AnimationCurve explosionOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [BoxGroup("Shoot Shake", false)]
    [SerializeField, Required, NoiseSettingsProperty] private NoiseSettings shootNoise;
    [BoxGroup("Shoot Shake", false)]
    [SerializeField, MinValue(0f)] private float shootDuration = 0.2f;
    [BoxGroup("Shoot Shake", false)]
    [SerializeField, Tooltip("Curve for fade-in effect")] private AnimationCurve shootInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [BoxGroup("Shoot Shake", false)]
    [SerializeField, Tooltip("Curve for fade-out effect")] private AnimationCurve shootOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Title("Debug")]
    [SerializeField] private bool _debug;
    [ShowInInspector, ReadOnly] private CinemachineBasicMultiChannelPerlin noise;

    private void Start()
    {
        noise = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _default = noise.m_NoiseProfile;
        noise.m_AmplitudeGain = defaultAmplitude;
    }

    private void OnEnable()
    {
        GunSettings.Shooted += TriggerShootShake;
        Health.OnTakeDamage += TriggerDamageShake;
    }

    private void OnDisable()
    {
        GunSettings.Shooted -= TriggerShootShake;
        Health.OnTakeDamage -= TriggerDamageShake;
    }

    [Button("Trigger Damage Shake")]
    public void TriggerDamageShake(AttackType attackType)
    {
        if (_debug) Debug.Log($"Damage Shake: {attackType}");

        if (attackType == AttackType.Explosive)
        {
            TriggerExplosionShake();
        }
        else
        {
            StartCoroutine(Shake(damageNoise, damageDuration, damageInCurve, damageOutCurve));
        }
    }

    [Button("Trigger Explosion Shake")]
    public void TriggerExplosionShake()
    {
        if (_debug) Debug.Log("Explosion Shake");

        StartCoroutine(Shake(explosionNoise, explosionDuration, explosionInCurve, explosionOutCurve));
    }

    [Button("Trigger Shoot Shake")]
    public void TriggerShootShake()
    {
        if (_debug) Debug.Log("Shoot Shake");

        StartCoroutine(Shake(shootNoise, shootDuration, shootInCurve, shootOutCurve));
    }

    private IEnumerator Shake(NoiseSettings preset, float duration, AnimationCurve inCurve, AnimationCurve outCurve)
    {
        if (noise == null) yield break;

        noise.m_NoiseProfile = preset;

        // Apply fade-in
        float elapsedTime = 0f;
        while (elapsedTime < duration / 2)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / (duration / 2);
            float intensity = inCurve.Evaluate(progress);
            noise.m_AmplitudeGain = intensity * defaultAmplitude;
            yield return null;
        }

        // Apply constant intensity
        float halfDuration = duration / 2;
        elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            noise.m_AmplitudeGain = defaultAmplitude;
            yield return null;
        }

        // Apply fade-out
        elapsedTime = 0f;
        while (elapsedTime < duration / 2)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / (duration / 2);
            float intensity = outCurve.Evaluate(progress);
            noise.m_AmplitudeGain = intensity * defaultAmplitude;
            yield return null;
        }

        // Reset to default
        noise.m_AmplitudeGain = defaultAmplitude;
        noise.m_NoiseProfile = _default;
    }
}
