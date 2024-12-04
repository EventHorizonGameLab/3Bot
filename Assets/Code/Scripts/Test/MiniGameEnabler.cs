using Sirenix.OdinInspector;
using UnityEngine;

public class MiniGameEnabler : MonoBehaviour, IMiniGame
{
    [SerializeField, HideIf("isDoor")] bool isEnemy;
    [SerializeField, HideIf("isEnemy")] bool isDoor;
    [SerializeField]
    [ShowIf("isDoor")]
    [InfoBox("Inserire 3 numeri. Se la psw sono meno 3 numeri, rimpiazzare i primi con 0")]
    string password = "000";
    [SerializeField]
    
    [InfoBox("Inserire l'oggetto con animator solo se ne hai bisogno")]
    Animator doorAnimator;

    public bool IsEnemy { get { return isEnemy; } }
    public bool IsDoor { get { return isDoor; } }
    public string Password { get { return password; } set { password = value; } }

    public void MinigameWon()
    {
        // Enemy Behavior
        if (isEnemy && doorAnimator != null)
        {
            gameObject.SetActive(false);
            doorAnimator.SetTrigger("DoorOpen");
        }
        else { gameObject.SetActive(false); }

        // Door Behavior
        if (isDoor && doorAnimator != null)
        {
            doorAnimator.SetTrigger("DoorOpen");
            gameObject.SetActive(false);
        }
    }

    void OnValidate()
    {
        if (isEnemy) isDoor = false;
        else if (isDoor) isEnemy = false;
    }
}
