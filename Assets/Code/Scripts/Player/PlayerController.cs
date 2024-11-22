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

        public static event Action<string> OnChangeState;

        private void Start()
        {
            // Inizializza gli stati e parte dallo stato di movimento
            _states = new List<IPlayerState>
        {
            new MovementState(this),
            new MovementState2(this),
            new ShootingState(this),
            new HeadState(this)
        };

            _currentStateIndex = 0; // Imposta lo stato iniziale (MovementState)
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

            // Esegui aggiornamenti dello stato attivo
            _currentState.Update();
            _currentState.HandleInput();
        }

        private void SwitchState(IPlayerState newState)
        {
            OnChangeState?.Invoke(newState.ToString());

            _currentState?.Exit(); // Esci dallo stato precedente
            _currentState = newState; // Passa al nuovo stato
            _currentState.Enter();    // Entra nel nuovo stato
        }

        private IPlayerState GetNextState()
        {
            _currentStateIndex = (_currentStateIndex + 1) % _states.Count; // Torna all'inizio quando arrivi alla fine
            return _states[_currentStateIndex];
        }

        private IPlayerState GetPreviousState()
        {
            _currentStateIndex = (_currentStateIndex - 1 + _states.Count) % _states.Count; // Torna all'ultimo stato se si va indietro
            return _states[_currentStateIndex];
        }
    }
}
