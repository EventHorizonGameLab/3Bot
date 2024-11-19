using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerSM
{
    public class HackingState : IPlayerState
    {
        public static Action OnHackingEnded = () => { };

        Camera cam;
        RaycastHit hit;
        Ray ray;
        bool isHacking;
        public void Enter()
        {
            Debug.Log("HackingState");

            isHacking = false;
            cam = Camera.main;
            ray = cam.ScreenPointToRay(Input.mousePosition);

            OnHackingEnded += EndHacking;
        }

        public void Exit()
        {
            isHacking = false;
            OnHackingEnded -= EndHacking;
        }

        public void HandleInput()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (isHacking) return;

                if (Physics.Raycast(ray, out hit) && !isHacking)
                {
                    if (hit.transform.gameObject.TryGetComponent(out IMiniGame enemy) && enemy.IsEnemy)
                    {
                        QTE_MiniGame.OnBarMiniGame?.Invoke(cam, hit.transform, enemy);
                        isHacking = true;
                    }
                }
            }





        }

        public void Update()
        {

        }

        void EndHacking()
        {
            Debug.Log("evento chiamato");
            isHacking = false;
        }
    }
}
