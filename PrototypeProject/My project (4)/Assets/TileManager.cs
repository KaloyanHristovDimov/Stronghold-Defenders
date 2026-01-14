using UnityEngine;
using System.Collections.Generic;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance;

    public List<TileData> allTiles;           // All available tiles
    public Transform lastTileEndPoint;        // Transform at the end of the last tile
    public TileData.Direction lastTileEndDirection;  // Last tile's end direction



    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    //public void SpawnTile(Vector3 position, Quaternion rotation, TileType type)
    //{
    //    GameObject prefab = GetRandomPrefab(type);
    //    Instantiate(prefab, position, rotation);
    //}

    //private GameObject GetRandomPrefab(TileType type)
    //{
    //    List<GameObject> list = null;

    //    switch (type)
    //    {
    //        case TileType.Straight:
    //            list = straightTiles;
    //            break;
    //        case TileType.LeftTurn:
    //            list = leftTurnTiles;
    //            break;
    //        case TileType.RightTurn:
    //            list = rightTurnTiles;
    //            break;
    //    }

    //    if (list == null || list.Count == 0)
    //    {
    //        Debug.LogError($"No tiles assigned for {type}");
    //        return null;
    //    }

    //    return list[Random.Range(0, list.Count)];
    //}



    private TileData GetCompatibleTile(TileData.Direction lastEnd)
    {
        TileData.Direction neededStart = GetOppositeDirection(lastEnd);

        // Filter tiles whose start matches the needed start
        List<TileData> compatibleTiles = allTiles.FindAll(tile => tile.startDirection == neededStart);

        if (compatibleTiles.Count == 0) return null;

        // Pick one randomly
        int index = Random.Range(0, compatibleTiles.Count);
        return compatibleTiles[index];
    }

    private TileData.Direction GetOppositeDirection(TileData.Direction dir)
    {
        switch (dir)
        {
            case TileData.Direction.Top: return TileData.Direction.Bottom;
            case TileData.Direction.Bottom: return TileData.Direction.Top;
            case TileData.Direction.Left: return TileData.Direction.Right;
            case TileData.Direction.Right: return TileData.Direction.Left;
            default: return dir;
        }
    }
}
