using Sirenix.OdinInspector;
using System.Collections;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [Title("Refernces")]
    [SerializeField, Required] private TMP_Text _timer;

    [Title("Settings")]
    [SerializeField] private bool _startOnAwake = true;

    private float _time = 0f;

    private void Start()
    {
        _timer.text = "00 : 00";

        if (_startOnAwake)
        {
            StartCoroutine(TimerCount());
        }
    }

    public void StartTimer()
    {
        StartCoroutine(TimerCount());
    }

    public void ResetTimer()
    {
        _time = 0f;
    }

    public void StopTimer()
    {
        StopCoroutine(TimerCount());
    }

    public void ReduceTimer(float time)
    {
        _time -= time;
    }

    IEnumerator TimerCount()
    {
        while (true)
        {
            _time += Time.deltaTime;
            _timer.text = $"{((int)_time / 60):00} : {((int)_time % 60):00}";
            yield return null;
        }
    }

    public float TimeValue
    {
        get { return _time; }
        set { _time = value; }
    }
}
