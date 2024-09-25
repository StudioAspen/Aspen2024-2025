using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnemyStateId 
{
    ChasePlayer,
    Death
}
public interface EnemyState
{
    EnemyStateId GetId();
    void Enter(EnemyAgent agent);
    void Update(EnemyAgent agent);
    void Exit(EnemyAgent agent);

}
