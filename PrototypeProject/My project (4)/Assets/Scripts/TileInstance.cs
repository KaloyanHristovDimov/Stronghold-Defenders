using UnityEngine;

public class TileInstance : MonoBehaviour
{
    public TileData tileData;
    public Vector2Int gridPosition;

    public void Initialize(TileData data, Vector2Int gridPos)
    {
        tileData = data;
        gridPosition = gridPos;

        transform.position = GridManager.Instance.GridToWorld(gridPos);
        GridManager.Instance.RegisterTile(gridPos, this);
    }
}
