using PlayerSM;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField] private KeyCode _keyToChangeStateUp = KeyCode.UpArrow;
    [SerializeField] private KeyCode _keyToChangeStateDown = KeyCode.DownArrow;

    [Title("Debug")]
    [SerializeField] private bool _debug;

    public static event Action<string> OnChangeStateDebug;
    public static event Action<int> OnChangeState;
    public static event Action<int, bool> OnStateStatusChange;

    private IPlayerState _currentState;
    private Dictionary<IPlayerState, bool> _stateStatus;
    private List<IPlayerState> _stateOrder;
    private int _currentStateIndex;

    private bool _isEnable = true;

    private void Start()
    {
        _stateOrder = new List<IPlayerState>
        {
            new MovementState2(this),
            new ShootingState(this),
            new HeadState(this)
        };

        _stateStatus = new Dictionary<IPlayerState, bool>();
        foreach (var state in _stateOrder)
        {
            _stateStatus[state] = true;
        }

        _currentStateIndex = 0;
        SwitchState(_stateOrder[_currentStateIndex]);
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
        for (int i = 1; i <= _stateOrder.Count; i++)
        {
            int nextIndex = (_currentStateIndex + i) % _stateOrder.Count;
            if (_stateStatus[_stateOrder[nextIndex]])
            {
                _currentStateIndex = nextIndex;
                return _stateOrder[nextIndex];
            }
        }
        return _stateOrder[_currentStateIndex];
    }

    private IPlayerState GetPreviousState()
    {
        for (int i = 1; i <= _stateOrder.Count; i++)
        {
            int prevIndex = (_currentStateIndex - i + _stateOrder.Count) % _stateOrder.Count;
            if (_stateStatus[_stateOrder[prevIndex]])
            {
                _currentStateIndex = prevIndex;
                return _stateOrder[prevIndex];
            }
        }
        return _stateOrder[_currentStateIndex];
    }

    public int CurrentState
    {
        get => _currentStateIndex;
        set
        {
            if (_stateStatus[_stateOrder[value]])
            {
                _currentStateIndex = value;
                SwitchState(_stateOrder[value]);
                _stateOrder[0].Reset();
            }
        }
    }

    public Dictionary<IPlayerState, bool> GetStateStatus()
    {
        return new Dictionary<IPlayerState, bool>(_stateStatus);
    }

    public void SetStateStatus(Dictionary<IPlayerState, bool> newStateStatus)
    {
        foreach (var state in _stateOrder)
        {
            if (newStateStatus.ContainsKey(state))
            {
                _stateStatus[state] = newStateStatus[state];
            }
        }

        if (!_stateStatus[_currentState])
        {
            SwitchState(GetNextState());
        }

        StateStatusChange();
    }

    public int StateStatus
    {
        get => 0;
        set
        {
            if (value < 0 || value >= _stateOrder.Count) return;

            IPlayerState state = _stateOrder[value];

            _stateStatus[state] = !_stateStatus[state];

            if (!_stateStatus[_currentState])
            {
                SwitchState(GetNextState());
            }

            StateStatusChange();
        }
    }

    private void StateStatusChange()
    {
        foreach (var state in _stateOrder)
        {
            OnStateStatusChange?.Invoke(_stateOrder.IndexOf(state), _stateStatus[state]);
        }
    }
}
