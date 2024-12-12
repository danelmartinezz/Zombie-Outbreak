using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateMachineLink : StateMachineBehaviour
{
    protected AIStateMachine _stateMachine;
    public AIStateMachine StateMachine { set { _stateMachine = value; } }
}
