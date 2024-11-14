namespace PlayerSM
{
    public interface IPlayerState
    {
        void Enter();      // Metodo chiamato quando lo stato viene attivato
        void Exit();       // Metodo chiamato quando lo stato viene disattivato
        void HandleInput(); // Gestione degli input durante lo stato
        void Update();      // Logica di aggiornamento ogni frame
    }
}
