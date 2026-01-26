using UnityEngine;

public class TileInstance : MonoBehaviour
{
    public TileData tileData;
    public Vector2Int gridPosition;

    public void Initialize(TileData data, Vector2Int gridPos, bool spawningStartingTile)
    {
        tileData = data;
        gridPosition = gridPos;

        transform.position = GridManager.Instance.GridToWorld(gridPos);
        GridManager.Instance.RegisterTile(gridPos, this);
        if (GetComponentInChildren<EnemyNavigation>() != null) 
        {
            GetComponentInChildren<EnemyNavigation>().SetupNavigation(tileData);
        }
        
        // Some error gets thrown on the below row (I think while dragging on the choice window over a tile)
        // The row itself is not faulty so don't touch it aside from gates/ifs
        if(!spawningStartingTile) foreach(var sp in transform.Find("default").GetChild(0).GetComponentsInChildren<TowerSpawn>()) sp.Initialize();
    }
}
