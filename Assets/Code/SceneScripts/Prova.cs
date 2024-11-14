using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Prova : MonoBehaviour
{
    public void SwitchScene()
    {
        SceneSwitch.OnSwitchScene?.Invoke("Scene2");
        
    }
}
