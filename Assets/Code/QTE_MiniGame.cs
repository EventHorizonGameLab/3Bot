using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QTE_MiniGame : MonoBehaviour
{
    public static Action<Camera, Transform, IMiniGame> OnBarMiniGame;
    public static Action<string, IMiniGame> OnPasswordMiniGame;

    [SerializeField] GameObject barHolder;
    [SerializeField] Image fillBar;
    [SerializeField] float fillDuration = 5f;
    [SerializeField] GameObject pswHolder;
    [Range(0f, 1f)] [SerializeField] float colorChangeThreshold = 0.8f;

    IMiniGame miniGameObject;
    private float fillTime = 0f;
    private bool isIncreasing = true;
    private bool isGreen = false;
    

    private void Awake()
    {
        barHolder.SetActive(false);
    }

    void OnEnable()
    {
        OnBarMiniGame += StartBarMiniGame;
    }

        

    private void OnDisable()
    {
        OnBarMiniGame -= StartBarMiniGame;
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

    private void Update() // TODO: eliminare ed integrare nella State Machine
    {
        if (Input.GetMouseButtonDown(1) && barHolder.activeInHierarchy)
        {
            if (isGreen)
            {
                Debug.Log("QTE VINTO");
                //TODO: play del suono di vittoria
                barHolder.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("QTE PERSO");
                // TODO:player del suono di fallimento
            }

        }

    }

    void StartBarMiniGame(Camera cam, Transform target, IMiniGame enemy)
    {
        var screenPosition = cam.WorldToScreenPoint(target.position);
        barHolder.transform.position = screenPosition;
        barHolder.gameObject.SetActive(true);

        fillBar.fillAmount = 0f;
        fillBar.color = Color.red;
        fillTime = 0f;
        isIncreasing = true;
        isGreen = false;
    }

    void StartPasswordMiniGame(string password, IMiniGame door)
    {

    }

    bool CheckPassword()
    {
        return false; // controllare psw
    }

    void OpenDoor()
    {
        if(CheckPassword()) { miniGameObject.MinigameWon(); }
    }

    void ChangeNumberOnButtonClick()
    {

    }

    
}
