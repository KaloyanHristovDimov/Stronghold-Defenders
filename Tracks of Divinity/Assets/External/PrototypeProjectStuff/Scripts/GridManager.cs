using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    public float cellSize = 10f;

    private Dictionary<Vector2Int, TileInstance> occupied = new Dictionary<Vector2Int, TileInstance>();
    private void Awake()
    {
        Instance = this;
    }


    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * cellSize, 0f, gridPos.y * cellSize);
    }

    public bool IsOccupied(Vector2Int pos)
    {
        return occupied.ContainsKey(pos);
    }

    public void RegisterTile(Vector2Int pos, TileInstance tile)
    {
        occupied[pos] = tile;
    }

}