using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyHitState : EnemyMovementState
{
    public EnemyMovementStateId GetId() => EnemyMovementStateId.Hit;

    public float RecoverTimeMax;
    public float RecoverTimer;
    public void Enter(EnemyAgent agent)
    {
        agent.navMeshAgent.enabled = false;
        RecoverTimeMax = 0.5f;
        RecoverTimer = RecoverTimeMax;
    }

    public void Exit(EnemyAgent agent)
    {

    }



    public void Update(EnemyAgent agent)
    {
        RecoverTimer -= Time.deltaTime;

        if (RecoverTimer < 0) 
        {
            agent.navMeshAgent.enabled = true;
            agent.movementStateMachine.ChangeState(EnemyMovementStateId.ChasePlayer);
        }
    }
  
}
