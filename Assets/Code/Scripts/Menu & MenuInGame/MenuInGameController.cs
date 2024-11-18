using UnityEngine;

[DisallowMultipleComponent]
public class MenuInGameController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Canvas menuInGameCanvas;

    private void OnEnable()
    {
        PauseManager.IsPaused += MenuEnable;
        MenuController.Resume += MenuDisable;
    }

    private void OnDisable()
    {
        PauseManager.IsPaused -= MenuEnable;
        MenuController.Resume -= MenuDisable;
    }

    private void MenuEnable(bool value)
    {
        if (!menuInGameCanvas)
        {
            Debug.LogWarning("MenuInGameCanvas not assigned");
            return;
        }

        menuInGameCanvas.gameObject.SetActive(value);
    }

    private void MenuDisable()
    {
        menuInGameCanvas.gameObject.SetActive(false);
    }
}
