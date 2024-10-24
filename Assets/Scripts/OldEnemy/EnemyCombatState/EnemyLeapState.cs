using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLeapState : EnemyState
{
    public void Enter(EnemyAgent agent)
    {
        // Set up for lunge
        agent.isLunging = true;
        agent.enemyRenderer.material.color = agent.lungeColorIndicator;

        // Start Lunge Coroutine
        agent.StartCoroutine(LungeCoroutine(agent,agent.playerTransform));
    }

    public void Exit(EnemyAgent agent)
    {
        agent.lungeCooldownTimer = agent.lungeCooldown; // Start cooldown timer
        agent.isLunging = false;
    }

    public EnemyStateId GetId()
    {
        return EnemyStateId.Leap;

    }

    public void Update(EnemyAgent agent)
    {

    }

    private IEnumerator LungeCoroutine(EnemyAgent agent, Transform playerTransform)
    {
        // Access the root transform
        Transform rootTransform = agent.enemyTransform.parent;


        // Calculate the direction to the player
        Vector3 directionToPlayer = (playerTransform.position - rootTransform.position).normalized;
        Vector3 targetPosition = rootTransform.position + directionToPlayer * agent.lungeDistance;

        float pastedTime = 0f;
        while (pastedTime < agent.lungeDuration)
        {
            float t = pastedTime / agent.lungeDuration;
            Vector3 curvedPosition = Vector3.Lerp(rootTransform.position, targetPosition, t);
            curvedPosition.y += agent.lungeHeight * Mathf.Sin(t * Mathf.PI);

            rootTransform.position = curvedPosition;
            pastedTime += Time.deltaTime;
            yield return null;
        }

        rootTransform.position = targetPosition;

        // Transition back to idle or follow state
        agent.movementStateMachine.ChangeState(EnemyMovementStateId.ChasePlayer);
    }
}