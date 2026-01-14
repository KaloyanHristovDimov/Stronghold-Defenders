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

    //public void SpawnNextTile()
    //{

    //    bool forwardBlocked = IsDirectionBlocked(transform.forward * -1);
    //    bool leftBlocked = IsDirectionBlocked(-transform.right);
    //    bool rightBlocked = IsDirectionBlocked(transform.right);

    //    List<TileManager.TileType> allowed = new List<TileManager.TileType>();

    //    // Single-exit tiles
    //    if (!forwardBlocked)
    //        allowed.Add(TileManager.TileType.Straight);
    //    Debug.Log("Straight allowed");

    //    if (!leftBlocked)
    //        allowed.Add(TileManager.TileType.LeftTurn);
    //    Debug.Log("Left turn allowed");

    //    if (!rightBlocked)
    //        allowed.Add(TileManager.TileType.RightTurn);
    //    Debug.Log("Right turn allowed");


    //    // No valid tiles → dead end
    //    if (allowed.Count == 0)
    //    {
    //        DeadEnd();
    //        return;
    //    }

    //    TileManager.TileType chosen = allowed[Random.Range(0, allowed.Count)];

    //    TileManager.Instance.SpawnTile(transform.position, transform.rotation, chosen);

    //    DeadEnd();
    //}

    private void DeadEnd()
    {
         deadEnd = true;
         gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //SpawnNextTile();
    }

    //public bool IsDirectionBlocked(Vector3 direction)
    //{
    //    return Physics.Raycast(transform.position, direction, 100);
    //}
}
