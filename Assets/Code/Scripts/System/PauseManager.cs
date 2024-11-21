using Sirenix.OdinInspector;
using System;
using UnityEngine;

[DisallowMultipleComponent]
public class PauseManager : MonoBehaviour
{
    [Title("Setting")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] private bool _isGameInPaused = false;

    [Title("Flags")]
    [SerializeField] private bool _isCursorHiddenAndLocked = false;

    [Title("Debug")]
    public bool _debug = false;

    public static event Action<bool> IsPaused;

    private void Awake()
    {
        Time.timeScale = 1;

        if (_debug) return;

        SetCursorState(_isCursorHiddenAndLocked && !_debug);
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey)) PauseGame();
    }

    private void OnEnable()
    {
        MenuController.Resume += PauseGame;
    }

    private void OnDisable()
    {
        MenuController.Resume -= PauseGame;
    }

    /// <summary>
    /// Pause the game
    /// </summary>
    public void PauseGame()
    {
        if(_debug) Debug.Log($"Pause Game: {_isGameInPaused}");

        _isGameInPaused = !_isGameInPaused;
        Time.timeScale = _isGameInPaused ? 0 : 1;

        SetCursorState(_isCursorHiddenAndLocked && !_debug);

        IsPaused?.Invoke(_isGameInPaused);
    }

    private void SetCursorState(bool isHiddenAndLocked)
    {
        Cursor.lockState = isHiddenAndLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isHiddenAndLocked;
    }
}
