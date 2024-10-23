using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerDazedState : EnemyBaseState 
{

    private float dazedDuration; // Might want to move this variable to a new script called Charger.cs that inherits from Enemy.cs and then serialize it for editor configuration.

    private Coroutine dazedCoroutine;
    private float healthWhenEnterDazed;

    public ChargerDazedState(Enemy enemy) : base(enemy) 
    {
        this.enemy = enemy;
    } 
    public override void OnEnter() 
    {
        dazedCoroutine = enemy.StartCoroutine(DazedCoroutine());
        healthWhenEnterDazed = enemy.CurrentHealth;
        //Debug.Log("Enter charger dazed coroutine");
    }

    public override void OnExit() 
    { 
        if (dazedCoroutine != null) 
        {
            enemy.StopCoroutine(dazedCoroutine);
            dazedCoroutine = null;
            //Debug.Log("Exit charger dazed coroutine");
        }
    }

    public override void Update() 
    {
        if (enemy.CurrentHealth < healthWhenEnterDazed) 
        {
            // Enemy took damage while dazed!
            //Debug.Log("Charger took damage while dazed! Entering damaged state.");
            //enemy.ChangeState(enemy.ChargerDamagedState); // Uncomment this once Charger.cs (inherits from Enemy.cs) exists, ChargerDamagedState exists, and ChargerDamagedState is added into Charger.cs.
        }
    }

    public override void FixedUpdate() { }

    private IEnumerator DazedCoroutine() 
    {
        yield return new WaitForSeconds(dazedDuration);
        //Debug.Log("Charger dazed timer is over!");
        //enemy.ChangeState(enemy.ChargerIdleState); // Uncomment this once Charger.cs (inherits from Enemy.cs) exists and inside there EnemyIdleState is replaced with ChargerIdleState.
    }

}
