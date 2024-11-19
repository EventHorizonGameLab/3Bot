using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class FullScreenController : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Toggle fullScreenToggle;

    private void Awake()
    {
        fullScreenToggle.isOn = PlayerPrefs.GetInt("FullScreen", 1) == 1;
    }

    private void OnEnable()
    {
        fullScreenToggle.onValueChanged.AddListener(SetFullScreen);
    }

    private void OnDestroy()
    {
        fullScreenToggle.onValueChanged.RemoveListener(SetFullScreen);
    }

    private void SetFullScreen(bool value)
    {
        PlayerPrefs.SetInt("FullScreen", fullScreenToggle.isOn ? 1 : 0);
        Screen.SetResolution(Screen.width, Screen.height, value);
    }
}