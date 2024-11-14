using TMPro;
using UnityEngine;
using PlayerSM;

public class UIController : MonoBehaviour
{
    [SerializeField] TMP_Text _text;

    private void OnEnable()
    {
        PlayerController.OnChangeState += SetText;
    }

    private void OnDisable()
    {
        PlayerController.OnChangeState -= SetText;
    }

    public void SetText(string text)
    {
        _text.text = text[9..^5];
    }
}
