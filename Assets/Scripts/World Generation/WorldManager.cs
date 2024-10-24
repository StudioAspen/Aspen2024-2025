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

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    void Update()
    {
        if (AreAllWavesFinished() && !IsSelecting)
        {
            IsSelecting = true;

            FindObjectOfType<IslandSelectUI>().PrepareIslandSelection();
        }
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
        foreach (IslandManager island in masterLevelManager.SpawnedIslands) 
        {
            island.LevelUp();
            island.EnemySpawner.WaveReset();
        }
    }
}
