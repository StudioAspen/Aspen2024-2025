using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChargeEnemyAgent : MonoBehaviour
{
    public EnemyStateMachine attackStateMachine;
    public EnemyMovementStateMachine movementStateMachine;
    public EnemyStateId initialChargeAttackState;
    public EnemyMovementStateId initialMovementState;

    public NavMeshAgent navMeshAgent;

    public Transform enemyTransform;
    public Transform playerTransform;
    public Renderer enemyRenderer;

    public float enemyDistance;
    /// <summary>
    /// Enemy Agent has variables for leaper
    /// instead should be variables for the charger
    /// this can be combined with the regular EnemyAgent later
    /// </summary>
    public float chargeDetection;
    public float chargeDistance;
    public float chargeDuration;

    public BaseEnemy enemyStats;

    public float initialHoverDistance;
    public float detectionRadius;

    public int maxChasingEnemies = 3;


    
    // Start is called before the first frame update
    void Start()
    {   
        // prep for charger targetting and navigation
        enemyTransform = GetComponent<Transform>();
        enemyRenderer = GetComponentInParent<Renderer>();
        navMeshAgent = GetComponentInParent<NavMeshAgent>();
        playerTransform = GameObject.Find("Player").transform;

        /// Attack State


        // Move State
        //movementStateMachine = new EnemyMovementStateMachine(this);

        movementStateMachine.RegisterState(new EnemyChasePlayerState());
        movementStateMachine.RegisterState(new EnemyHitState());
        movementStateMachine.RegisterState(new EnemyHoverPlayerState(initialHoverDistance));

        movementStateMachine.ChangeState(initialMovementState);


    }

    // Update is called once per frame
    void Update()
    {
        // attack state machine update here
        movementStateMachine.Update();
    }

    public void UpdateEnemyStates(List<EnemyAgent> nearbyEnemies)
    {
        // Sort by distance to the player
        nearbyEnemies.Sort((a, b) => Vector3.Distance(a.transform.position, transform.position).CompareTo(Vector3.Distance(b.transform.position, transform.position)));

        int chasingCount = 0;
        foreach (var enemy in nearbyEnemies)
        {
            float distanceToPlayer = Vector3.Distance(enemy.transform.position, transform.position);

            if (chasingCount < maxChasingEnemies)
            {
                if (distanceToPlayer < enemy.enemyDistance) // Check if they should chase
                {
                    enemy.movementStateMachine.ChangeState(EnemyMovementStateId.ChasePlayer);
                    chasingCount++;
                }
            }
            else
            {
                enemy.movementStateMachine.ChangeState(new EnemyHoverPlayerState(enemy.enemyDistance).GetId());
            }
        }

        // Allow hovering enemies to chase if below max count
        foreach (var enemy in nearbyEnemies)
        {
            if (enemy.movementStateMachine.currentState == EnemyMovementStateId.HoverPlayer)
            {
                float distanceToPlayer = Vector3.Distance(enemy.transform.position, transform.position);
                if (distanceToPlayer < enemy.enemyDistance) // If they are close enough
                {
                    enemy.movementStateMachine.ChangeState(EnemyMovementStateId.ChasePlayer);
                }
            }
        }
    }

}
