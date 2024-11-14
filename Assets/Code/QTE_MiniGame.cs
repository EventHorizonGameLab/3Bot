using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QTE_MiniGame : MonoBehaviour
{
    public static Action<Camera, Transform> OnMiniGame;

    [SerializeField] GameObject holder;
    [SerializeField] Image fillBar;
    [SerializeField] float fillDuration = 5f;
    [Range(0f, 1f)] public float colorChangeThreshold = 0.8f;


    private float fillTime = 0f;
    private bool isIncreasing = true;
    private bool isGreen = false;

    private void Awake()
    {
        holder.SetActive(false);
    }

    void OnEnable()
    {
        OnMiniGame += StartMinigame;
    }

        

    private void OnDisable()
    {
        OnMiniGame -= StartMinigame;
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
        if (Input.GetMouseButtonDown(1) && holder.activeInHierarchy)
        {
            if (isGreen)
            {
                Debug.Log("QTE VINTO");
                holder.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("QTE PERSO");
            }
        }

    }

    void StartMinigame(Camera cam, Transform target)
    {
        var screenPosition = cam.WorldToScreenPoint(target.position);
        holder.transform.position = screenPosition;
        holder.gameObject.SetActive(true);

        fillBar.fillAmount = 0f;
        fillBar.color = Color.red;
        fillTime = 0f;
        isIncreasing = true;
        isGreen = false;
    }

    
}
