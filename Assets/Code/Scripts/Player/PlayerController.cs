using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerSM
{
    public class PlayerController : MonoBehaviour
    {
        [Title("Settings")]
        [SerializeField] private KeyCode _keyToChangeStateUp = KeyCode.UpArrow;
        [SerializeField] private KeyCode _keyToChangeStateDown = KeyCode.DownArrow;

        [Title("Debug")]
        [SerializeField] private bool _debug;

        [Button]
        public void Reset()
        {
            _states[0].Reset();
        }

        private IPlayerState _currentState;
        private List<IPlayerState> _states;
        private int _currentStateIndex;

        public static event Action<string> OnChangeStateDebug;
        public static event Action<int> OnChangeState;

        private bool _isEnable = true;

        private void Start()
        {
            _states = new List<IPlayerState>
            {
                new MovementState2(this),
                new ShootingState(this),
                new HeadState(this)
            };

            _currentStateIndex = 0;
            SwitchState(_states[_currentStateIndex]);
        }

        private void OnEnable()
        {
            PauseManager.IsPaused += IsEnable;
        }

        private void OnDisable()
        {
            PauseManager.IsPaused -= IsEnable;
        }

        private void IsEnable(bool value)
        {
            _isEnable = !value;
        }

        private void Update()
        {
            if (!_isEnable) return;

            if (Input.GetKeyDown(_keyToChangeStateUp))
            {
                SwitchState(GetNextState());
            }
            if (Input.GetKeyDown(_keyToChangeStateDown))
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

        public int CurrentState
        {
            get => _currentStateIndex;
            set
            {
                _currentStateIndex = value;
                SwitchState(_states[value]);
                _states[0].Reset();
            }
        }
    }
}
