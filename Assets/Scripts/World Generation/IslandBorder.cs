using UnityEngine;

public class IslandBorder : MonoBehaviour
{
    [field: SerializeField] public Vector2Int RelativeBorderPosition { get; private set; }
    [field: SerializeField] public Vector2Int WorldBorderPosition { get; private set; }

    public void SetWorldBorderPosition(Vector2Int tileWorldPos)
    {
        WorldBorderPosition = tileWorldPos + RelativeBorderPosition;
    }
}
