using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class SqaureManager : MonoBehaviour
{

    [Header("Square Stats")]
    [SerializeField] public GameObject masterLevel;
    [SerializeField] public Transform masterLevelparent;
    [SerializeField] public int sqaureLevel;
    public WorldManager WorldManager;

    public Vector3 islandRiseSpeed;
    public bool openSides = true;
    public int sqaureScore;
    public int wallHitScore;
    public int wallHitScore2;
    public int wallHitScore3;
    public int wallHitScore4;

    [SerializeField] LayerMask WallLayerMask;
    public Vector3 raycastoffset;

    RaycastHit wallHit;
    RaycastHit wallHit2;
    RaycastHit wallHit3;
    RaycastHit wallHit4;

    public GameObject Border1;
    public GameObject Border2;
    public GameObject Border3;
    public GameObject Border4;

    public GameObject islandToSpawn;
    public Vector3 islandSpawnOS;

    public GameObject enemySpawnPoint1;
    public GameObject enemySpawnPoint2;
    public GameObject enemySpawnPoint3;
    public GameObject enemySpawnPoint4;
    public NavMeshSurface navsurface;

    void Start()
    {
        StartCoroutine(SpawnDelay());
        masterLevel = GameObject.Find("MasterLevel");
        transform.SetParent(masterLevel.transform);
        masterLevelparent = GameObject.Find("Borders").transform;
        WorldManager = GameObject.FindObjectOfType<WorldManager>();
        sqaureLevel = 1;
       
    }

    
    void FixedUpdate()
    {
        Ray rayFront = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
        Ray rayRight = new Ray(transform.position, transform.TransformDirection(Vector3.right));
        Ray rayLeft = new Ray(transform.position, transform.TransformDirection(Vector3.left));
        Ray rayBack = new Ray(transform.position, transform.TransformDirection(Vector3.back));

        if (gameObject.transform.position.y < -5) 
        {
              GetComponent<Transform>().position += islandRiseSpeed;
        }

        /*
            if (Physics.Raycast(rayFront, out wallHit, 100f))
            {
                Debug.DrawRay(transform.position + raycastoffset, transform.TransformDirection(Vector3.forward) * wallHit.distance, Color.red);
                wallHitScore = 1;
                Debug.Log(wallHit.point);

                if (Input.GetKeyDown(KeyCode.E)) 
                {
                Instantiate(islandToSpawn, wallHit.point + islandSpawnOS, Quaternion.identity);
                }
         
            }
            else { wallHitScore = 0; }


            if (Physics.Raycast(rayRight, out wallHit2, 100f)) 
            {
                Debug.DrawRay(transform.position + raycastoffset, transform.TransformDirection(Vector3.right) * wallHit2.distance, Color.red);
                wallHitScore2 = 1;
                Debug.Log(wallHit2.point);
            }
            else { wallHitScore2 = 0; }

            if (Physics.Raycast(rayLeft, out wallHit3, 100f)) 
            {
                Debug.DrawRay(transform.position + raycastoffset, transform.TransformDirection(Vector3.left) * wallHit3.distance, Color.red);
                wallHitScore3 = 1;
                Debug.Log(wallHit3.point);
            }
            else { wallHitScore3 = 0; }


            if (Physics.Raycast(rayBack, out wallHit4, 100f))
            {
                Debug.DrawRay(transform.position + raycastoffset, transform.TransformDirection(Vector3.back) * wallHit4.distance, Color.red);
                wallHitScore4 = 1;
                Debug.Log(wallHit4.point);
            }
            else { wallHitScore4 = 0; }
        
        

     

        if (sqaureScore <= 0)
        {
            openSides = false;
        }

        sqaureScore = wallHitScore + wallHitScore2 + wallHitScore3 + wallHitScore4;

        Debug.Log(sqaureScore);

        */
    }

    private IEnumerator SpawnDelay() 
    {

        yield return new WaitForSeconds(2);
   
        Border1.transform.SetParent(masterLevelparent);
        Border2.transform.SetParent(masterLevelparent);
        Border3.transform.SetParent(masterLevelparent);
        Border4.transform.SetParent(masterLevelparent);


        StartCoroutine(UpdateNavMesh());

    }

    public IEnumerator UpdateNavMesh() 
    {
       
        navsurface.BuildNavMesh();

        yield return new WaitForSeconds(3);

        navsurface.enabled = false;
    }

    public void LevelUp()
    {

        sqaureLevel += 1;

    }

}
    