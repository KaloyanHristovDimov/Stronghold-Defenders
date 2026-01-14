using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;


public class NextTilePlacement : MonoBehaviour, IPointerDownHandler
{
    public bool deadEnd = false;

    void OnTriggerEnter(Collider other)
    {
        DeadEnd();
    }

    private void DeadEnd()
    {
         deadEnd = true;
         gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isBlocked)
            return;

        TileManager.Instance.RequestTileSpawn(this);
        DeadEnd();
    }


}
