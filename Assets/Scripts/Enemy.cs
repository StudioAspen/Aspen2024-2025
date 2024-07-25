using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public void TakeDamage(int damage)
    {
        Debug.Log($"{gameObject.name} took {damage} damage");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Sword")
        {
            TakeDamage(1);
        }
    }
}
