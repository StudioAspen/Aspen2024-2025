using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine<EnumState> : MonoBehaviour where EnumState : Enum
{
    protected Dictionary<EnumState, BaseState<EnumState>> States = new Dictionary<EnumState, BaseState<EnumState>>();

    protected BaseState<EnumState> CurrentState;

    private void Start()
    {
        CurrentState.EnterState();
    }

    private void Update()
    {
        EnumState nextStateKey = CurrentState.GetNextState();

        if (nextStateKey.Equals(CurrentState.StateKey))
        {
            CurrentState.UpdateState();
        }
        else
        {
            TransitionToNextState(nextStateKey);
        }
    }

    private void TransitionToNextState(EnumState stateKey)
    {
        CurrentState.ExitState();
        CurrentState = States[stateKey];
        CurrentState.EnterState();
    }

    private void OnTriggerEnter(Collider other)
    {
        CurrentState.OnTriggerEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CurrentState.OnTriggerStay(other);
    }

    private void OnTriggerExit(Collider other)
    {
        CurrentState.OnTriggerExit(other);
    }
}
