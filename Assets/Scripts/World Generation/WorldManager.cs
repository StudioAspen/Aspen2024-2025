using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [Header("Infromation Needed to start")]
    [SerializeField] public GameObject spawnSqaure;
    [SerializeField] public GameObject sqaureToSpawn;

    [Header("Spawn Next Island")]
    [SerializeField] private int currentSquareCount = 0;
    [SerializeField] private int currentTotalOpenSides = 0;
    [SerializeField] private bool waveEnded = false;


    void Start()
    {
            
    }

   
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (waveEnded == false)) 
        {
            waveEnded = true;
      /*      /Instantiate(sqaureToSpawn)/*/
            /*StartCoroutine(selectNextIsland());*/

        }
    }

/*    private IEnumerator selectNextIsland() 
    {

        Debug.Log("Make Selection");
        yield return new WaitForSeconds(10);




        Debug.Log("Time is Up");
        waveEnded = false;
       
    }*/
}
