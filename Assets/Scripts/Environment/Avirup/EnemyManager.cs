using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float maxShopCurrency;
    public float currentShopCurrency;

    // Property to check if the wave is finished (no currency and no enemies left)
    public bool IsWaveFinished => currentShopCurrency <= 0 && enemiesSpawned.Count <= 0;

    public float spawnTimerMax; // Maximum time between enemy spawns
    public float spawnTimer;
    public bool spawnRdy;
    public int spawnLocation;
    public IslandManager islandManager;

    // Spawn thresholds for different enemy types
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
    public float RateScale; // Controls how enemy cost affects spawn rate
    [Header("Currency calculation")]
    [SerializeField] public float baseCurrency;
    [SerializeField] public float growthFactor;
    [SerializeField] public int polynomialDegree;

    public bool CanSpawn = false;
    private List<GameObject> enemiesSpawned = new List<GameObject>();
    private bool isSpawnDelayed = false; // Flag to delay spawning at the start of a wave
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
        // Initialization
        spawnTimerMax = 3;
        WorldManager = GameObject.FindObjectOfType<WorldManager>();
        islandManager = GetComponent<IslandManager>();

        // Calculate initial shop currency based on island level
        maxShopCurrency = baseCurrency + (growthFactor * Mathf.Pow(islandManager.level,polynomialDegree));
        currentShopCurrency = maxShopCurrency;
       
        // Add enemy types to the list with their associated costs
        enemies.Add(new Enemy { prefab = follower, enemyCost = followerCost });
        enemies.Add(new Enemy { prefab = leaper, enemyCost = leaperCost });
        enemies.Add(new Enemy { prefab = charger, enemyCost = chargerCost });

        StartCoroutine(SpawnCoroutine());
    }


    void Update()
    {
        // Remove dead enemies from the tracking list
        RemoveDeadEnemies();

        // Stop spawning if no currency left
        if (currentShopCurrency <= 0) CanSpawn = false;

        // Cheat key to clear the current wave
        if (Input.GetKeyDown(KeyCode.K))
        {
            ClearWave();
        }
    }

    // Coroutine to handle enemy spawning
    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            // Don't spawn if spawning is disabled
            if (!CanSpawn)
            {
                yield return null;
                continue;
            }

            // Don't spawn if no currency left
            if (currentShopCurrency <= 0)
            {
                yield return null;
                continue;
            }

            // Initial spawn delay at the start of a wave
            if (isSpawnDelayed)
            {
                yield return new WaitForSeconds(2f);
                isSpawnDelayed = false;
            }

            // Spawn an enemy
            SpawnEnemy();

            // Wait for the specified spawn interval
            yield return new WaitForSeconds(spawnTimerMax);
        }
    }

    private void SpawnEnemy() 
    {
        // Calculate spawn weights based on enemy costs
        CalculateWeight();

        // Generate random value for weighted selection
        randomValue = Random.Range(0f, 1f);
        cumalativeWeight = 0f;

        // Choose a random spawn location
        spawnLocation = Random.Range(1, 5);

        // Iterate through enemies and spawn one based on weighted probability and available currency
        foreach (var enemy in enemies) 
        {
            cumalativeWeight += enemy.weightNormalized;

            if (randomValue < cumalativeWeight && currentShopCurrency >= enemy.enemyCost) 
            {
                // Instantiate the chosen enemy at the selected spawn point
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

                // Deduct the enemy's cost from the currency
                currentShopCurrency -= enemy.enemyCost;
                break;
            }
        }
    } 

    private void RemoveDeadEnemies()
    {
        // Iterate through a copy of the list to avoid modification during iteration
        foreach(GameObject enemy in new List<GameObject>(enemiesSpawned))
        {
            if(enemy == null) enemiesSpawned.Remove(enemy);
        }
    }

    // Clears the current wave by destroying all enemies and resetting currency
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

    // Resets the wave by recalculating currency and enabling spawning
    public void WaveReset() 
    {
        maxShopCurrency = baseCurrency + (growthFactor * Mathf.Pow(islandManager.level,polynomialDegree));
        currentShopCurrency = maxShopCurrency;

        CanSpawn = true;
        isSpawnDelayed = true;
    }


    // Calculates the normalized weight for each enemy type based on its cost and the RateScale
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
