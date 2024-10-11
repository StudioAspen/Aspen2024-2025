using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public float maxShopCurrency;
    public float currentShopCurrency;

    public bool IsWaveFinished => currentShopCurrency <= 0 && enemiesSpawned.Count <= 0;

    public float spawnTimerMax;
    public float spawnTimer;
    public bool spawnRdy;
    public int spawnLocation;
    public IslandManager islandManager;

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

    public bool CanSpawn = false;
    private List<GameObject> enemiesSpawned = new List<GameObject>();
    private bool isSpawnDelayed = false;

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
        islandManager = GetComponent<IslandManager>();
        maxShopCurrency = baseCurrency + (growthFactor * Mathf.Pow(islandManager.level,polynomialDegree));
        currentShopCurrency = maxShopCurrency;
       
        ///not temp
        enemies.Add(new Enemy { prefab = follower, enemyCost = followerCost });
        enemies.Add(new Enemy { prefab = leaper, enemyCost = leaperCost });
        enemies.Add(new Enemy { prefab = charger, enemyCost = chargerCost });


        StartCoroutine(SpawnCoroutine());
    }


    void Update()
    {
        RemoveDeadEnemies();

        if (currentShopCurrency <= 0) CanSpawn = false;

        if (Input.GetKeyDown(KeyCode.K))
        {
            ClearWave();
        }
    }

    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            if (!CanSpawn)
            {
                yield return null;
                continue;
            }

            if (currentShopCurrency <= 0)
            {
                yield return null;
                continue;
            }

            if (isSpawnDelayed)
            {
                yield return new WaitForSeconds(1f);
                isSpawnDelayed = false;
            }

            SpawnEnemy();

            yield return new WaitForSeconds(spawnTimerMax);
        }
    }

    private void SpawnEnemy() 
    {
        CalculateWeight();
        randomValue = Random.Range(0f, 1f);
        cumalativeWeight = 0f;
        spawnLocation = Random.Range(1, 5);

        foreach (var enemy in enemies) 
        {
            cumalativeWeight += enemy.weightNormalized;

            if (randomValue < cumalativeWeight && currentShopCurrency >= enemy.enemyCost) 
            {
                switch (spawnLocation)
                {
                    case 1:
                        enemiesSpawned.Add(Instantiate(enemy.prefab, islandManager.EnemySpawnPoint1.transform));
                        break;
                    case 2:
                        enemiesSpawned.Add(Instantiate(enemy.prefab, islandManager.EnemySpawnPoint2.transform));
                        break;
                    case 3:
                        enemiesSpawned.Add(Instantiate(enemy.prefab, islandManager.EnemySpawnPoint3.transform));
                        break;
                    case 4:
                        enemiesSpawned.Add(Instantiate(enemy.prefab, islandManager.EnemySpawnPoint4.transform));
                        break;
                    default:
                        break;
                }
                currentShopCurrency -= enemy.enemyCost;
                break;
            }
        }
    } 

    private void RemoveDeadEnemies()
    {
        foreach(GameObject enemy in new List<GameObject>(enemiesSpawned))
        {
            if(enemy == null) enemiesSpawned.Remove(enemy);
        }
    }

    private void ClearWave()
    {
        currentShopCurrency = 0;

        foreach (GameObject enemy in new List<GameObject>(enemiesSpawned))
        {
            Destroy(enemy);
        }
        enemiesSpawned.Clear();

        CanSpawn = false;
    }

    public void WaveReset() 
    {
        maxShopCurrency = baseCurrency + (growthFactor * Mathf.Pow(islandManager.level,polynomialDegree));
        currentShopCurrency = maxShopCurrency;

        CanSpawn = true;
        isSpawnDelayed = true;
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
