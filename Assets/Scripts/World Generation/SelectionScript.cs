using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionScript : MonoBehaviour
{

    public Camera playerCam;
    public LayerMask SelectionLayer;

    public GameObject islandToSpawn;
    public Vector3 islandSpawnOffsetY;
    public Vector3 islandSpawnOffsetZ;
    public Vector3 islandSpawnOffsetX;

    public GameObject[] selectOptions;
    public GameObject[] selected_Option;
    public Vector3 spotSelect;

    public BorderRemover borderRemove;
    public WorldManager worldManage;


    void Start()
    {
        worldManage = GameObject.FindObjectOfType<WorldManager>();
    }


    void Update()
    {
            
        selectOptions = GameObject.FindGameObjectsWithTag("UnSelected");
        selected_Option= GameObject.FindGameObjectsWithTag("Selected");

       
        if (selected_Option.Length == 1) 
            {
            spotSelect = selected_Option[0].transform.position;

            }
            
            Vector3 mousePosition = Input.mousePosition;
            Ray myray = playerCam.ScreenPointToRay(mousePosition);
            RaycastHit raycasthit;

            bool weHitSomething = Physics.Raycast(myray, out raycasthit, Mathf.Infinity, SelectionLayer);

        if (selectOptions.Length != 0  && selected_Option.Length == 1 )
        {
            Debug.Log("Cleared");
            Destroy(GameObject.FindWithTag("UnSelected"));
        }


        if (Input.GetMouseButtonDown(0))
            {



            if (weHitSomething)
            {

                Debug.Log(raycasthit.transform.name);
                Debug.Log("Fuck it we ball");
                raycasthit.transform.gameObject.tag = "Selected";

                StartCoroutine(IslandToSpawn());
            }

            else
            {
                Debug.Log("Shits not working homie");

            }
            }

        if (weHitSomething)
        {
            
            Debug.DrawLine(myray.origin, raycasthit.point, Color.blue);
           
        }
    }

    public IEnumerator IslandToSpawn() 
    {
        worldManage.selecting = false;

        yield return new WaitForSeconds(1/*make variable for spawn in buffer*/);

        borderRemove = GameObject.FindObjectOfType<BorderRemover>();

        if (borderRemove.bordersToRemove[1].gameObject.name == "Wall 1")
        {
            islandSpawnOffsetX.Set((float)-0.01, 0, 0);
            islandSpawnOffsetZ.Set(0, 0, (float)15.07);
        }

        if (borderRemove.bordersToRemove[0].gameObject.name == "Wall 2")
        {
            islandSpawnOffsetX.Set((float)15.14, 0, 0);
            islandSpawnOffsetZ.Set(0, 0, (float)0.04);
        }

        if (borderRemove.bordersToRemove[1].gameObject.name == "Wall 3")
        {
            islandSpawnOffsetX.Set((float)-0.01, 0, 0);
            islandSpawnOffsetZ.Set(0, 0, (float)-15.07);
        }

        if (borderRemove.bordersToRemove[1].gameObject.name == "Wall 4")
        {
            islandSpawnOffsetX.Set((float)-15.14, 0, 0);
            islandSpawnOffsetZ.Set(0, 0, (float)0.04);
        }


        yield return new WaitForSeconds(2/*make variable for spawn in buffer*/);

        Instantiate(islandToSpawn, spotSelect + islandSpawnOffsetX + islandSpawnOffsetY + islandSpawnOffsetZ, Quaternion.identity);


    }
}
