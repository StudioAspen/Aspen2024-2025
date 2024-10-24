using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementStateMachine
{
    public EnemyMovementState[] states;
    public EnemyAgent agent;
    public EnemyMovementStateId currentState;


    public EnemyMovementStateMachine(EnemyAgent agent)
    {
        this.agent = agent;
        int numStates = System.Enum.GetNames(typeof(EnemyMovementStateId)).Length;
        states = new EnemyMovementState[numStates];
    }

    public void RegisterState(EnemyMovementState state)
    {
        int index = (int)state.GetId();
        states[index] = state;
    }

    public EnemyMovementState GetState(EnemyMovementStateId stateId)
    {
        int index = (int)stateId;
        return states[index];
    }

    public void Update()
    {
        GetState(currentState)?.Update(agent);
    }

    public void ChangeState(EnemyMovementStateId newState)
    {
        GetState(currentState)?.Exit(agent);
        currentState = newState;
        GetState(currentState)?.Enter(agent);
    }
   
}
