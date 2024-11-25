using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (_loseCanvas != null) _loseCanvas.SetActive(true);
        OnLose?.Invoke();
    }

    public void LoadScene(string sceneName)
    {
        SceneSwitch.instance.LoadScene(sceneName);
    }

    public void ReloadScene()
    {
        SceneSwitch.instance.ReLoadScene(SceneManager.GetActiveScene().name);
    }
}
