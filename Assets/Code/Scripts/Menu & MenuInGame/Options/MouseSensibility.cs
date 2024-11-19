using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class MouseSensibility : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider _slider;

    public static float sensibility = 0.5f;

    private void Awake()
    {
        _slider.value = PlayerPrefs.GetFloat("Sensibility", sensibility);
        sensibility = _slider.value;
    }

    private void OnEnable()
    {
        _slider.onValueChanged.AddListener(SetSensibility);
    }

    private void OnDisable()
    {
        _slider.onValueChanged.RemoveListener(SetSensibility);
    }

    private void SetSensibility(float value)
    {
        PlayerPrefs.SetFloat("Sensibility", value);
        sensibility = value;
    }
}
