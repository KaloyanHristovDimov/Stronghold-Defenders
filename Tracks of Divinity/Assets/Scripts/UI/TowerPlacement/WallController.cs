using UnityEngine;
using UnityEngine.EventSystems;

public class WallController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UICanvasController.canvasRectT, Input.mousePosition, null, out var mousePos);
        UICanvasController.WallCardRectT.anchoredPosition = new(mousePos.x, mousePos.y);

        UICanvasController.WallCardRectT.gameObject.SetActive(true);
        //Display wall info here if needed
    }

    public void OnPointerExit(PointerEventData eventData) => UICanvasController.WallCardRectT.gameObject.SetActive(false);
}
