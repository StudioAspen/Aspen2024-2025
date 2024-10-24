using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHoverPlayerState : EnemyMovementState
{


    private float hoverDistance;

    public EnemyHoverPlayerState(float initialHoverDistance)
    {
        hoverDistance = initialHoverDistance;
    }

    public EnemyMovementStateId GetId() => EnemyMovementStateId.HoverPlayer;

    public void Enter(EnemyAgent agent)
    {
        // You can perform any setup needed when entering this state
    }

    public void Update(EnemyAgent agent)
    {
        Vector3 playerPosition = agent.playerTransform.position;

        // Calculate a position to hover around the player
        Vector3 hoverPosition = CalculateHoverPosition(playerPosition, hoverDistance, Time.time);

        // Set the NavMeshAgent's destination
        agent.navMeshAgent.SetDestination(hoverPosition);

 
    }

    public void Exit(EnemyAgent agent)
    {
        // Clean up or reset anything necessary when exiting this state
    }

    private Vector3 CalculateHoverPosition(Vector3 playerPosition, float radius, float time)
    {
        // Calculate an offset around the player using sine and cosine
        float xOffset = radius * Mathf.Cos(time);
        float zOffset = radius * Mathf.Sin(time);

        // Ensure the enemy stays at the hover distance
        return new Vector3(playerPosition.x + xOffset, playerPosition.y, playerPosition.z + zOffset);
    }

    // Method to adjust hover distance
    public void SetHoverDistance(float newHoverDistance)
    {
        hoverDistance = newHoverDistance;
    }
}
