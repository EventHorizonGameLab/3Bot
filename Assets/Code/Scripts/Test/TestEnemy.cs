using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestEnemy : MonoBehaviour, IMiniGame
{
    [SerializeField] bool isEnemy;
    [SerializeField] bool isDoor;
    [SerializeField]
    [ShowIf("isDoor")]
    [InfoBox("Inserire 3 numeri. Se la psw sono meno 3 numeri, rimpiazzare i primi con 0")]
    string password;
    public bool IsEnemy { get { return isEnemy; } }

    public bool IsDoor { get { return isDoor; } }

    public string Password { get { return password; } set { password = value; } }

    public void MinigameWon()
    {
       gameObject.SetActive(false);
    }
}
