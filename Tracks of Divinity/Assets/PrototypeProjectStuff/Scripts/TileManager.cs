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

        //SpawnTile(validTiles[Random.Range(0, validTiles.Count)], spawnPos);
        List<TileData> options = PickRandomTiles(validTiles, 3);

        TileChoice.Instance.ShowChoices(
            options,
            choice => SpawnTile(choice, spawnPos));
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

    List<TileData> PickRandomTiles(List<TileData> source, int count)
    {
        return source
            .OrderBy(_ => Random.value)
            .Take(Mathf.Min(count, source.Count))
            .ToList();
    }

    void SpawnTile(TileData tileData, Vector2Int gridPos)
    {
        GameObject obj = Instantiate(tileData.prefab);

        TileInstance instance = obj.AddComponent<TileInstance>();
        instance.Initialize(tileData, gridPos);
    }

}
