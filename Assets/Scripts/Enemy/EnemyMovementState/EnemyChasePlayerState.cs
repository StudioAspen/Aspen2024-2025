using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChasePlayerState : EnemyMovementState
{
    public EnemyMovementStateId GetId() => EnemyMovementStateId.ChasePlayer;

    void EnemyMovementState.Enter(EnemyAgent agent)
    {
        // Set the agent's speed or any setup logic
        agent.navMeshAgent.speed = agent.enemyStats.enemyMoveSpeed;

    }

    void EnemyMovementState.Exit(EnemyAgent agent)
    {
        // Reset any parameters or cleanup
        //agent.navMeshAgent.isStopped = true; // Stop the agent when exiting
    }

    void EnemyMovementState.Update(EnemyAgent agent)
    {
        agent.navMeshAgent.destination = agent.playerTransform.position;
    }
  

}
/* if (figure out what needs to go in here so make it lungers only)
      {
           // Update cooldown timer
          if (agent.lungeCooldownTimer > 0)
          {
              agent.lungeCooldownTimer -= Time.deltaTime;
          }

          agent.enemyDistance = Vector3.Distance(agent.transform.position, agent.playerTransform.position);

          if (agent.enemyDistance <= agent.lungeDetection && !agent.isLunging && agent.canLunge && agent.lungeCooldownTimer <= 0)
          {
              // Start hopping
              agent.stateMachine.ChangeState(EnemyStateId.HopBack);
          }
          else
          {
              // Logic to follow the player
              agent.enemyRenderer.material.color = agent.followingColorIndicator;
          }
      }*/