using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class MasterLevelManager : MonoBehaviour
{

    public GameObject[] borders;
    public List<GameObject> allBorders;
    public int borderAmount;
    public GameObject masterBorders;

    public GameObject openSlotPlaceHolder;
    public Vector3 selectorOF;
    public NavMeshSurface navsurface;
    



    void Start()
    {
        masterBorders = GameObject.Find("Borders");
        UpdateNavMesh();
    }


    void Update()
    {

        borders = allBorders.ToArray();
        if (Input.GetKeyDown(KeyCode.P)) 
        {
            navsurface.BuildNavMesh();
        }

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

    public IEnumerator IslandSelectCheck()
    {
        Debug.Log("Checking");


        allBorders = CheckLevel(masterBorders);

        foreach (GameObject child in allBorders)
        {
            Debug.Log(child.name);
            borderAmount += 1;
          
        }

        yield return null;

    }

    public void SpawnPlaceHolder() 
    {
        Debug.Log("Spawning Options");
        for (int i = 0; i < borders.Length; i++)
        {
            Debug.Log("here");
            Instantiate(openSlotPlaceHolder, borders[i].transform.position + selectorOF, Quaternion.identity);
        }

    }

    public void UpdateNavMesh()
    {

        navsurface.BuildNavMesh();

    }
}
