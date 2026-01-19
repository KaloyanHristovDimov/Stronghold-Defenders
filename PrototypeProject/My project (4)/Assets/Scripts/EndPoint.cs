using UnityEngine;
using UnityEngine.EventSystems;


public class EndPoint : MonoBehaviour, IPointerDownHandler
{
    public TileData.Direction direction;

    public TileInstance parentTile;

    private void Start()
    {
        parentTile = GetComponentInParent<TileInstance>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        TileManager.Instance.TrySpawnTile(parentTile, direction);
        gameObject.SetActive(false);
    }
}
