using System;
using UnityEngine;

namespace PlayerSM
{
    public class ShootingState : IPlayerState
    {
        public static Action OnHackingEnded = () => { };

        private PlayerController _player;
        private GunSettings _gun;

        Camera cam;
        RaycastHit hit;
        Ray detectionRay;
        bool isHacking;

        public ShootingState(PlayerController player)
        {
            _player = player;
            _gun = _player.GetComponentInChildren<GunSettings>();
        }

        public void Enter()
        {
            Debug.Log("Shooting State");

            isHacking = false;
            cam = Camera.main;
            

            OnHackingEnded += EndHacking;
        }

        public void Exit()
        {
            isHacking = false;
            OnHackingEnded -= EndHacking;
        }

        public void HandleInput()
        {
            detectionRay = cam.ScreenPointToRay(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(detectionRay, out RaycastHit hit))
                {
                    if (_gun.Is2D) hit.point = new Vector3(hit.point.x, _gun.transform.position.y, hit.point.z);

                    _gun.transform.LookAt(hit.point);
                    _gun.Shoot();
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                if (isHacking) return;

                if (Physics.Raycast(detectionRay, out hit))
                {
                    if (hit.transform.gameObject.TryGetComponent(out IMiniGame enemy) && enemy.IsEnemy)
                    {
                        QTE_MiniGame.OnBarMiniGame?.Invoke(cam, hit.transform, enemy);
                        isHacking = true;
                    }
                }
            }

            Debug.Log(isHacking);
        }

        public void Update()
        {

        }

        void EndHacking()
        {
            isHacking = false;
        }


    }
}
