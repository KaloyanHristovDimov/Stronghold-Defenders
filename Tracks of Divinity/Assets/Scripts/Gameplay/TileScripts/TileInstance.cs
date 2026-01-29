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
        WaveManager.Instance?.RefreshAllEndpoints();
        if (GetComponentInChildren<EnemyNavigation>() != null) 
        {
            GetComponentInChildren<EnemyNavigation>().SetupNavigation(tileData);
        }
        
        if(!spawningStartingTile) foreach(var sp in transform.Find("default").GetChild(0).GetComponentsInChildren<TowerSpawn>())
            sp.Initialize((int)tileData.tileBiome);


    }
}
