using PlayerSM;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QTE_MiniGame : MonoBehaviour
{
    public static Action<Camera, Transform, IMiniGame> OnBarMiniGame;
    public static Action<string, IMiniGame> OnPasswordMiniGame;
    public static Action OnPlayerStateChanged;

    [SerializeField] GameObject barHolder;
    [SerializeField] Image fillBar;
    [SerializeField] float fillDuration = 5f;
    [SerializeField] GameObject pswHolder;
    [SerializeField] TMP_Text[] passwordField;
    [Range(0f, 1f)][SerializeField] float colorChangeThreshold = 0.8f;

    IMiniGame miniGameObject;
    string currentCombination = string.Empty;
    private float fillTime = 0f;
    private bool isIncreasing = true;
    private bool isGreen = false;


    private void Awake()
    {
        barHolder.SetActive(false);
        pswHolder.SetActive(false);
    }

    private void Start()
    {
        ResetPasswordField();
    }


    void OnEnable()
    {
        OnBarMiniGame += StartBarMiniGame;
        OnPasswordMiniGame += StartPasswordMiniGame;
        OnPlayerStateChanged += ForceCloseMiniGames;
    }



    private void OnDisable()
    {
        OnBarMiniGame -= StartBarMiniGame;
        OnPasswordMiniGame -= StartPasswordMiniGame;
        OnPlayerStateChanged -= ForceCloseMiniGames;
    }

    void FixedUpdate()
    {
        if (isIncreasing)
        {
            fillTime += Time.fixedDeltaTime;
            if (fillTime >= fillDuration)
            {
                fillTime = fillDuration;
                isIncreasing = false;
            }
        }
        else
        {
            fillTime -= Time.fixedDeltaTime;
            if (fillTime <= 0f)
            {
                fillTime = 0f;
                isIncreasing = true;
            }
        }

        float fillAmount = fillTime / fillDuration;
        fillBar.fillAmount = fillAmount;

        if (fillAmount >= colorChangeThreshold && !isGreen)
        {
            fillBar.color = Color.green;
            isGreen = true;
        }
        else if (fillAmount < colorChangeThreshold && isGreen)
        {
            fillBar.color = Color.red;
            isGreen = false;
        }

    }

    private void Update()
    {
        if (miniGameObject == null) { Awake(); Start(); }
        if (Input.GetMouseButtonDown(1) && barHolder.activeInHierarchy)
        {
            if (isGreen)
            {
                Debug.Log("QTE VINTO");
                //TODO: play del suono di vittoria
                barHolder.SetActive(false);
                miniGameObject.MinigameWon();
                miniGameObject = null;
                ShootingState.OnMiniGameEnded?.Invoke();
            }
            else
            {
                Debug.Log("QTE PERSO");
                // TODO:play del suono di fallimento
            }

        }

    }

    void StartBarMiniGame(Camera cam, Transform target, IMiniGame enemy)
    {
        miniGameObject ??= enemy;
        var screenPosition = cam.WorldToScreenPoint(target.position);
        barHolder.transform.position = screenPosition;
        barHolder.SetActive(true);

        fillBar.fillAmount = 0f;
        fillBar.color = Color.red;
        fillTime = 0f;
        isIncreasing = true;
        isGreen = false;
    }

    void StartPasswordMiniGame(string password, IMiniGame door)
    {
        miniGameObject ??= door;
        ResetPasswordField();
        pswHolder.SetActive(true);
    }

    bool CheckPassword()
    {
        if (currentCombination == miniGameObject.Password) return true;
        return false;
    }


    void TryOpenDoor()
    {
        if (CheckPassword())
        {
            miniGameObject.MinigameWon();
            miniGameObject = null;
            pswHolder.SetActive(false);
            ShootingState.OnMiniGameEnded?.Invoke();
        }
    }


    public void ChangeNumberOnButtonClick(TMP_Text numberOnButton)
    {
        int currentNumber = int.Parse(numberOnButton.text);
        currentNumber = (currentNumber + 1) % 10;
        numberOnButton.text = currentNumber.ToString();
        currentCombination = string.Empty;
        foreach (TMP_Text number in passwordField)
        {
            currentCombination += number.text;
        }
        TryOpenDoor();

    }

    void ResetPasswordField()
    {
        foreach (TMP_Text text in passwordField)
        {
            text.text = "0";
        }
    }

    public void ClosePasswordPanel()
    {
        miniGameObject = null;
        pswHolder.SetActive(false);
        ShootingState.OnMiniGameEnded?.Invoke();
    }

    void ForceCloseMiniGames()
    {
        miniGameObject = null;
        pswHolder.SetActive(false);
        barHolder.SetActive(false);
    }


}
