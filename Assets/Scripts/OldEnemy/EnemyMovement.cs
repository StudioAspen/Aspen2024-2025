using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Transform playerTransform;
    NavMeshAgent agent;

    public BaseEnemy enemyStats;

   

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.Find("Player").transform;
        enemyStats = GameObject.FindObjectOfType<BaseEnemy>();
    }

    void Update()
    {
        agent.speed = enemyStats.enemyMoveSpeed;
        agent.destination = playerTransform.position;
    }
}
