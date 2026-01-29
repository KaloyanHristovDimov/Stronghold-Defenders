using UnityEngine;

public class CloudTile : MonoBehaviour
{
    public Vector2Int gridPosition;

    public void Initialize(Vector2Int pos)
    {
        gridPosition = pos;
        transform.position = GridManager.Instance.GridToWorld(pos);
    }
}
