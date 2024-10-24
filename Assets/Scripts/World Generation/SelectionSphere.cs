using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionSphere : MonoBehaviour
{
    public bool CanBeSelected { get; private set; } = false;

    [SerializeField] private float spawnDuration = 1f;
    [field: SerializeField] public Vector2Int DesiredIslandSpawnPosition { get; private set; }

    private void Start()
    {
        transform.DOMove(new Vector3(transform.position.x, 0f, transform.position.z), spawnDuration).SetEase(Ease.OutQuint).OnComplete(() => CanBeSelected = true);
    }

    public void SetDesiredIslandSpawn(Vector2Int spawnPos)
    {
        DesiredIslandSpawnPosition = spawnPos;
    }
}
