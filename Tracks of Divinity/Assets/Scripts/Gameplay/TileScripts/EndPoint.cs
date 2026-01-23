using UnityEngine;
using UnityEngine.EventSystems;

public class EndPoint : MonoBehaviour, IPointerDownHandler
{
    public TileData.Direction direction;
    public TileInstance parentTile;

    private Collider col;
    private bool interactable = true;

    private void Awake()
    {
        col = GetComponent<Collider>();
    }

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
        if (WaveManager.Instance != null)
            WaveManager.Instance.UnregisterEndpoint(this);
    }

    public void SetInteractable(bool value)
    {
        interactable = value;

        if (col != null)
            col.enabled = value;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!interactable)
            return;

        TileManager.Instance.TrySpawnTile(parentTile, direction);
        gameObject.SetActive(false);
    }
}
