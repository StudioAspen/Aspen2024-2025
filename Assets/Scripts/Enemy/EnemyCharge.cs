using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCharge : MonoBehaviour
{
    public Transform playerTransform;
    NavMeshAgent agent;
    public Rigidbody rb;

    public BaseEnemy enemyStats;
    public float enemyDistance;
    public Vector3 height;

    public float maxChargeSpeed;
//public float chargeSpeed;
    public float chargeAcceleration;
    public float chargeDuration;
    public float stunDuration;
    public float knockbackForce;
    public bool isCharging;
    public float chargeTime;
    public Vector3 chargeDirection;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.Find("Player").transform;
        enemyStats = GameObject.FindObjectOfType<BaseEnemy>();
    }

    void Update()
    {
       
        agent.speed = enemyStats.enemyMoveSpeed ;
        agent.destination = playerTransform.position;
        
   
        if (Vector3.Distance(transform.position, playerTransform.position) < enemyDistance && !isCharging) 
        {
            StartCharge();
        }

        if (isCharging) 
        {
            ChargeToPlayer();
        }

    }

    public void StartCharge() 
    {
        isCharging = true;
        chargeTime = 0f;
        agent.speed = 0f;

        chargeDirection = (playerTransform.position - transform.position).normalized;
    }
    public void ChargeToPlayer() 
    {
        if (!isCharging) return;

    
            enemyStats.enemyMoveSpeed += chargeAcceleration * Time.deltaTime;
            enemyStats.enemyMoveSpeed = Mathf.Clamp(enemyStats.enemyMoveSpeed, 0, maxChargeSpeed);

           
            rb.velocity = chargeDirection * enemyStats.enemyMoveSpeed;

        
        if(chargeTime >= chargeDuration)
        {
            isCharging = false;
            rb.velocity = Vector3.zero;
        }
    }

    public IEnumerator WallStun() 
    {
        yield return new WaitForSeconds(stunDuration);
    
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall")) 
        {
            isCharging = false;
            rb.velocity = Vector3.zero;
            
            Vector3 knockbackDirection = -collision.contacts[0].normal;
            
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode.VelocityChange);

            rb.isKinematic = true;

            StartCoroutine(WallStun());
        }
    }
}
