using System;
using UnityEngine;

namespace PlayerSM
{
    public class ShootingState : IPlayerState
    {
        public static Action OnMiniGameEnded = () => { };

        private PlayerController _player;
        private GunSettings _gun;

        Camera cam;
        RaycastHit hit;
        Ray detectionRay;
        bool isHacking;
        bool isTryingPassword;

        public static event Action<bool> OnBodyState;

        public ShootingState(PlayerController player)
        {
            _player = player;
            _gun = _player.GetComponentInChildren<GunSettings>();
        }

        public void Enter()
        {
            Debug.Log("Shooting State");
            OnBodyState?.Invoke(true);

            isHacking = false;
            cam = Camera.main;

            OnMiniGameEnded += EndHacking;
        }

        public void Exit()
        {
            OnBodyState?.Invoke(false);

            isHacking = false;
            isTryingPassword = false;
            OnMiniGameEnded -= EndHacking;
            QTE_MiniGame.OnPlayerStateChanged?.Invoke();
        }

        public void HandleInput()
        {
            detectionRay = cam.ScreenPointToRay(Input.mousePosition);

            if (Input.GetMouseButtonDown(0) && !isTryingPassword)
            {
                if (Physics.Raycast(detectionRay, out RaycastHit hit))
                {
                    if (_gun.Is2D) hit.point = new Vector3(hit.point.x, _gun.transform.position.y, hit.point.z);

                    //_gun.transform.LookAt(hit.point);
                    _gun.Shoot();
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (isHacking) return;

                if (Physics.Raycast(detectionRay, out hit))
                {
                    if (hit.transform.gameObject.TryGetComponent(out IMiniGame clickedObj) && clickedObj.IsEnemy && !isHacking && !isTryingPassword)
                    {
                        isHacking = true;
                        QTE_MiniGame.OnBarMiniGame?.Invoke(cam, hit.transform, clickedObj);
                    }
                    if (hit.transform.gameObject.TryGetComponent(out IMiniGame clickedObj2) && clickedObj2.IsDoor && !isHacking && !isTryingPassword)
                    {

                        isTryingPassword = true;
                        QTE_MiniGame.OnPasswordMiniGame?.Invoke(clickedObj.Password, clickedObj);
                    }
                }
            }
        }

        public void Update() { }

        void EndHacking()
        {
            isHacking = false;
            isTryingPassword = false;
        }
    }
}
