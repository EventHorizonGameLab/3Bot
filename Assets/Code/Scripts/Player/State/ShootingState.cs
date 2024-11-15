using UnityEngine;

namespace PlayerSM
{
    public class ShootingState : IPlayerState
    {
        private PlayerController _player;
        private GunSettings _gun;

        public ShootingState(PlayerController player)
        {
            _player = player;
            _gun = _player.GetComponentInChildren<GunSettings>();
        }

        public void Enter()
        {
            Debug.Log("Shooting State");

        }

        public void Exit() { }

        public void HandleInput()
        {

        }

        public void Update()
        {
            Shoot1();
        }

        private void Shoot1()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Lancia il Raycast e controlla se colpisce un punto valido
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if(_gun.Is2D) hit.point = new Vector3(hit.point.x, _gun.transform.position.y, hit.point.z);

                    _gun.transform.LookAt(hit.point);
                    _gun.Shoot();
                }
            }
        }

        private void Hack() //to be implemented
        {
            Debug.Log("Hacking...");
        }
    }
}
