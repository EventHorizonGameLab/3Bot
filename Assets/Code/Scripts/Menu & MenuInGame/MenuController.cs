using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class MenuController : MonoBehaviour
{
    public delegate void MenuEvent();
    public static event MenuEvent Resume;

    #region Menu Buttons

    public void ExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ResumeButton()
    {
        Resume?.Invoke();
    }

    #endregion
}