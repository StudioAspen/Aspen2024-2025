using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderRemover : MonoBehaviour
{

    public int wallsTouched;
    public Vector3 selectorDropSpeed;

    void Start()
    {
        
    }


    void Update()
    {
        if(gameObject.transform.position.y > 0)
        {
            GetComponent<Transform>().position -= selectorDropSpeed;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Border") 
        {
        wallsTouched += 1;
        }
            
    }
}
