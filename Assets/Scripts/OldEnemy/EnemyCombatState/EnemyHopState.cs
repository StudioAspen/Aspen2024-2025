using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHopState : EnemyState
{
  
    public int hopCount;

    void EnemyState.Enter(EnemyAgent agent)
    {
        hopCount = agent.hopCount;
        agent.enemyRenderer.material.color = agent.lungeChanceColorIndicator;
        agent.StartCoroutine(HopCoroutine(agent));

    }

    void EnemyState.Exit(EnemyAgent agent)
    {
        
    }

    EnemyStateId EnemyState.GetId()
    {
        return EnemyStateId.HopBack;

    }

    void EnemyState.Update(EnemyAgent agent)
    {
       
    }
    public IEnumerator HopCoroutine(EnemyAgent agent)
    {
        // Access the root transform
        Transform rootTransform = agent.enemyTransform.parent;
        Vector3 startPosition = rootTransform.position;

       // Vector3 startPosition = agent.enemyTransform.position;

        for (int i = 0; i < hopCount; i++)
        {
            Vector3 hopDirection = -rootTransform.forward * agent.hopDistance;
            Vector3 targetPositionHop = startPosition + hopDirection;
            float hopPastedTime = 0f;

            while (hopPastedTime < agent.hopDuration)
            {
                hopPastedTime += Time.deltaTime;
                float t = Mathf.Clamp01(hopPastedTime / agent.hopDuration);
                Vector3 currentPosition = Vector3.Lerp(startPosition, targetPositionHop, t);
                currentPosition.y += agent.hopHeight * Mathf.Sin(t * Mathf.PI);
                rootTransform.position = currentPosition;

                yield return null;
            }

            rootTransform.position = targetPositionHop;
            startPosition = targetPositionHop;
        }

        // Transition to lunge state
        agent.attackStateMachine.ChangeState(EnemyStateId.Leap);
    }
}
