using DG.Tweening;
using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class IslandManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Scene] private WorldManager worldManager;
    [field: SerializeField, Self] public EnemySpawner EnemySpawner { get; private set; }
    [SerializeField, Anywhere] private MasterLevelManager masterLevelManager;
    [field: SerializeField] public Vector2Int GridPosition { get; private set; }
    [SerializeField, Self] private NavMeshSurface navMeshSurface;
    [SerializeField] private IslandBorder[] borders;
    [SerializeField] private List<Transform> enemySpawnPoints;

    [field: Header("Settings")]
    [field:SerializeField] public int Level { get; private set; }

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        masterLevelManager = GetComponentInParent<MasterLevelManager>();
    }

    void Start()
    {
        Level = 1;

        InitializeBorders();

        transform.DOMoveY(-5, 0.5f).SetEase(Ease.InBounce).OnComplete(()=>StartCoroutine(OnCompleteSpawn()));
    }

    private IEnumerator OnCompleteSpawn()
    {
        masterLevelManager.RemoveConnectedBorders();

        yield return null;

        masterLevelManager.BuildNavMesh();

        EnemySpawner.CanSpawn = true;
    }

    private void InitializeBorders()
    {
        foreach(IslandBorder border in borders)
        {
            border.SetWorldBorderPosition(GridPosition);
            masterLevelManager.AddBorder(border);
        }
    }

    public void LevelUp()
    {
        Level += 1;
    }

    public void Init(int x, int y)
    {
        GridPosition = new Vector2Int(x, y);
    }

    public Transform GetRandomEnemySpawn()
    {
        int randomIndex = Random.Range(0, enemySpawnPoints.Count);
        return enemySpawnPoints[randomIndex];
    }
}
    