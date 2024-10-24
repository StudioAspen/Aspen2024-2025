using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderRemover : MonoBehaviour
{

    public int wallsTouched;
    public Vector3 selectorDropSpeed;

    public List<GameObject> borders;
    public GameObject[] bordersToRemove;

    public bool Buffer = false;
    
    void Start()
    {

    }


    void Update()
    {
        if (gameObject.transform.position.y > 0)
        {
            GetComponent<Transform>().position -= selectorDropSpeed;
        }

        if (wallsTouched >= 4) 
        {
            StartCoroutine(BorderRemoval());

            if (Buffer == true) 
            {
                foreach (GameObject go in bordersToRemove) 
                {
                    Destroy(go);
                }
            
            }
        }

        bordersToRemove = borders.ToArray();    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Border") 
        {
        wallsTouched += 1;
        borders.Add(other.gameObject);
        }



    }

    private IEnumerator BorderRemoval()
    {
        yield return new WaitForSeconds(3/*make variable for spawn in buffer*/);

        Buffer = true;

        yield return new WaitForSeconds(1/*make variable for spawn in buffer*/);

        Destroy(gameObject);

    }
}
