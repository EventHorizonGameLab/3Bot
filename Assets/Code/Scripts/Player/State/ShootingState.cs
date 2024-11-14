using UnityEngine;


namespace PlayerSM
{
    public class ShootingState : IPlayerState
    {
        private PlayerController _player;
        private ParticleSystem _gun1;

        public ShootingState(PlayerController player)
        {
            _player = player;
            _gun1 = _player.GetComponentInChildren<ParticleSystem>();
        }

        public void Enter()
        {
            Debug.Log("Shooting State");

        }

        public void Exit() { }

        public void HandleInput()
        {
            if (Input.GetKey(KeyCode.W)) // Esempio di input
            {
                Debug.Log("Moving Forward");
            }
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
                    // Imposta la destinazione dell'agent al punto di impatto
                    _gun1.transform.LookAt(hit.point);
                    _gun1.Emit(1);
                }
            }
        }

        private void Shoot2()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Lancia il Raycast e controlla se colpisce un punto valido
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // Imposta la destinazione dell'agent al punto di impatto
                    
                }
            }
        }
    }
}
