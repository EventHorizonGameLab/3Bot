using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIInputVisualizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Image _upArrow;
    [SerializeField] Image _downArrow;

    [SerializeField] Image _leftMouse;
    [SerializeField] Image _rightMouse;

    [Header("Settings")]
    [SerializeField] private Color _upColor = Color.white;
    [SerializeField] private Color _downColor = Color.grey;

    [SerializeField] private float colorDuration = 0.5f; // Durata del cambiamento di colore in secondi

    private void Update()
    {
        CheckKeyInput(KeyCode.UpArrow, _upArrow);
        CheckKeyInput(KeyCode.DownArrow, _downArrow);

        CheckMouseInput(0, _leftMouse);
        CheckMouseInput(1, _rightMouse);
    }

    private void CheckMouseInput(int mouseButton, Image image)
    {
        if (Input.GetMouseButtonDown(mouseButton))
        {
            StopAllCoroutines(); // Ferma qualsiasi coroutine in corso per evitare conflitti
            StartCoroutine(ChangeColorTemporarily(image));
        }
    }

    private void CheckKeyInput(KeyCode key, Image image)
    {
        if (Input.GetKeyDown(key))
        {
            StopAllCoroutines(); // Ferma qualsiasi coroutine in corso per evitare conflitti
            StartCoroutine(ChangeColorTemporarily(image));
        }
    }

    private IEnumerator ChangeColorTemporarily(Image image)
    {
        // Cambia colore al colore premuto
        image.color = _downColor;

        // Aspetta il tempo impostato
        yield return new WaitForSeconds(colorDuration);

        // Ritorna al colore originale
        image.color = _upColor;
    }
}
