using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;

    private int tilesWithoutSplit = 0;

    [System.Serializable]
    public class GameObjectList
    {
        public string Biome;
        public List<TileData> Tiles;
    }

    public List<GameObjectList> listOfBiomesNormal;
    public List<GameObjectList> listOfBiomesSplit;
    private List<TileData> allNormalTiles = new List<TileData>();
    private List<TileData> allSplitTiles = new List<TileData>();
    public TileData StartingTile;

    private bool spawningStartingTile = false;

    private void Awake()
    {
        Instance = this;

        foreach (var item in listOfBiomesNormal)
            foreach (var t in item.Tiles)
                allNormalTiles.Add(t);

        foreach (var item in listOfBiomesSplit)
            foreach (var t in item.Tiles)
                allSplitTiles.Add(t);
    }

    private void Start()
    {
        // Starting tile should NOT start wave 1.
        spawningStartingTile = true;
        SpawnTile(StartingTile, Vector2Int.zero);
        spawningStartingTile = false;
    }

    public void TrySpawnTile(TileInstance sourceTile, TileData.Direction exitDir)
    {
        // NEW: hard lock tile placement when wave is active OR countdown is running
        if (WaveManager.Instance != null && !WaveManager.Instance.CanPlaceTile)
            return;

        List<TileData> tilesToUse;

        if (tilesWithoutSplit >= 4)
        {
            tilesToUse = allSplitTiles;
            tilesWithoutSplit = 0;
        }
        else
        {
            tilesToUse = allNormalTiles;
            tilesWithoutSplit++;
        }

        Vector2Int spawnPos = sourceTile.gridPosition + DirectionUtils.ToVector(exitDir);

        if (GridManager.Instance.IsOccupied(spawnPos))
            return;

        TileData.Direction requiredStart = DirectionUtils.Opposite(exitDir);

        List<TileData> validTiles = tilesToUse.Where(tile =>
            tile.startDirection == requiredStart &&
            EndpointsAreFree(tile, spawnPos)
        ).ToList();

        if (validTiles.Count == 0)
            return;

        List<TileData> options = PickRandomTiles(validTiles, 3);
        ChoiceWindow.Instance.Open(options, choice => SpawnTile(choice, spawnPos));
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

        // Player-placed tiles (NOT the starting tile) start the next wave.
        if (!spawningStartingTile && WaveManager.Instance != null)
            WaveManager.Instance.NotifyTilePlaced_StartNextWave();
    }
}
