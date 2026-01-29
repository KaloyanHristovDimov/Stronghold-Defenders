using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TowerType type;
    [SerializeField] private Image icon;
    [SerializeField] private GameObject cover, dragCover;

    private bool isDragging;

    public void OnBeginDrag(PointerEventData e)
    {
        isDragging = true;
        Persister.SetCursor(true);
        dragCover.SetActive(true);
        UICanvasController.currentTowerButton = this;
        UICanvasController.ActivateTowerSpawnPoints(true);
        UICanvasController.DraggedIcon.Activate(icon.sprite);
    }

    public void OnDrag(PointerEventData e)
    {
        //removing this method breaks everything
    }

    public void OnEndDrag(PointerEventData e)
    {
        if(cover.activeSelf) cover.SetActive(false);
        if(UICanvasController.currentTowerSpawnPoint != null) UICanvasController.currentTowerSpawnPoint.TrySpawnTower();

        isDragging = false;
        Persister.SetCursor();
        dragCover.SetActive(false);
        UICanvasController.currentTowerButton = null;
        UICanvasController.ActivateTowerSpawnPoints();
        UICanvasController.DraggedIcon.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cover.SetActive(true);
        ShowCard(0, (int)type); //0 is Plains
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!isDragging) cover.SetActive(false);
        HideCard();
    }

    public void HideCard() => UICanvasController.MainTowerCard.gameObject.SetActive(false);

    public void ShowCard(int biome, int type)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UICanvasController.canvasRectT, Input.mousePosition, null, out var mousePos);
        UICanvasController.towerCardRectT.anchoredPosition = new(mousePos.x, mousePos.y);

        UICanvasController.MainTowerCard.Display(UICanvasController.Towers[biome].pair[type].tower, icon.sprite);
        UICanvasController.MainTowerCard.gameObject.SetActive(true);
    }
}
