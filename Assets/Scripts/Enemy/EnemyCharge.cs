using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCharge : MonoBehaviour
{
    public Transform playerTransform;
    NavMeshAgent agent;

    public BaseEnemy enemyStats;
    public Vector3 enemyDistance;
    public Vector3 height;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.Find("Player").transform;
        enemyStats = GameObject.FindObjectOfType<BaseEnemy>();
    }

    void Update()
    {

        agent.speed = enemyStats.enemyMoveSpeed * 2;
        agent.destination = playerTransform.position;
        enemyDistance = playerTransform.position - transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy") 
        {
            Debug.Log("THROW");
         /*
            collision.rigidbody.AddForce(playerTransform.position - transform.position + height  * 3000);       */
        }
    }
}
