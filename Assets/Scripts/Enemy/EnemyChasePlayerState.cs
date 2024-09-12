using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChasePlayerState : EnemyState
{

    public Transform playerTransform;
    public BaseEnemy enemyStats;

    void EnemyState.Enter(EnemyAgent agent)
    {
     
        playerTransform = GameObject.Find("Player").transform;
        enemyStats = GameObject.FindObjectOfType<BaseEnemy>();
    }

    void EnemyState.Exit(EnemyAgent agent)
    {
        
    }

    EnemyStateId EnemyState.GetId()
    {
        return EnemyStateId.ChasePlayer;
    }

    void EnemyState.Update(EnemyAgent agent)
    {
        agent.navMeshAgent.speed = enemyStats.enemyMoveSpeed;
        agent.navMeshAgent.destination = playerTransform.position;
    }
}
