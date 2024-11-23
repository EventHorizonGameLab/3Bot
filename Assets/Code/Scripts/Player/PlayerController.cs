using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerSM
{
    public class PlayerController : MonoBehaviour
    {
        [Title("Debug")]
        [SerializeField] private bool _debug;

        private IPlayerState _currentState;
        private List<IPlayerState> _states;
        private int _currentStateIndex;

        public static event Action<string> OnChangeStateDebug;
        public static event Action<int> OnChangeState;

        private void Start()
        {
            // Inizializza gli stati e parte dallo stato di movimento
            _states = new List<IPlayerState>
            {
                new MovementState2(this),
                new ShootingState(this),
                new HeadState(this)
            };

            _currentStateIndex = 0;
            SwitchState(_states[_currentStateIndex]);
        }

        private void Update()
        {
            // Gestisce l'input per passare tra gli stati
            if (Input.GetKeyDown(KeyCode.UpArrow)) // Tasto freccia destra
            {
                SwitchState(GetNextState());
            }
            if (Input.GetKeyDown(KeyCode.DownArrow)) // Tasto freccia sinistra
            {
                SwitchState(GetPreviousState());
            }

            _currentState.Update();
            _currentState.HandleInput();
        }

        private void SwitchState(IPlayerState newState)
        {
            if (_debug) OnChangeStateDebug?.Invoke(newState.ToString());
            OnChangeState?.Invoke(_currentStateIndex);

            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }

        private IPlayerState GetNextState()
        {
            _currentStateIndex = (_currentStateIndex + 1) % _states.Count;
            return _states[_currentStateIndex];
        }

        private IPlayerState GetPreviousState()
        {
            _currentStateIndex = (_currentStateIndex - 1 + _states.Count) % _states.Count;
            return _states[_currentStateIndex];
        }

        private void SetState(int index)
        {
            _states[0].Reset();
            _currentStateIndex = index;
            SwitchState(_states[index]);
        }

        public int CurrentState
        {
            get => _currentStateIndex;
            set => SetState(value);
        }
    }
}
