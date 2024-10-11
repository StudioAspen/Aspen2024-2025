using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public float maxShopCurrency;
    public float currentShopCurrency;

    public bool IsWaveFinished;

    public float spawnTimerMax;
    public float spawnTimer;
    public bool spawnRdy;
    public int spawnLocation;
    public IslandManager SqaureManager;

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

    [Header("Currency calculation")]
    [SerializeField] public float baseCurrency;
    [SerializeField] public float growthFactor;
    [SerializeField] public int polynomialDegree;

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
        spawnTimerMax = 3;

        ///not temp 
        WorldManager = GameObject.FindObjectOfType<WorldManager>();
        SqaureManager = GameObject.FindObjectOfType<IslandManager>();
        maxShopCurrency = baseCurrency + (growthFactor * Mathf.Pow(SqaureManager.sqaureLevel,polynomialDegree));
        currentShopCurrency = maxShopCurrency;
       
        ///not temp
        enemies.Add(new Enemy { prefab = follower, enemyCost = followerCost });
        enemies.Add(new Enemy { prefab = leaper, enemyCost = leaperCost });
        enemies.Add(new Enemy { prefab = charger, enemyCost = chargerCost });

    }


    void Update()
    {

        if (currentShopCurrency <= 0) 
        {
            IsWaveFinished = true;    
        }

        if (currentShopCurrency > 0) 
        {
            IsWaveFinished = false;
            
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
                    Instantiate(enemy.prefab, SqaureManager.EnemySpawnPoint1.transform);
                }   


                if (spawnLocation == 2)
                {
                    Instantiate(enemy.prefab, SqaureManager.EnemySpawnPoint2.transform);
                }


                if (spawnLocation == 3)
                {
                    Instantiate(enemy.prefab, SqaureManager.EnemySpawnPoint3.transform);
                }   

    
                if (spawnLocation == 4)
                {
                    Instantiate(enemy.prefab, SqaureManager.EnemySpawnPoint4.transform);
                }
                currentShopCurrency -= enemy.enemyCost;
                break;
            }
        

        }


        yield return new WaitForSeconds(1);
    } 

    public void WaveReset() 
    {
        maxShopCurrency = baseCurrency + (growthFactor * Mathf.Pow(SqaureManager.sqaureLevel,polynomialDegree));
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
