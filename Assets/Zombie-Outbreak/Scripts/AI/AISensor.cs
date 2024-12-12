using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISensor : MonoBehaviour
{
    //Private
    private AIStateMachine _parentStateMachine = null;
    public AIStateMachine ParentStateMachine { set { _parentStateMachine = value; } }  

    /// <summary>
    /// Se llama cuando un collider entra en el sensor trigger.
    /// Pasa la información de colisión a la máquina de estados de IA principal.
    /// </summary>
    /// <param name="other">El collider que entró en el sensor.</param>
    private void OnTriggerEnter(Collider other)
    {
        if(_parentStateMachine != null)
        {
            _parentStateMachine.OnTriggerEvent(AITriggerEventType.Enter, other);
        }
    }  

    /// <summary>
    /// Se llama cuando un collider se mantiene en el sensor trigger.
    /// Pasa la información de colisión a la máquina de estados de IA principal.
    /// </summary>
    /// <param name="other">El collider que se mantiene en el sensor.</param>
    private void OnTriggerStay(Collider other) 
    {
        if(_parentStateMachine != null)
        {
            _parentStateMachine.OnTriggerEvent(AITriggerEventType.Stay, other);
        }
    }

    /// <summary>
    /// Se llama cuando un collider sale del sensor trigger.
    /// Pasa la información de colisión a la máquina de estados de IA principal.
    /// </summary>
    /// <param name="other">El collider que salió del sensor.</param>
    private void OnTriggerExit(Collider other) 
    {
        if(_parentStateMachine != null)
        {
            _parentStateMachine.OnTriggerEvent(AITriggerEventType.Exit, other);
        }
    }
}
