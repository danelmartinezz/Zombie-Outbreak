using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState : MonoBehaviour
{
    protected AIStateMachine _stateMachine;

    // Abstract Methods    
    /// <summary>
    /// Obtiene el tipo de estado asociado con este estado de IA.
    /// </summary>
    /// <returns>El tipo de estado asociado con este estado de IA.</returns>
    public abstract AIStateType GetStateType();
    
    /// <summary>
    /// Actualiza el estado actual y devuelve el tipo de estado que debe ser establecido en el próximo frame.
    /// </summary>
    /// <returns>El tipo de estado que debe ser establecido en el próximo frame.</returns>
    public abstract AIStateType OnUpdate();

    /// <summary>
    /// Establece la máquina de estados asociada con este estado de IA.
    /// </summary>
    /// <param name="stateMachine">La maquina de estado.</param>
     //
    public void SetStateMachine(AIStateMachine stateMachine) => _stateMachine = stateMachine;

    /// <summary>
    /// Llamado cuando el estado es ingresado.
    /// </summary>
    public virtual void OnEnterState() {}
    /// <summary>
    /// Llamado cuando el estado es abandonado.
    /// </summary>
    public virtual void OnExitState() {}    
    /// <summary>
    /// Llamado cada frame, antes de que el motor de animación actualice el estado de la animación.
    /// </summary>
    public virtual void OnAnimatorUpdated() {}
    /// <summary>
    /// 
    /// </summary>
    public virtual void OnAnimatorIKUpdated() {}
    /// <summary>
    /// Llamado cuando un trigger colisiona con el personaje.
    /// </summary>
    /// <param name="eventType">El tipo de evento del trigger.</param>
    /// <param name="other">El objeto que se encuentra en el trigger.</param>
    public virtual void OnTriggerEvent(AITriggerEventType eventType, Collider other) { }
    /// <summary>
    /// Llamado cuando se alcanza el destino.
    /// </summary>
    /// <param name="isReached">Indica si el destino ha sido alcanzado.</param>
    public virtual void OnDestinationReached(bool isReached) {}
}
