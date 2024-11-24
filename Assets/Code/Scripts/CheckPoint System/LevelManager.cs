using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Title("References")]
    [SerializeField] private GameObject _loseCanvas;

    [Title("Debug")]
    [SerializeField] private bool _debug;

    public static event Action OnLose;

    void Start() { }

    private void OnEnable()
    {
        Health.OnDeath += LoseCondition;
    }

    private void OnDisable()
    {
        Health.OnDeath += LoseCondition;
    }

    public void WinCondition()
    {
        if (_debug) Debug.Log("You won!");
    }

    public void LoseCondition()
    {
        if (_debug) Debug.Log("You lost!");
        _loseCanvas.SetActive(true);
        OnLose?.Invoke();
    }
}
