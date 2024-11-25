using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitch : MonoBehaviour
{
    public static SceneSwitch instance;

    public static Action<string> OnSwitchScene;

    public Image blackScreen;
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(this.gameObject);
    }

    private void OnEnable()
    {
        OnSwitchScene += LoadScene;
    }
    private void OnDisable()
    {
        OnSwitchScene -= LoadScene;
    }

    private void Start()
    {
        blackScreen.color = new Color(0, 0, 0, 0);
    }
        

    public void LoadScene(string sceneName)
    {
        StartCoroutine(TransitionToScene(sceneName));
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        yield return StartCoroutine(FadeIn());
        yield return StartCoroutine(LoadSceneAsync(sceneName));
        yield return StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
       
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        blackScreen.color = new Color(0, 0, 0, 1);
    }

    private IEnumerator FadeOut()
    {
        float timerOut = 0f;
        while (timerOut < fadeDuration)
        {
            timerOut += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, timerOut / fadeDuration);
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        blackScreen.color = new Color(0, 0, 0, 0);
        
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
