using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public float detectionRadius = 10f; // Radius to detect nearby enemies
    public LayerMask enemyLayer; // Layer to identify enemy objects

    public  List<EnemyAgent> nearbyEnemies = new List<EnemyAgent>();

    void Update()
    {
        DetectEnemies();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private void DetectEnemies()
    {
        nearbyEnemies.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

        Debug.Log(colliders.Length);

        foreach (var collider in colliders)
        {
            EnemyAgent enemy = collider.GetComponentInChildren<EnemyAgent>();
            if (enemy != null)
            {
                nearbyEnemies.Add(enemy);
            }
        }

        // Update each enemy with the number of nearby enemies
        foreach (var enemy in new List<EnemyAgent>(nearbyEnemies))
        {
            enemy.UpdateEnemyStates(nearbyEnemies);
        }
    }
}
