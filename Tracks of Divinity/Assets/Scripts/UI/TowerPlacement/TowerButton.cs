using UnityEngine;
using UnityEngine.EventSystems;

public class TowerButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject cover, dragCover;

    private bool isDragging;

    public void OnBeginDrag(PointerEventData e)
    {
        isDragging = true;
        Persister.SetCursor(true);
        dragCover.SetActive(true);
    }

    public void OnDrag(PointerEventData e)
    {
        
    }

    public void OnEndDrag(PointerEventData e)
    {
        isDragging = false;
        Persister.SetCursor();
        dragCover.SetActive(false);

        if(cover.activeSelf) cover.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cover.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!isDragging) cover.SetActive(false);
    }
}
