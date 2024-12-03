using UnityEngine;
using Cinemachine;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CameraShaker : MonoBehaviour
{
    [Title("Cameras")]
    [SerializeField, Tooltip("Virtual Cameras to apply the shake effect.")]
    private List<CinemachineVirtualCamera> cameras = new();

    [BoxGroup("Default Shake")]
    [SerializeField, Required, Tooltip("Default noise profile for the shake."), NoiseSettingsProperty]
    private NoiseSettings defaultNoise;
    [BoxGroup("Default Shake")]
    [SerializeField, MinValue(0f), Tooltip("Default amplitude for the shake.")]
    private float defaultAmplitude = 1f;

    [BoxGroup("Damage Shake")]
    [SerializeField, Tooltip("Settings for the damage shake effect.")]
    private ShakePreset damageShake;

    [BoxGroup("Explosion Shake")]
    [SerializeField, Tooltip("Settings for the explosion shake effect.")]
    private ShakePreset explosionShake;

    [BoxGroup("Shoot Shake")]
    [SerializeField, Tooltip("Settings for the shoot shake effect.")]
    private ShakePreset shootShake;

    [Title("Debug")]
    [SerializeField, Tooltip("Enable debug logs for shake events.")]
    private bool debug;

    private List<CinemachineBasicMultiChannelPerlin> perlinComponents = new();

    private void Start()
    {
        perlinComponents = cameras
            .Select(cam => cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>())
            .ToList();

        ApplyShakeSettings(defaultNoise, defaultAmplitude);
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

    private void ApplyShakeSettings(NoiseSettings noise, float amplitude)
    {
        foreach (var perlin in perlinComponents)
        {
            perlin.m_NoiseProfile = noise;
            perlin.m_AmplitudeGain = amplitude;
        }
    }

    [Button("Trigger Damage Shake")]
    public void TriggerDamageShake(AttackType attackType = AttackType.Null)
    {
        if (debug) Debug.Log($"Damage Shake triggered by: {attackType}");
        StartShake(attackType == AttackType.Explosive ? explosionShake : damageShake);
    }

    [Button("Trigger Shoot Shake")]
    public void TriggerShootShake()
    {
        if (debug) Debug.Log("Shoot Shake triggered");
        StartShake(shootShake);
    }

    private void StartShake(ShakePreset preset)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(preset));
    }

    private IEnumerator ShakeRoutine(ShakePreset preset)
    {
        ApplyShakeSettings(preset.noiseSettings, 0);

        float elapsedTime = 0f;
        while (elapsedTime < preset.duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / preset.duration);
            float intensity = preset.curve.Evaluate(normalizedTime) + defaultAmplitude;

            foreach (var perlin in perlinComponents)
            {
                perlin.m_AmplitudeGain = intensity;
            }

            yield return null;
        }

        ApplyShakeSettings(defaultNoise, defaultAmplitude);
    }

    [System.Serializable]
    public class ShakePreset
    {
        [HorizontalGroup("Settings", LabelWidth = 100)]
        [SerializeField, Required, NoiseSettingsProperty]
        public NoiseSettings noiseSettings;

        [HorizontalGroup("Settings")]
        [SerializeField, MinValue(0f), Tooltip("Duration of the shake.")]
        public float duration = 0.5f;

        [BoxGroup("Curve", false)]
        [SerializeField, Tooltip("Curve for the shake effect over time.")]
        public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }
}
