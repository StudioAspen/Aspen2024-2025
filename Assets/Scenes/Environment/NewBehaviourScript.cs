using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [Header("Item Types")]
    [SerializeField]
    private GameObject itemPrefab;
    [Space(2)]

    [Header("Spawn System")]
    [SerializeField]
    private float spawnTimer = 0f;
    [SerializeField]
    private Tilemap tileMap;
    [SerializeField]
    private List<Vector3> stockTiles;

    void Start()
    {
        FindLocationsOfTiles();
    }

    private void FindLocationsOfTiles()
    {
        stockTiles = new List<Vector3>(); 

       for (int x = tileMap.cellBounds.xMin; x < tileMap.cellBounds.xMax; x++) 
        {
          for (int p = tileMap.cellBounds.yMin; p < tileMap.cellBounds.yMax; p++) 
           {
              Vector3Int localTiles = new Vector3Int(x, p, (int)tileMap.transform.position.y); 
              Vector3 tiles = tileMap.CellToWorld(localTiles); 
          
          if (tileMap.HasTile(localTiles))
          {
             stockTiles.Add(tiles);
          }

          else{}

          }

       }
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        InvokeRepeating("SpawnItem", spawnTimer, 0.05f);


    }

    private void SpawnItem()
    {
        if (spawnTimer > 2)
        {
          for (int i = 0; i < stockTiles.Count; i++)
           {
            // Added 0.5f units to the bottom left and spawn prefab at the vector's location, at the availablePlaces location.
            // of the CELL (square) is (0,0), and the upper right corner is (1,1). (0.5,0.5) is the center.
            Instantiate(itemPrefab, new Vector3(stockTiles[i].x + 0.5f, stockTiles[i].y + 0.5f, stockTiles[i].z), Quaternion.identity);
           }
        spawnTimer = 0f;
        }
    }
}