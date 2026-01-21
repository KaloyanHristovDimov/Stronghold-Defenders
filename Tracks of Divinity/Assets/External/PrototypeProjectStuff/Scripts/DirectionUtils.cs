using UnityEngine;

public class DirectionUtils : MonoBehaviour
{
    public static TileData.Direction Opposite(TileData.Direction dir)
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

    public static Vector2Int ToVector(TileData.Direction dir)
    {
        switch (dir)
        {
            case TileData.Direction.Top: return Vector2Int.up;
            case TileData.Direction.Bottom: return Vector2Int.down;
            case TileData.Direction.Left: return Vector2Int.left;
            case TileData.Direction.Right: return Vector2Int.right;
            default: return Vector2Int.zero;
        }
    }
}
