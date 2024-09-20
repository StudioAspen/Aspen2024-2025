using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyLeap : MonoBehaviour
{
    public Transform playerTransform;
    public Rigidbody rb;
    NavMeshAgent agent;

    public BaseEnemy enemyStats;
    public float enemyDistance;
    public float lungeDetection;

    public float lungeDisatance;
    public float lungeSpeed;
    public bool isLunging;

    public float pastedTime;
    public float lungeDuration;
    public float lungeHeight;

    public int hopCount;
    public float hopDistance;
    public float hopDuration;
    public float hopHeight;

    public float lungeCooldown;
    public bool canLunge;

    public Renderer enemyRenderer;
    public Color lungeChanceColorIndicator;
    public Color lungeColorIndicator;
    public Color followingColorIndicator;
    public Color originalColor;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.Find("Player").transform;
        enemyStats = GameObject.FindObjectOfType<BaseEnemy>();
        canLunge = true;
        rb = GetComponent<Rigidbody>();

        enemyRenderer = GetComponent<Renderer>();
        originalColor = enemyRenderer.material.color;
    }

    void Update()
    {    
        agent.stoppingDistance = 0;
        agent.speed = enemyStats.enemyMoveSpeed;

        if (isLunging == false) 
        {
           agent.destination = playerTransform.position;
           rb.MovePosition(agent.destination);
           enemyRenderer.material.color = followingColorIndicator;
        }

        enemyDistance = Vector3.Distance(transform.position, playerTransform.position);

        if (enemyDistance <= lungeDetection && !isLunging && canLunge) 
        {
            StartCoroutine(LeapColorIndicator());

            if (Random.value > 0.5f) 
            {
                StartCoroutine(Lunge());
            }
  
        }
    }

    public IEnumerator Lunge() 
    {
        isLunging = true;
        canLunge = false;

        Vector3 startPosition = transform.position;   
       
        ////THE WINDUP HOPCODE 

        enemyRenderer.material.color = lungeChanceColorIndicator;
        
        for (int i = 0; i < hopCount; i++)
        {
            Vector3 hopDirection = -transform.forward * hopDistance;
            Vector3 targetPositionHop = startPosition + hopDirection;
            
            float hopPastedTime = 0f;

        

            while (hopPastedTime < hopDuration) 
            {
                hopPastedTime += Time.deltaTime;
                float t = Mathf.Clamp01(hopPastedTime / hopDuration);

                Vector3 currentPosition = Vector3.Lerp(startPosition, targetPositionHop, t);
                currentPosition.y += hopHeight * Mathf.Sin(t * Mathf.PI);

                transform.position = currentPosition;
                                
                yield return null;
            }

            transform.position = targetPositionHop;
            startPosition = targetPositionHop;

            ///delete is now no longer efficent 
            //hopPastedTime = 0f;

       /*     while (hopPastedTime < hopDuration)
            {
                transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward * hopDistance, (hopPastedTime / hopDuration));
                hopPastedTime += Time.deltaTime;
                yield return null;
            }
*/
            //transform.position += transform.forward * hopDistance;

        }

        ////THE LUNGE CODE 
        enemyRenderer.material.color = lungeColorIndicator;

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Vector3 targetPosition = playerTransform.position + directionToPlayer * lungeDisatance;

        pastedTime = 0f;
        

        while (pastedTime < lungeDuration) 
        {
            float t = pastedTime / lungeDuration;

            Vector3 curvedPosition = Vector3.Lerp(transform.position, targetPosition, t);
            curvedPosition.y += lungeHeight * Mathf.Sin(t * Mathf.PI);

            transform.position = curvedPosition;
            
            pastedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        isLunging = false;

        yield return new WaitForSeconds(lungeCooldown);

        canLunge = true;
    }

    public IEnumerator LeapColorIndicator() 
    {
        enemyRenderer.material.color = lungeChanceColorIndicator;

        yield return new WaitForSeconds(0.5f);

        //enemyRenderer.material.color = originalColor;
    }
}
