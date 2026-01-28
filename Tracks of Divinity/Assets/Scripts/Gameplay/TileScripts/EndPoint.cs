using UnityEngine;
using UnityEngine.EventSystems;

public class EndPoint : MonoBehaviour
{
    public TileData.Direction direction;
    public TileInstance parentTile;

    [Header("Visuals")]
    [SerializeField] public GameObject placeTileInteractable;
    [SerializeField] public GameObject sealInteractable;

    private bool interactable = true;

    public bool sealedOff = false;


    private void Start()
    {
        parentTile = GetComponentInParent<TileInstance>();
        WaveManager.Instance?.RefreshAllEndpoints();
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

    public void SetInteractable(bool allowInteraction)
    {
        if (sealedOff || !allowInteraction)
        {
            placeTileInteractable.SetActive(false);
            sealInteractable.SetActive(false);
            return;
        }

        if (IsBlocked())
        {
            placeTileInteractable.SetActive(false);
            sealInteractable.SetActive(true);
        }
        else
        {
            placeTileInteractable.SetActive(true);
            sealInteractable.SetActive(false);
        }
    }

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    if (!interactable)
    //        return;

    //    TileManager.Instance.TrySpawnTile(parentTile, direction);
    //    gameObject.SetActive(false);
    //}

    public bool IsBlocked()
    {
        if (parentTile == null)
            return true;

        Vector2Int targetPos =
            parentTile.gridPosition + DirectionUtils.ToVector(direction);

        return GridManager.Instance.IsOccupied(targetPos);
    }

    public void OnPlaceTilePressed()
    {
        TileManager.Instance.TrySpawnTile(parentTile, direction);
        gameObject.SetActive(false);
    }

    public void OnSealPressed()
    {
        if (sealedOff)
            return;

        if (WaveManager.Instance.TrySealEndpoint(this))
        {
            sealedOff = true;
            placeTileInteractable.SetActive(false);
            sealInteractable.SetActive(false);
        }
    }
}
