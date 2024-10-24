using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Scene] private WorldManager worldManager;
    [SerializeField, Self] private IslandManager islandManager;
    [SerializeField, Anywhere] private List<Enemy> enemyPrefabs = new List<Enemy>();
    private List<float> enemyNormalizedWeights = new List<float>();

    [Header("Settings")]
    [SerializeField] private float weightingSkewFactor = 2.2f;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float baseCurrency;
    [SerializeField] private float growthFactor;
    [SerializeField] private int polynomialDegree;
    private float maxShopCurrency;
    private float currentShopCurrency;
    public bool IsWaveFinished => currentShopCurrency <= 0 && enemiesSpawned.Count <= 0;

    [HideInInspector] public bool CanSpawn = false;
    private List<Enemy> enemiesSpawned = new List<Enemy>();
    private bool isSpawnDelayed = false;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        CalculateNormalizedWeights();
    }

    void Start()
    {
        maxShopCurrency = baseCurrency + (growthFactor * Mathf.Pow(islandManager.Level,polynomialDegree));
        currentShopCurrency = maxShopCurrency;
      
        StartCoroutine(SpawnCoroutine());
    }


    void Update()
    {

        if (currentShopCurrency <= 0) CanSpawn = false;

        if (Input.GetKeyDown(KeyCode.K)) ClearWave();
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
                yield return new WaitForSeconds(2f);
                isSpawnDelayed = false;
            }

            SpawnEnemy();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy() 
    {
        float randomValue = Random.Range(0f, 1f);
        int spawnLocation = Random.Range(1, 5);
        float cumalativeWeight = 0f;

        for(int i = 0; i < enemyPrefabs.Count; i++)
        {
            cumalativeWeight += enemyNormalizedWeights[i];

            if (currentShopCurrency < enemyPrefabs[i].Cost) continue;

            if(randomValue < cumalativeWeight)
            {

                Enemy e = Instantiate(enemyPrefabs[i], islandManager.GetRandomEnemySpawn().position, Quaternion.identity);
                e.Init(this);
                enemiesSpawned.Add(e);

                currentShopCurrency -= enemyPrefabs[i].Cost;

                break;
            }
        }
    } 

    private void ClearWave()
    {
        currentShopCurrency = 0;

        foreach (Enemy enemy in new List<Enemy>(enemiesSpawned))
        {
            Destroy(enemy.gameObject);
        }
        enemiesSpawned.Clear();

        CanSpawn = false;
        worldManager.DecrementActiveLandCount();
    }

    public void WaveReset() 
    {
        if (islandManager.Level <= 0)
        {
            CanSpawn = false;
            maxShopCurrency = 0;
            currentShopCurrency = 0;
            return;
        }

        maxShopCurrency = baseCurrency + (growthFactor * Mathf.Pow(islandManager.Level,polynomialDegree));
        currentShopCurrency = maxShopCurrency;

        CanSpawn = true;
        isSpawnDelayed = true;
    }

    private void CalculateNormalizedWeights() 
    {
        enemyNormalizedWeights = new List<float>();

        float totalWeight = 0f;

        foreach (Enemy enemy in enemyPrefabs) 
        {
            totalWeight += 1f / Mathf.Pow(enemy.Cost, weightingSkewFactor);
        }

        for(int i = 0; i < enemyPrefabs.Count; i++)
        {
            float weight = 1f / Mathf.Pow(enemyPrefabs[i].Cost, weightingSkewFactor);

            enemyNormalizedWeights.Add(weight / totalWeight);
        }
    }

    public void RemoveEnemyFromList(Enemy e)
    {
        enemiesSpawned.Remove(e);
        if(enemiesSpawned.Count == 0)
        {
            worldManager.DecrementActiveLandCount();
        }
    }

}
