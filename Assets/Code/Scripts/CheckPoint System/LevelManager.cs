using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Title("References")]
    [SerializeField] private GameObject _loseCanvas;

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
        Debug.Log("You won!");
    }

    public void LoseCondition()
    {
        Debug.Log("You lost!");
        _loseCanvas.SetActive(true);
        OnLose?.Invoke();
    }
}
