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

    [Header("Misc Controls")]
    [SerializeField] public bool skyView;
    [SerializeField] public float waveBuffer;
    [SerializeField] public float waveBufferTimer;

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
        masterIsland = GameObject.Find("MasterLevel");
        masterLevel = GameObject.FindObjectOfType<MasterLevelManager>();
        waveBuffer = 5f;
        waveBufferTimer = waveBuffer;
    }

   
    void Update()
    {
        if (waveBufferTimer <= 0) 
        {
          

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

        /*islandSelectTimertext.text = "Time Left: " + islandSelectTimer.ToString("0.00");*/

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
       
    }

}
