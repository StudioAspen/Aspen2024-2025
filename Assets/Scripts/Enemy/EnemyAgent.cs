using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAgent : MonoBehaviour
{
    public EnemyStateMachine stateMachine;
    public EnemyStateId initialState;
    public NavMeshAgent navMeshAgent;
    

    void Start()
    {
        navMeshAgent = GetComponentInParent<NavMeshAgent>();
        stateMachine = new EnemyStateMachine(this);

        stateMachine.RegistarState(new EnemyChasePlayerState());
        stateMachine.RegistarState(new EnemyDeathState());

        stateMachine.ChangeState(initialState);
    }

   
    void Update()
    {
        stateMachine.Update();
    }
}
