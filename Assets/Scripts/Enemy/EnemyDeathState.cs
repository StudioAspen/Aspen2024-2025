using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathState : EnemyState
{
    public BaseEnemy enemyStats;
   

    public void Enter(EnemyAgent agent)
    {
        enemyStats = GameObject.FindObjectOfType<BaseEnemy>();
       
    }

    public void Exit(EnemyAgent agent)
    {
     
    }

    public EnemyStateId GetId()
    {
        return EnemyStateId.Death;
    }

    public void Update(EnemyAgent agent)
    {
   
    }
}
