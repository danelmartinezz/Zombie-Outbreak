using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIStateType { None, Idle, Alert, Patrol, Attack, Feeding, Pursuit, Dead }

public abstract class AIStateMachine : MonoBehaviour
{
    // Private
    private Dictionary<AIStateType, AIState> _states = new Dictionary<AIStateType, AIState>();

    protected virtual void Start()
    {
        // Obtener todos los estados de este gameObject.
        AIState[] states = GetComponents<AIState>();

        // Recorremos todos los estados y lo añadimos al diccionario de estados.
        foreach (AIState state in states)
        {
            if (state != null && !_states.ContainsKey(state.GetStateType()))
            {
                // Añadimos este estado al diccionario de estado.
                _states[state.GetStateType()] = state;
            }
        }
    }
}
