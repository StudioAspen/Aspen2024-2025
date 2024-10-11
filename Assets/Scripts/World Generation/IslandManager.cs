using DG.Tweening;
using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class IslandManager : MonoBehaviour
{
    public Vector2Int GridPosition { get; private set; }
    [field: SerializeField, Self] public EnemyManager EnemyManager { get; private set; }
    [SerializeField, Self] private NavMeshSurface navMeshSurface;

    [Header("Square Stats")]
    private MasterLevelManager masterLevelManager;
    [SerializeField] public int sqaureLevel;
    private WorldManager worldManager;

    [SerializeField] LayerMask WallLayerMask;

    [SerializeField] private IslandBorder[] borders;

    [field: SerializeField] public GameObject EnemySpawnPoint1 { get; private set; }
    [field: SerializeField] public GameObject EnemySpawnPoint2 { get; private set; }
    [field: SerializeField] public GameObject EnemySpawnPoint3 { get; private set; }
    [field: SerializeField] public GameObject EnemySpawnPoint4 { get; private set; }

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        masterLevelManager = FindObjectOfType<MasterLevelManager>();
        worldManager = FindObjectOfType<WorldManager>();
    }

    void Start()
    {
        StartCoroutine(UpdateNavMesh());
        sqaureLevel = 1;

        InitializeBorders();

        transform.DOMoveY(-5, 1f).SetEase(Ease.InBounce);
    }

    
    void FixedUpdate()
    {
        Ray rayFront = new Ray(transform.position, transform.TransformDirection(Vector3.forward));
        Ray rayRight = new Ray(transform.position, transform.TransformDirection(Vector3.right));
        Ray rayLeft = new Ray(transform.position, transform.TransformDirection(Vector3.left));
        Ray rayBack = new Ray(transform.position, transform.TransformDirection(Vector3.back));

        
    }

    private void InitializeBorders()
    {
        foreach(IslandBorder border in borders)
        {
            border.SetWorldBorderPosition(GridPosition);
            masterLevelManager.AddBorder(border);
        }
    }

    public IEnumerator UpdateNavMesh() 
    {
       
        navMeshSurface.BuildNavMesh();

        yield return new WaitForSeconds(3);

        navMeshSurface.enabled = false;
    }

    public void LevelUp()
    {

        sqaureLevel += 1;

    }

    public void Init(int x, int y)
    {
        GridPosition = new Vector2Int(x, y);
    }
}
    