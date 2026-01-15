using UnityEngine;
using UnityEngine.EventSystems;

public class CloseWindowSpace : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Window window;

    public void OnPointerClick(PointerEventData eventData) => window.Close();
}
