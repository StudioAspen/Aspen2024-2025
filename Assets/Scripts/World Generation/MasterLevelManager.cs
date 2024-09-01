using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterLevelManager : MonoBehaviour
{

    public GameObject[] borders;
    public List<GameObject> allBorders;
    public int borderAmount;
    public GameObject masterBorders;

    public GameObject openSlotPlaceHolder;
    public Vector3 selectorOF;

    void Start()
    {
        masterBorders = GameObject.Find("Borders");
 
    }


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            StartCoroutine(IslandSelectCheck());
        }


        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Spawning Options");
            for (int i = 0; i < borders.Length; i++)
           {
                Debug.Log("here");
                Instantiate(openSlotPlaceHolder, borders[i].transform.position + selectorOF, Quaternion.identity);
           }


        }
  
        borders = allBorders.ToArray();

    }

    List<GameObject> CheckLevel(GameObject masterBorders)
    {
        allBorders = new List<GameObject>();

        foreach (Transform child in masterBorders.transform) 
        {
            allBorders.Add(child.gameObject);      
        }

        return allBorders;
      

    }

    private IEnumerator IslandSelectCheck()
    {
        Debug.Log("Checking");

        yield return new WaitForSeconds(1);


        allBorders = CheckLevel(masterBorders);

        foreach (GameObject child in allBorders)
        {
            Debug.Log(child.name);
            borderAmount += 1;
          
        }



    }

}
