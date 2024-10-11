using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCharge : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float acceleration = 5f;
    public float chargeDistanceThreshold = 10f;
    public float chargeDuration = 2f; // Duration of the charge
    public float knockbackDistance = 5f;
    public float stunDuration = 2f;

    private NavMeshAgent navAgent;
    private Rigidbody rb;
    private Vector3 playerPosition;
    private Vector3 previousPlayerPosition; // Store the previous player position
    private bool isCharging = false;
    private float currentSpeed = 0f;
    private bool isStunned = false;

    private GameObject Playerobj;

    void Start()
    {
        Playerobj = GameObject.Find("Player");
        navAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isStunned)
        {
            UpdatePlayerPosition();

            if (!isCharging && Vector3.Distance(transform.position, playerPosition) < chargeDistanceThreshold)
            {
                previousPlayerPosition = playerPosition; // Store the previous position
                StartCharging();
            }
            else if (isCharging)
            {
                ChargeTowardsPreviousPosition();
            }
            else
            {
                FollowPlayer();
            }
        }
    }

    void StartCharging()
    {
        isCharging = true;
        currentSpeed = 0f; // Reset speed at start of charge
        navAgent.enabled = false; // Disable NavMeshAgent
        StartCoroutine(ChargeCoroutine()); // Start charging coroutine
    }

    void ChargeTowardsPreviousPosition()
    {
        currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
        Vector3 direction = (previousPlayerPosition - transform.position).normalized;

        // Move the enemy manually
        rb.MovePosition(transform.position + direction * currentSpeed * Time.deltaTime);
    }

    void FollowPlayer()
    {
        navAgent.SetDestination(playerPosition); // Use NavMeshAgent to follow
    }

    IEnumerator ChargeCoroutine()
    {
        yield return new WaitForSeconds(chargeDuration);

        // Stop charging after the duration
        isCharging = false;
        navAgent.enabled = true; // Re-enable NavMeshAgent
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Stop any movement toward the player
            isCharging = false;
            currentSpeed = 0f; // Reset speed immediately

            KnockBack(collision); // Pass the collision object
        }
        else if (collision.gameObject.CompareTag("Enemy") && isCharging)
        {
            ThrowEnemy(collision.gameObject.GetComponent<Rigidbody>());
        }
    }

    void KnockBack(Collision collision)
    {
        Vector3 knockbackDirection = -collision.contacts[0].normal; // Ensure this is correct
        Debug.DrawLine(transform.position, transform.position + knockbackDirection * 2, Color.red, 1f);

        float hopDistance = 5f; // Adjust as needed
        float upwardForce = 3f; // Adjust for hop height

        Vector3 force = knockbackDirection.normalized * hopDistance + Vector3.up * upwardForce;
        rb.AddForce(force, ForceMode.Impulse);

        StartCoroutine(StunCoroutine());
    }

    IEnumerator StunCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        isStunned = true;
        isCharging = false; // Reset charging state
        navAgent.enabled = false; // Disable NavMeshAgent during stun
        yield return new WaitForSeconds(stunDuration); // Wait for stun duration
        isStunned = false; // Reset stunned state
        navAgent.enabled = true; // Re-enable NavMeshAgent after the stun
    }

    void UpdatePlayerPosition()
    {
        playerPosition = Playerobj.transform.position;
    }

    void ThrowEnemy(Rigidbody otherEnemyRb)
    {
        Vector3 force = (playerPosition - transform.position).normalized * currentSpeed;
        otherEnemyRb.AddForce(force, ForceMode.Impulse);
    }
    
}
