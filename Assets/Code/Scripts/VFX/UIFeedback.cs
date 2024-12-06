using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIFeedback : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, MinValue(0)] private float _duration = 0.5f;
    [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Title("References")]
    [SerializeField, Required] private Image _image;

    private void OnEnable()
    {
        Health.OnTakeDamage += Feed;
        Health.OnDeath += KillCoroutine;
        CheckPointManager.OnLoadCheckpoint += KillCoroutine;
    }

    private void OnDisable()
    {
        Health.OnTakeDamage -= Feed;
        Health.OnDeath -= KillCoroutine;
        CheckPointManager.OnLoadCheckpoint -= KillCoroutine;
    }

    private void Feed(AttackType type)
    {
        StartCoroutine(FadeInOut());
    }

    private void KillCoroutine()
    {
        StopAllCoroutines();
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0);
    }

    private IEnumerator FadeInOut()
    {
        float timer = 0f;
        while (timer < _duration)
        {
            timer += Time.deltaTime;
            float alpha = _fadeCurve.Evaluate(timer);
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, alpha);
            yield return null;
        }
    }
}
