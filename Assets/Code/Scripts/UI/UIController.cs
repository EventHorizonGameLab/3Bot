using TMPro;
using UnityEngine;
using PlayerSM;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using System;

public class UIController : MonoBehaviour
{
    [TabGroup("", "Health", SdfIconType.FullscreenExit, TextColor = "green")]
    [SerializeField, Required] private Slider _HPBar;

    [TabGroup("", "Player State Machine", SdfIconType.Robot, TextColor = "yellow")]
    [SerializeField, Required] private UI_FSM _robot;
    [TabGroup("", "Player State Machine")]
    [SerializeField] private Color _colorRobots = Color.yellow;

    [TabGroup("", "Magnet Gun", SdfIconType.BroadcastPin, TextColor = "red")]
    [SerializeField, Required] private Image _slot;

    [TabGroup("", "EMP Gun", SdfIconType.LightningCharge, TextColor = "blue")]
    [SerializeField, Required] private TMP_Text _currentAmmo;
    [TabGroup("", "EMP Gun")]
    [SerializeField, Required] private TMP_Text _ammoInStorage;
    [TabGroup("", "EMP Gun")]
    [SerializeField, Required] private Image _reloadLed;
    [TabGroup("", "EMP Gun")]
    [SerializeField, Tooltip("Color of the LED")] private Color _color = Color.white;
    [TabGroup("", "EMP Gun")]
    [SerializeField, Required] private Slider _coolDown;

    [TabGroup("", "Menu In Game", SdfIconType.Columns, TextColor = "orange")]
    [SerializeField, Required] private Canvas _menuInGame;

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

    private float _maxhealth = 0f;

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
        GunSettings.OnCoolDown += SetCoolDown;

        UISlot.OnSpriteChanged += SetSlot;
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
        GunSettings.OnCoolDown -= SetCoolDown;

        UISlot.OnSpriteChanged -= SetSlot;
    }

    private void SetText(string text)
    {
        _text.text = text[9..^5];
    }

    private void SetHP(int hp)
    {
        if (_maxhealth == 0f) _maxhealth = hp;
        _HPBar.value = hp / _maxhealth;
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

    private void SetCoolDown(float value)
    {
        _coolDown.value = value;
    }
}
