using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAgent : MonoBehaviour
{
    public EnemyStateMachine attackStateMachine;
    public EnemyMovementStateMachine movementStateMachine;
    public EnemyStateId initialAttackState;
    public EnemyMovementStateId initialMovementState;
    
    public NavMeshAgent navMeshAgent;

    public Transform enemyTransform;//Reference THIS enemies transform
    public Transform playerTransform; // Reference to the player's transform
    public Renderer enemyRenderer; // Renderer for color changes

    public float enemyDistance; // Distance to player
    public float lungeDetection; // Detection distance for lunging
    public float lungeDistance; // Distance to lunge
    public float lungeDuration; // Duration of the lunge
    public float lungeHeight; // Height of the lunge
    public float hopDistance; // Distance for each hop
    public float hopDuration; // Duration of each hop
    public float hopHeight; // Height of each hop
    public int hopCount; // Number of hops

    public float lungeCooldown; // Cooldown duration for lunge
    public float lungeCooldownTimer; // Timer for lunge cooldown

    public bool isLunging; // Indicates if currently lunging
    public bool canLunge; // Indicates if lunging is allowed

    public Color lungeChanceColorIndicator; // Color for lunging chance indicator
    public Color lungeColorIndicator; // Color for lunging
    public Color followingColorIndicator; // Color for following
    public Color originalColor; // Original color

    public BaseEnemy enemyStats;//THIS enemies stats

    public float initialHoverDistance = 15f;

    public float detectionRadius; // Radius to detect nearby enemies
    public int maxChasingEnemies = 3; // Maximum number of enemies that can chase

    public EnemyDetection enemyDetection;




    void Start()
    {
        ////prep
        enemyTransform = GetComponent<Transform>();
        enemyRenderer = GetComponentInParent<Renderer>();
        navMeshAgent = GetComponentInParent<NavMeshAgent>();
        playerTransform = GameObject.Find("Player").transform;
        canLunge = true;
        enemyStats = GameObject.FindObjectOfType<BaseEnemy>();
        enemyDetection = GameObject.FindObjectOfType<EnemyDetection>();
        originalColor = enemyRenderer.material.color;

        ///Attack state
        attackStateMachine = new EnemyStateMachine(this);

        attackStateMachine.RegistarState(new EnemyDeathState());
        attackStateMachine.RegistarState(new EnemyLeapState());
        attackStateMachine.RegistarState(new EnemyHopState());
        attackStateMachine.RegistarState(new EnemyIdleAttackState());

        attackStateMachine.ChangeState(initialAttackState);


        //Movestate
        movementStateMachine = new EnemyMovementStateMachine(this);

        movementStateMachine.RegisterState(new EnemyChasePlayerState());
        movementStateMachine.RegisterState(new EnemyHitState());
        movementStateMachine.RegisterState(new EnemyHoverPlayerState(initialHoverDistance));

        movementStateMachine.ChangeState(initialMovementState);
    }

   
    void Update()
    {
        attackStateMachine.Update();
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
            if (enemy.movementStateMachine.currentState== EnemyMovementStateId.HoverPlayer)
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
