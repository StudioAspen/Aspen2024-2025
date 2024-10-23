using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ChargerIdleState : EnemyIdleState
{

    /* These three fields below might want to be moved to a new script called Charger.cs that inherits from Enemy.cs and then serialize them for editor configuration. */
    private float wanderRadius = 5f;
    private float wanderWaitMin = 1f, wanderWaitMax = 3f;


    private Coroutine wanderCoroutine;

    public ChargerIdleState(Enemy enemy) : base(enemy)
    {
        this.enemy = enemy;
    }

    public override void OnEnter() 
    {
        wanderCoroutine = enemy.StartCoroutine(WanderCoroutine());
        //Debug.Log("Started charger wander coroutine");
    }

    public override void OnExit() 
    {
        if (wanderCoroutine != null) {
            enemy.StopCoroutine(wanderCoroutine);
            wanderCoroutine = null;
            //Debug.Log("Stopped charger wander coroutine");
        }
    }

    public override void Update() 
    { 
        if (enemy.Target != null) 
        {
            enemy.ChangeState(enemy.EnemyChaseState); // Replace EnemyChaseState with ChargerPlayerDetectedState later on
        }
    }


    public override void FixedUpdate() { }


    private IEnumerator WanderCoroutine() 
    {  
        while (enemy.CurrentState == this) 
        {
            yield return new WaitForSeconds(Random.Range(wanderWaitMin, wanderWaitMax));
            while (GoToRandomWanderPoint() == false) { continue; } // Keeps trying to find a random wander point until successful
        }
    }

    /* Returns true and moves to new wander-to point if found successfully, and false otherwise. */
    private bool GoToRandomWanderPoint() 
    {
        // Get a random point nearby 
        Vector3 randomPoint = (Random.insideUnitSphere * wanderRadius) + enemy.transform.position;
        NavMeshHit navMeshHit;
        if (NavMesh.SamplePosition(randomPoint, out navMeshHit, wanderRadius, NavMesh.AllAreas)) 
        {
            enemy.NavMeshAgent.SetDestination(navMeshHit.position);
            //Debug.Log("New charger wander-to position found!");
        } else {
            //Debug.LogWarning("New random wander-to position try for charger failed!");
            return false;
        }
        return true;
    }

}
