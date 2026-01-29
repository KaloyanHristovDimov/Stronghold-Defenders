using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    public float cellSize = 10f;

    [Header("Clouds")]
    public GameObject cloudPrefab;
    public int cloudGridSize = 1000;

    private Dictionary<Vector2Int, TileInstance> occupied = new Dictionary<Vector2Int, TileInstance>();
    private Dictionary<Vector2Int, CloudTile> clouds = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnCloudGrid();
    }

    void SpawnCloudGrid()
    {
        int half = cloudGridSize / 2;

        for (int x = -half; x < half; x++)
        {
            for (int y = -half; y < half; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                GameObject obj = Instantiate(cloudPrefab, transform);
                CloudTile cloud = obj.GetComponent<CloudTile>();
                cloud.Initialize(pos);

                clouds[pos] = cloud;
            }
        }
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

        if (clouds.TryGetValue(pos, out var placeholder))
        {
            Destroy(placeholder.gameObject);
            clouds.Remove(pos);
        }
    }

    public void RegisterPlaceholder(Vector2Int pos, CloudTile tile)
    {
        clouds[pos] = tile;
    }

}