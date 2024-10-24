using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyMovementStateId
{
    Idle,
    Patrol,
    ChasePlayer,
    HoverPlayer,
    Hit,

}

public interface EnemyMovementState
{
    EnemyMovementStateId GetId();
    void Enter(EnemyAgent agent);
    void Update(EnemyAgent agent);
    void Exit(EnemyAgent agent);
}