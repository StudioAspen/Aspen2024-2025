using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{

    [Header("Enemy Stats")]
    [SerializeField] public int enemyMaxHP;
    [SerializeField] public int enemyCurrentHP;
    [SerializeField] public int enemyLevel;
    [SerializeField] public float enemyMoveSpeed;

    public WorldManager levelManager;
    void Start()
    {
        enemyMaxHP = 100;   
        enemyMoveSpeed = 2;
        enemyLevel = 1;
        enemyCurrentHP = enemyMaxHP;

        levelManager = GameObject.FindObjectOfType<WorldManager>();

        levelManager.monstersAlive += 1;
    }


    void Update()
    {
        if (enemyCurrentHP <= 0) 
        {
            Destroy(gameObject);
            levelManager.monstersAlive -= 1;
        }

    }
}
