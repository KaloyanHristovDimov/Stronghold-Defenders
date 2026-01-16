using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;

    public List<TileData> allTiles;

    private void Awake()
    {
        Instance = this;
    }

    public void TrySpawnTile(TileInstance sourceTile, TileData.Direction exitDir)
    {
        Vector2Int spawnPos = sourceTile.gridPosition + DirectionUtils.ToVector(exitDir);

        if (GridManager.Instance.IsOccupied(spawnPos))
            return;

        TileData.Direction requiredStart =
            DirectionUtils.Opposite(exitDir);

        List<TileData> validTiles = allTiles.Where(tile =>
            tile.startDirection == requiredStart &&
            EndpointsAreFree(tile, spawnPos)
        ).ToList();

        if (validTiles.Count == 0)
            return;

        SpawnTile(validTiles[Random.Range(0, validTiles.Count)], spawnPos);
    }

    bool EndpointsAreFree(TileData tile, Vector2Int gridPos)
    {
        foreach (var end in tile.endDirections)
        {
            Vector2Int checkPos = gridPos + DirectionUtils.ToVector(end);
            if (GridManager.Instance.IsOccupied(checkPos))
                return false;
        }
        return true;
    }

    void SpawnTile(TileData tileData, Vector2Int gridPos)
    {
        GameObject obj = Instantiate(tileData.prefab);

        TileInstance instance = obj.AddComponent<TileInstance>();
        instance.Initialize(tileData, gridPos);
    }





    //private TileData GetCompatibleTile(TileData.Direction lastEnd)
    //{
    //    TileData.Direction neededStart = GetOppositeDirection(lastEnd);

    //    // Filter tiles whose start matches the needed start
    //    List<TileData> compatibleTiles = allTiles.FindAll(tile => tile.startDirection == neededStart);

    //    if (compatibleTiles.Count == 0) return null;

    //    // Pick one randomly
    //    int index = Random.Range(0, compatibleTiles.Count);
    //    return compatibleTiles[index];
    //}

    //private TileData.Direction GetOppositeDirection(TileData.Direction dir)
    //{
    //    switch (dir)
    //    {
    //        case TileData.Direction.Top: return TileData.Direction.Bottom;
    //        case TileData.Direction.Bottom: return TileData.Direction.Top;
    //        case TileData.Direction.Left: return TileData.Direction.Right;
    //        case TileData.Direction.Right: return TileData.Direction.Left;
    //        default: return dir;
    //    }
    //}
}
