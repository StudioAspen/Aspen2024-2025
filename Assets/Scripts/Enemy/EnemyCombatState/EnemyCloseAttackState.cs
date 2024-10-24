using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCloseAttackState : EnemyState
{
    private int consecutiveHooks;
    private int maxHooks;
    private bool isAttacking;
    EnemyStateId EnemyState.GetId()
    {
        return EnemyStateId.Attack;
    }
    public void Enter(EnemyAgent agent)
    {
        consecutiveHooks = 0;
        isAttacking = true;
    }

    public void Update(EnemyAgent agent)
    {
        if (isAttacking)
        {
            if (agent.enemyDetection) // Check if player is within attack range
            {
                //Attacks
            }
            else
            {
                agent.movementStateMachine.ChangeState(EnemyMovementStateId.Idle); //Idle if player is out of range
            }

            if (consecutiveHooks >= maxHooks)
            {
                agent.movementStateMachine.ChangeState(EnemyMovementStateId.Idle); //Idle after completing attacks
            }
        }
    }
    
    public void Exit(EnemyAgent agent) 
    {
        isAttacking = false;
    }
    
}
