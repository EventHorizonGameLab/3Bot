using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class MixerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioMixer mixer;
    [Space]
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    bool isSetted = false;

    private void Start()
    {
        SetSliderVolumes();
    }

    private void OnEnable()
    {
        masterSlider.onValueChanged.AddListener(SaveSliderVolumes);
        musicSlider.onValueChanged.AddListener(SaveSliderVolumes);
        sfxSlider.onValueChanged.AddListener(SaveSliderVolumes);
    }

    private void OnDisable()
    {
        masterSlider.onValueChanged.RemoveListener(SaveSliderVolumes);
        musicSlider.onValueChanged.RemoveListener(SaveSliderVolumes);
        sfxSlider.onValueChanged.RemoveListener(SaveSliderVolumes);
    }

    void SetMixerVolumes()
    {
        mixer.SetFloat("Master", Mathf.Lerp(-80f, 20f, PlayerPrefs.GetFloat(masterSlider.name, 0.75f)));
        mixer.SetFloat("Music", Mathf.Lerp(-80f, 20f, PlayerPrefs.GetFloat(musicSlider.name, 0.75f)));
        mixer.SetFloat("Sfx", Mathf.Lerp(-80f, 20f, PlayerPrefs.GetFloat(sfxSlider.name, 0.75f)));
    }

    public void SetSliderVolumes()
    {
        masterSlider.value = PlayerPrefs.GetFloat(masterSlider.name, 0.75f);
        musicSlider.value = PlayerPrefs.GetFloat(musicSlider.name, 0.75f);
        sfxSlider.value = PlayerPrefs.GetFloat(sfxSlider.name, 0.75f);

        isSetted = true;

        SetMixerVolumes();
    }

    public void SaveSliderVolumes(float value)
    {
        if (!isSetted) return;

        PlayerPrefs.SetFloat(masterSlider.name, masterSlider.value);
        PlayerPrefs.SetFloat(musicSlider.name, musicSlider.value);
        PlayerPrefs.SetFloat(sfxSlider.name, sfxSlider.value);

        SetMixerVolumes();
    }
}
