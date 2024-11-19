using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class SpeakerMode : MonoBehaviour
{
    [Title("References")]
    [SerializeField, Required] private TMP_Dropdown speakerModeDropdown = null;

    [Title("Debug")]
    [SerializeField, PropertyOrder(2)] private bool _debug = false;

    private List<string> options = new();

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        speakerModeDropdown.onValueChanged.AddListener(SetSpeakerMode);
    }

    private void OnDisable()
    {
        speakerModeDropdown.onValueChanged.RemoveListener(SetSpeakerMode);
    }

    private void SetSpeakerMode(int value)
    {
        if (value < 0 || value >= options.Count)
        {
            if (_debug) Debug.LogWarning("Invalid index: " + value);
            return;
        }

        // Save the selected value
        PlayerPrefs.SetInt("SpeakerMode", value);

        // Apply the selected speaker mode
        AudioConfiguration config = AudioSettings.GetConfiguration();  // Get the current audio configuration
        AudioSpeakerMode selectedMode = (AudioSpeakerMode)value;

        // If the selected mode is different, update the configuration
        if (config.speakerMode != selectedMode)
        {
            config.speakerMode = selectedMode;  // Set new speaker mode
            AudioSettings.Reset(config);  // Apply the new configuration
        }

        if (_debug) Debug.Log("Speaker Mode Set to: " + selectedMode);
    }

    public void Initialize()
    {
        // Load saved speaker mode or use Stereo as the default
        int savedSpeakerMode = PlayerPrefs.GetInt("SpeakerMode", (int)AudioSpeakerMode.Stereo);

        options.Clear();
        foreach (AudioSpeakerMode mode in System.Enum.GetValues(typeof(AudioSpeakerMode)))
        {
            options.Add(mode.ToString());
        }

        // Update dropdown options
        speakerModeDropdown.ClearOptions();
        speakerModeDropdown.AddOptions(options);

        // Set the current selected value from PlayerPrefs
        speakerModeDropdown.value = options.FindIndex(x => x == ((AudioSpeakerMode)savedSpeakerMode).ToString());
        speakerModeDropdown.RefreshShownValue();  // Update displayed value

        // Apply the initial speaker mode based on the saved value
        AudioConfiguration config = AudioSettings.GetConfiguration();  // Get the current audio configuration
        AudioSpeakerMode initialMode = (AudioSpeakerMode)savedSpeakerMode;

        if (config.speakerMode != initialMode)
        {
            config.speakerMode = initialMode;  // Set the saved speaker mode
            AudioSettings.Reset(config);  // Apply the new configuration
        }
    }
}
