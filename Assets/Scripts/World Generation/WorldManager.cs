using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldManager : MonoBehaviour
{
    [Header("Infromation Needed to start")]
    [SerializeField] public GameObject spawnSqaure;
    [SerializeField] public GameObject sqaureToSpawn;

    [Header("Spawn Next Island")]
    [SerializeField] private int currentSquareCount = 0;
    [SerializeField] private int currentTotalOpenSides = 0;

    [SerializeField] public int monstersAlive;
    [SerializeField] public bool mapClear;

    [SerializeField] public MasterLevelManager masterLevel;
    public HashSet<SqaureManager> islandManagers = new HashSet<SqaureManager>();
    public HashSet<EnemyManager> enemyManagers = new HashSet<EnemyManager>();

    [Header("Misc Controls")]
    [SerializeField] public bool skyView;
    [SerializeField] public float waveBuffer;
    [SerializeField] public float waveBufferTimer;
    [SerializeField] public bool LevelIsands;

    [Header("Island Selection")]
    [SerializeField] public GameObject playerCamera;
    [SerializeField] public GameObject islandSelectCamera;
    [SerializeField] float islandSelectTimer;
    [SerializeField] float islandSelectTimerSet;
    [SerializeField] public bool selecting;
    [SerializeField] public GameObject islandTimertext;
    [SerializeField] public TextMeshProUGUI islandSelectTimertext;
    [SerializeField] public GameObject islandSelectPlaceHolder;
    [SerializeField] public GameObject masterIsland;




    void Start()
    {
        islandManagers.Add(GameObject.FindObjectOfType<SqaureManager>());
        enemyManagers.Add(GameObject.FindObjectOfType<EnemyManager>());
        masterIsland = GameObject.Find("MasterLevel");
        masterLevel = GameObject.FindObjectOfType<MasterLevelManager>();
        waveBuffer = 5f;
        waveBufferTimer = waveBuffer;
    }

   
    void Update()
    {
        if (waveBufferTimer <= -3) 
        {

            masterLevel.UpdateNavMesh();           

          if (mapClear == true && selecting == false) 
          {
            
              StartCoroutine(selectNextIsland());
                waveBufferTimer = waveBuffer;
          }
        }
      

        if (monstersAlive == 0)
        {

            mapClear = true;

        }
        else 
        {
            mapClear = false;
        }

        if (Input.GetKeyDown(KeyCode.Q) && skyView == false) 
        {
            islandTimertext.SetActive(true);
            islandSelectCamera.SetActive(true);
            playerCamera.SetActive(false);
            skyView = true;
        }

        if (Input.GetKeyDown(KeyCode.E) && skyView == true)
        {
           
            islandSelectCamera.SetActive(false);
            playerCamera.SetActive(true);
            islandTimertext.SetActive(false);
            skyView = false;
        }

        if (selecting == false) 
        {
            waveBufferTimer -= Time.deltaTime;
        }
    }

    private IEnumerator selectNextIsland() 
    {
       
        Debug.Log("Make Selection");
        islandSelectTimer = islandSelectTimerSet;
        selecting = true;

        StartCoroutine(masterLevel.IslandSelectCheck());
            //make sure Island Select Check wait is less than this number
        yield return new WaitForSeconds(0.75f);

        masterLevel.SpawnPlaceHolder();
        PrepareNextWaves();

    }

    public void PrepareNextWaves() 
    {
        foreach (var SqaureManager in islandManagers) 
        {
            SqaureManager.LevelUp();
            AddIslandManager(SqaureManager);
        }
        
        foreach (var enemyManager in enemyManagers)
        {
            enemyManager.WaveReset();
            AddEnemyManager(enemyManager);
        }

    }

    public void AddIslandManager(SqaureManager spawner) 
    {
        islandManagers.Add(spawner);
    }
    public void AddEnemyManager(EnemyManager director)
    {
        enemyManagers.Add(director);
    }
}
