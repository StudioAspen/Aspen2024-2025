using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public BaseEnemy enemyStats;

    public void Start()
    {
        enemyStats = GameObject.FindObjectOfType<BaseEnemy>();
    }
    public void TakeDamage(int damage)
    {
        enemyStats.enemyCurrentHP -= 25;
    }
}
