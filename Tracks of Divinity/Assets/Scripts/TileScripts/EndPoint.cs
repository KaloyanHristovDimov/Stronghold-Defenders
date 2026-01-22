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
    private void OnEnable()
    {
        WaveManager.Instance.RegisterEndpoint(this);
    }

    private void OnDisable()
    {
        WaveManager.Instance.UnregisterEndpoint(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        TileManager.Instance.TrySpawnTile(parentTile, direction);
        gameObject.SetActive(false);

    }
}
