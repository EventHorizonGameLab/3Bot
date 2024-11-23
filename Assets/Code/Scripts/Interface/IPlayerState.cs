namespace PlayerSM
{
    public interface IPlayerState
    {
        void Enter();
        void Exit();
        void HandleInput();
        void Update();
        void Reset();
    }
}
