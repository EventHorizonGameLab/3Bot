using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class BrightnessController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider brightnessSlider;

    private ColorAdjustments colorAdjustments;

    private void Start()
    {
        Volume volume = GameObject.Find("Global Volume").GetComponent<Volume>();

        if (!volume.profile.TryGet(out colorAdjustments)) return;

        Initialize();
    }

    private void OnEnable()
    {
        brightnessSlider.onValueChanged.AddListener(SetBrightness);
    }

    private void OnDisable()
    {
        brightnessSlider.onValueChanged.RemoveListener(SetBrightness);
    }

    private void SetBrightness(float value)
    {
        PlayerPrefs.SetFloat("Brightness", value);

        int _brightnessLevel = (int)Mathf.Lerp(50, 255, value);

        Color newColor = new(_brightnessLevel / 255f, _brightnessLevel / 255f, _brightnessLevel / 255f, 1);
        colorAdjustments.colorFilter.Override(newColor);
    }

    public void Initialize()
    {
        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 0.75f);
        SetBrightness(brightnessSlider.value);
    }
}