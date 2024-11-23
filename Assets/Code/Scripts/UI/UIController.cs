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
    [SerializeField] private Color _colorRobots = Color.yellow;
    [SerializeField, Required] private Image _slot;
    [SerializeField, Required] private Canvas _menuInGame;
    [SerializeField, Required] private TMP_Text _currentAmmo;
    [SerializeField, Required] private TMP_Text _ammoInStorage;
    [SerializeField, Required] private Image _reloadLed;
    [SerializeField] private Color _color = Color.white;

    [Title("Debug")]
    [SerializeField] private bool _debug;
    [ShowIf("_debug"), SerializeField, Required] private TMP_Text _text;

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
            if (_text != null) _text.text = "";
        }
    }

    private void OnEnable()
    {
        PlayerController.OnChangeStateDebug += SetText;
        Health.OnHealthChange += SetHP;
        PauseManager.IsPaused += SetMenuInGame;
        PlayerController.OnChangeState += SetFSM;
        GunSettings.OnMagazineChanged += SetAmmoInStorage;
        GunSettings.OnAmmoChanged += SetCurrentAmmo;
        GunSettings.HasInfiniteAmmo += SetAmmoInStorageActive;
        GunSettings.OnReload += SetReloadingLed;
        //Slot
    }

    private void OnDisable()
    {
        PlayerController.OnChangeStateDebug -= SetText;
        Health.OnHealthChange -= SetHP;
        PauseManager.IsPaused -= SetMenuInGame;
        PlayerController.OnChangeState -= SetFSM;
        GunSettings.OnMagazineChanged -= SetAmmoInStorage;
        GunSettings.OnAmmoChanged -= SetCurrentAmmo;
        GunSettings.HasInfiniteAmmo -= SetAmmoInStorageActive;
        GunSettings.OnReload -= SetReloadingLed;
        //Slot
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
                _robot.legs.color = _colorRobots;
                _robot.body.color = Color.white;
                _robot.head.color = Color.white;
                break;
            case 1:
                _robot.legs.color = Color.white;
                _robot.body.color = _colorRobots;
                _robot.head.color = Color.white;
                break;
            case 2:
                _robot.legs.color = Color.white;
                _robot.body.color = Color.white;
                _robot.head.color = _colorRobots;
                break;
            default:
                break;
        }
    }

    private void SetMenuInGame(bool state)
    {
        _menuInGame.gameObject.SetActive(state);
    }

    private void SetCurrentAmmo(int value)
    {
        _currentAmmo.text = value.ToString();
    }

    private void SetAmmoInStorage(int value)
    {
        _ammoInStorage.text = value.ToString();
    }

    private void SetAmmoInStorageActive(bool state)
    {
        _ammoInStorage.gameObject.SetActive(state);
    }

    private void SetReloadingLed(bool state)
    {
        _reloadLed.color = state ? _color : Color.white;
    }
}
