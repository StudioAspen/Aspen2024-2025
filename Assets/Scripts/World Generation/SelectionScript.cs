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


    void Start()
    {

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

    private IEnumerator IslandToSpawn() 
    {

        yield return new WaitForSeconds(2/*make variable for spawn in buffer*/);

        Instantiate(islandToSpawn, spotSelect + islandSpawnOffsetY , Quaternion.identity);


    }
}
