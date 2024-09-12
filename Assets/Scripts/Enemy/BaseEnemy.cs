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
    void Start()
    {
        enemyMoveSpeed = 2;
    }


    void Update()
    {
        
    }
}
