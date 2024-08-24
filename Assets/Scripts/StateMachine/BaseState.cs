using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState<EnumState> where EnumState : Enum
{
    public BaseState(EnumState key)
    {
        StateKey = key;
    }

    public EnumState StateKey { get; private set; }

    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void UpdateState();
    public abstract EnumState GetNextState();
    public abstract void OnTriggerEnter(Collider other);
    public abstract void OnTriggerStay(Collider other);
    public abstract void OnTriggerExit(Collider other);
}
