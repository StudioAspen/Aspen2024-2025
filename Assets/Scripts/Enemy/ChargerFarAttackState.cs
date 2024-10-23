using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerFarAttackState : MonoBehaviour
{
    [Header("Charging Radius")]
    public float chargingProcRadius;    //radius for entering charge state.
    public float chargingEndRadius;     //radius for within players range to stop tracking.
    
    [Header("Charging Adjustments")]
    public float chargeSpeed;
    public float slowingDownDuration;       //time in seconds for how long the charger moves after tracking stops.
    public float rotationSpeed;

    private Transform player;
    private Vector3 targetPosition;      

    private bool isCharging = false;          
    private bool isSlowingDown = false;       

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        //variable made for reusability.
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        //if distance between charger and player is less than "chargingProcRadius" and is not yet charging or slowing down then:
        if (distanceToPlayer <= chargingProcRadius && !isCharging && !isSlowingDown)
        {
            StartCoroutine(Charge());
        }

        //if distance between charger and player is less than "chargingEndRadius" and is charging, but not yet slowing down then:
        if (distanceToPlayer <= chargingEndRadius && isCharging && !isSlowingDown)
        {
            StopCoroutine(Charge());
            StartCoroutine(SlowingDown());
        }
    }

    private IEnumerator Charge()
    {
        isCharging = true;

        while (isCharging)
        {
            Vector3 direction = (player.position - transform.position).normalized;

            //charging enemy looking towards player while charging.
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            //charging logic.
            transform.position += direction * chargeSpeed * Time.deltaTime;

            //moves each frame.
            yield return null; 
        }
    }

    private IEnumerator SlowingDown()
    {
        isCharging = false;
        isSlowingDown = true;

        Vector3 forwardDirection = transform.forward;
        float timer = 0f;

        //continues moving foward for the specified "slowingDownDuration". 
        while (timer < slowingDownDuration)
        {
            transform.position += forwardDirection * chargeSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        isSlowingDown = false; 
    }

    private void OnDrawGizmosSelected()
    {
        //visual for "chargingProcRadius",
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chargingProcRadius);

        //visual for "chargingEndRadius",
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chargingEndRadius);
    }
}
