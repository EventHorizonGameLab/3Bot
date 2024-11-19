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
        Ray ray;
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
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Lancia il Raycast e controlla se colpisce un punto valido
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (_gun.Is2D) hit.point = new Vector3(hit.point.x, _gun.transform.position.y, hit.point.z);

                    _gun.transform.LookAt(hit.point);
                    _gun.Shoot();
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                if (isHacking) return;

                if (Physics.Raycast(ray, out hit) )
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
