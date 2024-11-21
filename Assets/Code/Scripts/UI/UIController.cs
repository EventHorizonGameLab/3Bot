using TMPro;
using UnityEngine;
using PlayerSM;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using System;

public class UIController : MonoBehaviour
{
    [Title("References")]
    [SerializeField, Required] private Slider _HPBar;
    [SerializeField, Required] private UI_FSM _robot;
    [SerializeField] private Image _slot;
    [SerializeField] private Canvas _menuInGame;

    [Title("Debug")]
    [SerializeField] private bool _debug;
    [SerializeField] private TMP_Text _text;

    [Serializable]
    struct UI_FSM
    {
        public Image head;
        public Image body;
        public Image legs;
    }

    private void Start()
    {
        if (!_debug)
        {
            if(_text != null) _text.text = "";
        }
    }

    private void OnEnable()
    {
        PlayerController.OnChangeState += SetText;
        Health.OnHealthChange += SetHP;
        PauseManager.IsPaused += SetMenuInGame;
        //slot
        //fsm
    }

    private void OnDisable()
    {
        PlayerController.OnChangeState -= SetText;
        Health.OnHealthChange -= SetHP;
        PauseManager.IsPaused -= SetMenuInGame;
        //slot
        //fsm
    }

    private void SetText(string text)
    {
        _text.text = text[9..^5];
    }

    private void SetHP(int hp)
    {
        _HPBar.value = hp / 100f;
    }

    private void SetSlot(Sprite sprite)
    {
        _slot.sprite = sprite;
    }

    private void SetFSM(int fsm)
    {
        switch (fsm)
        {
            case 0:
                _robot.legs.color = Color.yellow;
                _robot.body.color = Color.white;
                _robot.head.color = Color.white;
                break;
            case 1:
                _robot.legs.color = Color.white;
                _robot.body.color = Color.yellow;
                _robot.head.color = Color.white;
                break;
            case 2:
                _robot.legs.color = Color.white;
                _robot.body.color = Color.white;
                _robot.head.color = Color.yellow;
                break;
            default:
                break;
        }
    }

    private void SetMenuInGame(bool state)
    {
        _menuInGame.gameObject.SetActive(state);
    }
}
