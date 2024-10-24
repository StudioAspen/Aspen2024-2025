using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathState : EnemyState
{

   

    public void Enter(EnemyAgent agent)
    {
 
       
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
