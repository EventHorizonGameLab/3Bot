using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTest : MonoBehaviour
{

    Camera cam;
    RaycastHit hit;
    Ray ray;

    private void Awake()
    {
        cam = Camera.main;
        ray = cam.ScreenPointToRay(Input.mousePosition);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {


            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.TryGetComponent(out IMiniGame enemy) && enemy.IsEnemy)
                {
                    QTE_MiniGame.OnBarMiniGame?.Invoke(cam, hit.transform, enemy);
                }

            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.TryGetComponent(out IMiniGame door) && door.IsDoor)
                {
                    QTE_MiniGame.OnPasswordMiniGame?.Invoke(door.Password, door);
                }

            }
        }
    }



}
