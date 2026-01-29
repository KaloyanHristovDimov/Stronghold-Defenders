using UnityEngine;
using UnityEngine.UI;

public class DraggedIcon : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image icon;

    public void Activate(Sprite sprite)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UICanvasController.canvasRectT, Input.mousePosition, null, out var mousePos);
        rectTransform.anchoredPosition = new(mousePos.x, mousePos.y);
        
        icon.sprite = sprite;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UICanvasController.canvasRectT, Input.mousePosition, null, out var mousePos);
        rectTransform.anchoredPosition = new(mousePos.x, mousePos.y);
    }
}
