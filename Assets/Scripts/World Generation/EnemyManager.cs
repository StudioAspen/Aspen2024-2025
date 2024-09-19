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

    
    public int enemyCost;
    public int followerCost;
    public int leaperCost;
    public int chargerCost;

    public GameObject follower;
    public GameObject leaper;
    public GameObject charger;

    public WorldManager WorldManager;

    [Header("Enemy Weight Process")]
  
    
    [SerializeField] public float totalWeight;

    [SerializeField] public float randomValue;
    [SerializeField] public float cumalativeWeight;
    public float RateScale;
    
    [System.Serializable]
    public class Enemy 
    {
        public GameObject prefab;
        public float weight;
        public float weightNormalized;
        public int enemyCost;
    }

    public List<Enemy> enemies = new List<Enemy>();
    void Start()
    {
        ////temp
        maxShopCurrency = 10;
        spawnTimerMax = 3;
        SqaureManager = GameObject.FindObjectOfType<SqaureManager>();
        currentShopCurrency = maxShopCurrency;
        WorldManager = GameObject.FindObjectOfType<WorldManager>();

        ///not temp
        enemies.Add(new Enemy { prefab = follower, enemyCost = followerCost });
        enemies.Add(new Enemy { prefab = leaper, enemyCost = leaperCost });
        enemies.Add(new Enemy { prefab = charger, enemyCost = chargerCost });

    }


    void Update()
    {

        if (currentShopCurrency <= 0) 
        {
            finishedWave = true;    
        }

        if (currentShopCurrency > 0 && WorldManager.waveBufferTimer <= 0) 
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
        CalculateWeight();
        randomValue = Random.Range(0f, 1f);
        cumalativeWeight = 0f;
        spawnLocation = Random.Range(1, 5);
        yield return new WaitForSeconds(1);

        foreach (var enemy in enemies) 
        {
            cumalativeWeight += enemy.weightNormalized;

            if (randomValue < cumalativeWeight && currentShopCurrency >= enemy.enemyCost) 
            {
                if (spawnLocation == 1) 
                {
                    Instantiate(enemy.prefab, SqaureManager.enemySpawnPoint1.transform);
                }   


                if (spawnLocation == 2)
                {
                    Instantiate(enemy.prefab, SqaureManager.enemySpawnPoint2.transform);
                }


                if (spawnLocation == 3)
                {
                    Instantiate(enemy.prefab, SqaureManager.enemySpawnPoint3.transform);
                }   

    
                if (spawnLocation == 4)
                {
                    Instantiate(enemy.prefab, SqaureManager.enemySpawnPoint4.transform);
                }
                currentShopCurrency -= enemy.enemyCost;
                break;
            }
        

        }


        yield return new WaitForSeconds(1);
    } 

    public void WaveReset() 
    {

        currentShopCurrency = maxShopCurrency;
    }

    public void CalculateWeight() 
    {
        totalWeight = 0f;

        foreach (var enemy in enemies) 
        {
            enemy.weight = 1f / Mathf.Pow(enemy.enemyCost, RateScale);
            totalWeight += enemy.weight;
        }

        foreach (var enemy in enemies) 
        {
            enemy.weightNormalized = enemy.weight / totalWeight;
        }
    }
}
