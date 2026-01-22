using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "TileData", menuName = "ScriptableObjects/TileData", order = 1)]
public class TileData : ScriptableObject
{
    public enum Direction
    {
        Top,
        Bottom,
        Right,
        Left
    }

    public Direction startDirection;          // Where the tile "starts"
    public List<Direction> endDirections;     // Where the tile "ends"
    public GameObject prefab;                 // Tile prefab
    public GameObject icon;                       // Optional, for UI previews
    public int towerAmount;               // Number of towers on this tile
    public enum biomeType
    {
        Plains,
        Desert,
        Snow,
        Volcanic
    }
    public biomeType tileBiome;
}
