using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class InvertY : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Toggle toggle;

    private CinemachineFreeLook freeLook;

    private void Awake()
    {
        toggle.isOn = PlayerPrefs.GetInt("InvertY", 1) == 1;
    }

    private void Start()
    {
        freeLook = FindAnyObjectByType<CinemachineFreeLook>();
        if (freeLook) freeLook.m_YAxis.m_InvertInput = toggle.isOn;
    }

    private void OnEnable()
    {
        toggle.onValueChanged.AddListener(Invert);
    }

    private void OnDisable()
    {
        toggle.onValueChanged.RemoveListener(Invert);
    }

    private void Invert(bool value)
    {
        PlayerPrefs.SetInt("InvertY", value ? 1 : 0);
        if (freeLook) freeLook.m_YAxis.m_InvertInput = value;
    }
}
