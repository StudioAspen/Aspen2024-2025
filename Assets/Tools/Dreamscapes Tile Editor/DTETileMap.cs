using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dreamscapes.TileEditor
{
    ///-/////////////////////////////////////////////////////////////////////////
    /// 
    public enum BiomeType
    {
        Dreamscape = 0,
        Lava = 1,
    }
    
    ///-/////////////////////////////////////////////////////////////////////////
    ///
    [SelectionBase]
    public class DTETileMap : MonoBehaviour
    {
        [SerializeField] private SODTEBiomeManager _biomeManager;
        public SODTEBiomeManager biomeManager => _biomeManager;
        
        // Stores the TileMap's BiomeType
        [SerializeField] private BiomeType _biomeType = BiomeType.Dreamscape;
        public BiomeType biomeType => _biomeType;
        
        // Stores the TileMap's Biome Seed
        [SerializeField] private int _biomeSeed = 0;
        public int biomeSeed => _biomeSeed;
        
        // Stores the TileMap's Grid Size
        [SerializeField] private Vector2Int _gridSize = new Vector2Int(1, 1);
        public Vector2Int gridSize => _gridSize;
        
        // A child object that contains all the tiles
        [SerializeField] private Transform _tilesContainer;
        public Transform tilesContainer => _tilesContainer;
        
#if UNITY_EDITOR
        
        ///-/////////////////////////////////////////////////////////////////////////
        /// 
        public void SetBiomeType(BiomeType newBiomeType)
        {
            _biomeType = newBiomeType;
            
            ResetBiomeTiles();
        }

        ///-/////////////////////////////////////////////////////////////////////////
        ///
        public void SetBiomeSeed(int newBiomeSeed)
        {
            _biomeSeed = newBiomeSeed;

            ResetBiomeTiles();
        }

        ///-/////////////////////////////////////////////////////////////////////////
        ///
        public void SetGridSize(Vector2Int newGridSize)
        {
            _gridSize = newGridSize;
            
            ResetBiomeTiles();
        }
        
        ///-/////////////////////////////////////////////////////////////////////////
        ///
        public SODTEBiome GetBiome()
        {
            return biomeManager.biomes[(int)_biomeType];
        }
        
        ///-/////////////////////////////////////////////////////////////////////////
        ///
        private void ResetBiomeTiles()
        {
            ClearTiles();
            
            SODTEBiome biome = biomeManager.biomes[(int)_biomeType];
            Random.InitState(_biomeSeed);

            GameObject[] tiles = biome.GetTileVariations();
            
            Vector2 tileSize = biomeManager.tileSize;
            Vector2 tilePosition = tileSize / 2;
            Vector2 tileSpawnOrigin = new Vector2(-(tilePosition.x * (_gridSize.x - 1)), -(tilePosition.y * (_gridSize.y - 1)));
            
            for (int x = 0; x < _gridSize.x; x++)
            {
                for (int y = 0; y < _gridSize.y; y++)
                {
                    // Get random tile prefab
                    GameObject randomTilePrefab = tiles[Random.Range(0, tiles.Length)];
                    GameObject newTile = (GameObject)PrefabUtility.InstantiatePrefab(randomTilePrefab, tilesContainer.transform);

                    // Set the tile's position
                    float xPos = tileSpawnOrigin.x + x * tileSize.x;
                    float yPos = tileSpawnOrigin.y + y * tileSize.y;
                    newTile.transform.localPosition = new Vector3(xPos, 0, yPos);
                }
            }
        }

        ///-/////////////////////////////////////////////////////////////////////////
        ///
        /// Clear the tiles container or create a new one if it does not exist
        /// 
        private void ClearTiles()
        {
            if (_tilesContainer == null)
            {
                // Cache TilesContainer if null
                DTETilesContainer container = transform.GetComponentInChildren<DTETilesContainer>();
                if (container == null)
                {
                    // Create new TilesContainer object if null
                    _tilesContainer = new GameObject("TilesContainer").transform;
                    _tilesContainer.gameObject.AddComponent<DTETilesContainer>();
                    _tilesContainer.parent = transform;
                    _tilesContainer.localPosition = Vector3.zero;
                }
                else
                {
                    _tilesContainer = container.transform;
                }
            }

            List<Transform> tiles = new List<Transform>();
            foreach (Transform tile in tilesContainer)
            {
                tiles.Add(tile);
            }

            foreach (Transform tile in tiles)
            {
                DestroyImmediate(tile.gameObject);
            }
        }

#endif // UNITY_EDITOR
        
    }
}