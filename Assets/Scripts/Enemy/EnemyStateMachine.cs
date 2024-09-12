using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine
{
    //array of states 
    public EnemyState[] states;
    public EnemyAgent agent;
    public EnemyStateId currentState;

    public EnemyStateMachine(EnemyAgent agent) 
    {
        //take  a single parameer
        this.agent = agent;
        //number of states to allocate for 
        int numstates = System.Enum.GetNames(typeof(EnemyStateId)).Length;
        //make new array
        states = new EnemyState[numstates];
    }

    public void RegistarState(EnemyState state) 
    {
        //sets it(the number of posssible states that would be loaded in) at the start of the game rather than runtime 
        int index = (int)state.GetId();
        states[index] = state;

    }

    public EnemyState GetState(EnemyStateId stateId) 
    {
        int index = (int)stateId;
        return states[index];   
    }
    public void Update() 
    {
        GetState(currentState)?.Update(agent);
    }

    public void ChangeState(EnemyStateId newState) 
    {
        GetState(currentState)?.Exit(agent);
        currentState = newState;
        GetState(currentState)?.Enter(agent);
    }
}
