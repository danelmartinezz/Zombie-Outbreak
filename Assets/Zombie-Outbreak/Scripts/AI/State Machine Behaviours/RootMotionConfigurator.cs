using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotionConfigurator : AIStateMachineLink
{
    [SerializeField] private int _rootPosition = 0;
    [SerializeField] private int _rootRotation = 0;

    /// <summary>
    /// Llamado cuando el estado es ingresado.
    /// </summary>
    /// <param name="animator">El animator que contiene el estado.</param>
    /// <param name="stateInfo">Información del estado que se ha ingresado.</param>
    /// <param name="layerIndex">El índice de la capa del estado.</param>
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_stateMachine != null)
        {
            _stateMachine.AddRootionMotionRequest(_rootPosition, _rootRotation);
        }
    }

    /// <summary>
    /// Llamado cuando el estado es abandonado.
    /// </summary>
    /// <param name="animator">El animator que contiene el estado.</param>
    /// <param name="stateInfo">Información del estado que se ha abandonado.</param>
    /// <param name="layerIndex">El índice de la capa del estado.</param>
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_stateMachine != null)
        {
            _stateMachine.AddRootionMotionRequest(-_rootPosition, -_rootRotation);
        }
    }

}
