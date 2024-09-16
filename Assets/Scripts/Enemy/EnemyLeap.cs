using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyLeap : MonoBehaviour
{
    public Transform playerTransform;
    NavMeshAgent agent;

    public BaseEnemy enemyStats;
    public float enemyDistance;



    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.Find("Player").transform;
        enemyStats = GameObject.FindObjectOfType<BaseEnemy>();
    }

    void Update()
    {
      
        agent.stoppingDistance = 3;
        agent.speed = enemyStats.enemyMoveSpeed;
        agent.destination = playerTransform.position;
    }
}
