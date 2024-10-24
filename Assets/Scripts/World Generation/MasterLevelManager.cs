using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class MasterLevelManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Self] private NavMeshSurface navMeshSurface;
    private WorldManager worldManager;

    [field: Header("Grid")]
    public List<IslandManager> SpawnedIslands { get; private set; } = new List<IslandManager>();
    private List<IslandBorder> bordersList = new List<IslandBorder>();

    [Header("Player Selection")]
    [SerializeField] private LayerMask selectionLayer;
    [SerializeField] private IslandManager islandToSpawnPrefab;
    [SerializeField] private SelectionSphere selectionSpherePrefab;
    [SerializeField] private Vector3 selectorOffset;
    public List<SelectionSphere> CurrentSelectionSpheres { get; private set; } = new List<SelectionSphere>();

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        worldManager = FindObjectOfType<WorldManager>();
    }

    void Start()
    {
        SpawnedIslands.Add(FindObjectOfType<IslandManager>());

        BuildNavMesh();
    }


    void Update()
    {
        HandleMouseInput();
    }

    public IslandManager GetIsland(int x, int y)
    {
        foreach(IslandManager island in new List<IslandManager>(SpawnedIslands))
        {
            if(island.GridPosition.x == x && island.GridPosition.y == y) return island;
        }

        return null;
    }

    private SelectionSphere GetMouseLookSelectionSphere()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        bool didHit = Physics.Raycast(mouseRay, out hit, Mathf.Infinity, selectionLayer);

        if (!didHit) return null;

        return hit.transform.GetComponent<SelectionSphere>();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectionSphere selectionSphere = GetMouseLookSelectionSphere();

            if (selectionSphere == null) return;
            if (!selectionSphere.CanBeSelected) return;

            SpawnIsland(selectionSphere.DesiredIslandSpawnPosition.x, selectionSphere.DesiredIslandSpawnPosition.y);
            DeleteAllSelectionSpheres();

            worldManager.IsSelecting = false;
            worldManager.PrepareForNextWave();
        }
    }

    private void SpawnIsland(int x, int y)
    {
        float islandScale = islandToSpawnPrefab.transform.localScale.x;

        IslandManager spawnedIsland = Instantiate(islandToSpawnPrefab, new Vector3(islandScale * y, -15f, islandScale * x) , Quaternion.identity, transform);
        spawnedIsland.Init(x, y);

        SpawnedIslands.Add(spawnedIsland);

        FindObjectOfType<IslandSelectUI>().RemoveAllCards();
    }

    public void SpawnSelectionSpheres() 
    {
        DeleteAllSelectionSpheres();

        for (int i = 0; i < bordersList.Count; i++)
        {
            SelectionSphere newSphere = Instantiate(selectionSpherePrefab, bordersList[i].transform.position + selectorOffset, Quaternion.identity);
            newSphere.SetDesiredIslandSpawn(bordersList[i].WorldBorderPosition);
            CurrentSelectionSpheres.Add(newSphere);
        }

    }

    private void DeleteAllSelectionSpheres()
    {
        foreach(var sphere in new List<SelectionSphere>(CurrentSelectionSpheres))
        {
            Destroy(sphere.gameObject);
        }

        CurrentSelectionSpheres.Clear();
    }

    public void BuildNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }

    public void AddBorder(IslandBorder border)
    {
        bordersList.Add(border);
    }

    public void RemoveConnectedBorders()
    {
        foreach (IslandManager island in new List<IslandManager>(SpawnedIslands))
        {
            foreach (IslandBorder border in new List<IslandBorder>(bordersList))
            {
                if (island.GridPosition == border.WorldBorderPosition)
                {
                    Destroy(border.gameObject);
                    bordersList.Remove(border);
                }
            }
        }
    }
}
