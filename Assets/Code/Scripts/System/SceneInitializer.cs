using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent, RequireComponent(typeof(SceneInitializer))]
public class SceneInitializer : MonoBehaviour
{
    [Header("Scene Manager")]
    [SerializeField] private List<string> sceneNames = new();

    private void Awake()
    {
        LoadScene();
    }

    private void LoadScene()
    {
        foreach (var sceneName in sceneNames)
        {
            if (!IsSceneLoaded(sceneName))
            {
                SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
        }
    }

    private bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName)
            {
                return true;
            }
        }
        return false;
    }
}
