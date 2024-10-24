using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using KBCore.Refs;

public class WorldManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Scene] private MasterLevelManager masterLevelManager;

    [Header("Settings")]
    public bool IsSelecting;
    private int activeLandCount;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    void Update(){}

    private void Start()
    {
        activeLandCount = 1;
    }

    private bool AreAllWavesFinished()
    {
        bool finished = true;

        foreach(IslandManager island in masterLevelManager.SpawnedIslands)
        {
            EnemySpawner enemyManager = island.EnemySpawner;

            if (!enemyManager.IsWaveFinished) finished = false;
        }

        return finished;
    }

    public void PrepareForNextWave() 
    {
        activeLandCount = masterLevelManager.SpawnedIslands.Count;
        foreach (IslandManager island in masterLevelManager.SpawnedIslands) 
        {
            island.EnemySpawner.WaveReset();
        }
    }

    public void DecrementActiveLandCount()
    {
        activeLandCount--;
        if (activeLandCount == 0)
        {
            IsSelecting = true;
            FindObjectOfType<IslandSelectUI>().PrepareIslandSelection();
        }
    }

}
