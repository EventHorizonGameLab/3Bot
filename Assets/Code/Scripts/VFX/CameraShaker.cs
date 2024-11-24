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
    [SerializeField, Tooltip("Curve for the entire shake effect")] private AnimationCurve damageCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [BoxGroup("Explosion Shake", false)]
    [SerializeField, Required, NoiseSettingsProperty] private NoiseSettings explosionNoise;
    [BoxGroup("Explosion Shake", false)]
    [SerializeField, MinValue(0f)] private float explosionDuration = 0.7f;
    [BoxGroup("Explosion Shake", false)]
    [SerializeField, Tooltip("Curve for the entire shake effect")] private AnimationCurve explosionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [BoxGroup("Shoot Shake", false)]
    [SerializeField, Required, NoiseSettingsProperty] private NoiseSettings shootNoise;
    [BoxGroup("Shoot Shake", false)]
    [SerializeField, MinValue(0f)] private float shootDuration = 0.2f;
    [BoxGroup("Shoot Shake", false)]
    [SerializeField, Tooltip("Curve for the entire shake effect")] private AnimationCurve shootCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

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
    public void TriggerDamageShake(AttackType attackType = AttackType.Null)
    {
        if (_debug) Debug.Log($"Damage Shake: {attackType}");

        if (attackType == AttackType.Explosive)
        {
            TriggerExplosionShake();
        }
        else
        {
            StartCoroutine(Shake(damageNoise, damageDuration, damageCurve));
        }
    }

    [Button("Trigger Explosion Shake")]
    public void TriggerExplosionShake()
    {
        if (_debug) Debug.Log("Explosion Shake");

        StartCoroutine(Shake(explosionNoise, explosionDuration, explosionCurve));
    }

    [Button("Trigger Shoot Shake")]
    public void TriggerShootShake()
    {
        if (_debug) Debug.Log("Shoot Shake");

        StartCoroutine(Shake(shootNoise, shootDuration, shootCurve));
    }

    private IEnumerator Shake(NoiseSettings preset, float duration, AnimationCurve shakeCurve)
    {
        if (noise == null) yield break;

        noise.m_NoiseProfile = preset;

        float elapsedTime = 0f;

        // Applicazione dell'effetto di shake seguendo la curva unica
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / duration); // Progresso normalizzato [0, 1]
            float intensity = shakeCurve.Evaluate(progress) + defaultAmplitude; // Calcolo intensità in base alla curva e defaultAmplitude
            noise.m_AmplitudeGain = intensity;
            yield return null;
        }

        // Ritorno ai valori predefiniti
        noise.m_AmplitudeGain = defaultAmplitude;
        noise.m_NoiseProfile = _default;
    }
}
