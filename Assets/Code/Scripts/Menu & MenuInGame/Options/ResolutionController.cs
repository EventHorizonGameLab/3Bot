using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class ResolutionController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Dropdown resolutionDropdown = null;

    private Resolution[] resolutions;
    private List<string> options = new();

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    private void OnDisable()
    {
        resolutionDropdown.onValueChanged.RemoveListener(SetResolution);
    }

    private void SetResolution(int value)
    {
        // Check if the selected value is within valid range
        if (value < 0 || value >= options.Count)
        {
            Debug.LogWarning("Invalid resolution index: " + value);
            return;
        }

        // Apply selected resolution to the screen
        Resolution resolution = new()
        {
            width = int.Parse(options[value].Split('x')[0]),
            height = int.Parse(options[value].Split('x')[1])
        };
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        // Save selected resolution to PlayerPrefs
        PlayerPrefs.SetInt("ResolutionWidth", resolution.width);
        PlayerPrefs.SetInt("ResolutionHeight", resolution.height);
    }

    public void Initialize()
    {
        // Load saved resolution or use current screen resolution as default
        int screenWidth = PlayerPrefs.GetInt("ResolutionWidth", Screen.currentResolution.width);
        int screenHeight = PlayerPrefs.GetInt("ResolutionHeight", Screen.currentResolution.height);
        bool fullScreen = Screen.fullScreen;

        // Set the screen resolution
        Screen.SetResolution(screenWidth, screenHeight, fullScreen);

        // Get all resolutions supported by the system
        resolutions = Screen.resolutions;

        // Filter and remove duplicate resolutions and only keep 16:9 resolutions
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            // Check if the resolution is 16:9 aspect ratio
            if (!Is16By9Resolution(resolutions[i]))
            {
                continue;
            }

            string option = resolutions[i].width + "x" + resolutions[i].height;

            // Skip duplicate resolutions
            if (options.Contains(option))
            {
                continue;
            }

            options.Add(option);

            // Find the index of the current resolution
            if (resolutions[i].width == screenWidth && resolutions[i].height == screenHeight)
            {
                currentResolutionIndex = i;
            }
        }

        // Update dropdown options
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex; // Set current resolution
        resolutionDropdown.RefreshShownValue(); // Update displayed value in dropdown
    }

    private bool Is16By9Resolution(Resolution resolution)
    {
        // Check if the resolution has a 16:9 aspect ratio
        float aspectRatio = (float)resolution.width / resolution.height;
        return Mathf.Approximately(aspectRatio, 16f / 9f);
    }
}
