using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewBehaviourScript : MonoBehaviour
{
    [Header("Infromation Needed to start")]
    [SerializeField] public GameObject spawnSqaure;
    [SerializeField] public GameObject sqaureToSpawn;

    [Header("Spawn Next Island")]
    [SerializeField] private int currentSquareCount = 0;
    [SerializeField] private int currentTotalOpenSides = 0;
    [SerializeField] private bool waveEnded = false;

    [Header("Island Selection")]
    [SerializeField] public GameObject playerCamera;
    [SerializeField] public GameObject islandSelectCamera;
    [SerializeField] float islandSelectTimer;
    [SerializeField] float islandSelectTimerSet;
    [SerializeField] bool selecting;
    [SerializeField] public GameObject islandTimertext;
    [SerializeField] public TextMeshProUGUI islandSelectTimertext;
    [SerializeField] public GameObject islandSelectPlaceHolder;
    [SerializeField] public GameObject masterIsland;



    void Start()
    {
        masterIsland = GameObject.Find("MasterLevel");
    }

   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            
        StartCoroutine(selectNextIsland());

        }

        if (Input.GetKeyDown(KeyCode.R) && (waveEnded == true)) 
        {
            waveEnded = false;
            islandSelectCamera.SetActive(false);
            playerCamera.SetActive(true);
        }

        islandSelectTimertext.text = "Time Left: " + islandSelectTimer.ToString("0.00");

        if (selecting == true) 
        {
            islandSelectTimer -= Time.deltaTime;
        }
    }

    private IEnumerator selectNextIsland() 
    {

        Debug.Log("Make Selection");
        islandTimertext.SetActive(true);
        islandSelectCamera.SetActive(true);
        playerCamera.SetActive(false);
        islandSelectTimer = islandSelectTimerSet;
        selecting = true;


        yield return new WaitForSeconds(islandSelectTimer);
        

        if (islandSelectTimer <= 0) 
        {

        selecting = false;     
        islandSelectCamera.SetActive(false);
        playerCamera.SetActive(true);
        islandTimertext.SetActive(false);
        Debug.Log("Time is Up");
        waveEnded = false;

        }
   
       
    }

    public void PossibleIslands() 
    {
     
        

    }
}
