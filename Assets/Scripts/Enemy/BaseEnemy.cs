using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{

    [Header("Enemy Stats")]
    [SerializeField] public int enemyMaxHP;
    [SerializeField] public int enemyCurrentHP;
    [SerializeField] public int enemyLevel;
    [SerializeField] public float enemyMoveSpeed;

    [SerializeField] public EnemyAgent agent;
    public WorldManager levelManager;
    void Start()
    {
        enemyMaxHP = 100;   
        enemyMoveSpeed = 2;
        enemyLevel = 1;
        enemyCurrentHP = enemyMaxHP;

        agent = GameObject.FindObjectOfType<EnemyAgent>();


        levelManager = GameObject.FindObjectOfType<WorldManager>();
    }


    void Update()
    {
        if (enemyCurrentHP <= 0) 
        {

            EnemyDeathState enemyDead = agent.attackStateMachine.GetState(EnemyStateId.Death) as EnemyDeathState;
            agent.attackStateMachine.ChangeState(EnemyStateId.Death);
            Destroy(gameObject);
        }
    }
}
