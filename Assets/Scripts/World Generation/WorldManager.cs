using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldManager : MonoBehaviour
{
    private MasterLevelManager masterLevelManager;

    [Header("Misc Controls")]
    [SerializeField] public bool isInSkyView;

    [Header("Island Selection")]
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject islandSelectCamera;
    public bool IsSelecting;

    private void Awake()
    {
        masterLevelManager = FindAnyObjectByType<MasterLevelManager>();
    }

    void Update()
    {
        if (AreAllWavesFinished() && !IsSelecting)
        {
            IsSelecting = true;

            masterLevelManager.SpawnSelectionSpheres();
        }
      
/*        if (Input.GetKeyDown(KeyCode.Q) && isInSkyView == false) 
        {
            islandTimertext.SetActive(true);
            islandSelectCamera.SetActive(true);
            playerCamera.SetActive(false);
            isInSkyView = true;
        }

        if (Input.GetKeyDown(KeyCode.E) && isInSkyView == true)
        {
           
            islandSelectCamera.SetActive(false);
            playerCamera.SetActive(true);
            islandTimertext.SetActive(false);
            isInSkyView = false;
        }*/
    }

    private bool AreAllWavesFinished()
    {
        bool finished = true;

        foreach(IslandManager island in masterLevelManager.SpawnedIslands)
        {
            EnemyManager enemyManager = island.EnemyManager;

            if (!enemyManager.IsWaveFinished) finished = false;
        }

        return finished;
    }

    public void PrepareForNextWave() 
    {
        foreach (IslandManager island in masterLevelManager.SpawnedIslands) 
        {
            island.LevelUp();
            island.EnemyManager.WaveReset();
        }
    }
}
