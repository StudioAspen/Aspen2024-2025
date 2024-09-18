using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public int maxShopCurrency;
    public int currentShopCurrency;

    public bool finishedWave;

    public float spawnTimerMax;
    public float spawnTimer;
    public bool spawnRdy;
    public int spawnLocation;
    public SqaureManager SqaureManager;

    public int followerSpawnThreshhold;
    public int leaperSpawnThreshhold;
    public int chargerSpawnThreshhold;
    

    public int followerValue;
    public int leaperValue;
    public int chargerValue;

    public GameObject follower;
    public GameObject leaper;
    public GameObject charger;

    void Start()
    {
        ////temp
        maxShopCurrency = 10;
        spawnTimerMax = 3;
        SqaureManager = GameObject.FindObjectOfType<SqaureManager>();
        currentShopCurrency = maxShopCurrency;

    }

     
    void Update()
    {

        if (currentShopCurrency <= 0) 
        {
            finishedWave = true;    
        }

        if (currentShopCurrency > 0 ) 
        {
            finishedWave = false;
            
            if (spawnRdy == true) 
            {
                StartCoroutine(SpawnEnemy());
                spawnTimer = spawnTimerMax;
                spawnRdy = false;
            }

        }

        if (spawnTimer <= 0 && spawnRdy == false) 
        {
            
            spawnRdy = true;
        }
        spawnTimer -= Time.deltaTime;
    }

    public IEnumerator SpawnEnemy() 
    {
        spawnLocation = Random.Range(1, 5);
        yield return new WaitForSeconds(1);

        if (spawnLocation == 1) 
        {
            Instantiate(follower, SqaureManager.enemySpawnPoint1.transform);
        }


        if (spawnLocation == 2)
        {
            Instantiate(follower, SqaureManager.enemySpawnPoint2.transform);
        }


        if (spawnLocation == 3)
        {
            Instantiate(follower, SqaureManager.enemySpawnPoint3.transform);
        }


        if (spawnLocation == 4)
        {
            Instantiate(follower, SqaureManager.enemySpawnPoint4.transform);
        }
        currentShopCurrency -= followerValue;
        yield return new WaitForSeconds(1);
    } 
}
