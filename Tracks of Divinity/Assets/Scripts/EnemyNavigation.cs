
using System.Net;
using UnityEngine;

public class EnemyNavigation : MonoBehaviour
{
    private TileData currentTile;
    private TileData.Direction DirectionToTurn;
    private Quaternion rotation;
    public void SetupNavigation(TileData tileData)
    {
        
        switch (DirectionToTurn)
        {
            case TileData.Direction.Top:
                rotation = Quaternion.Euler(0f, -90f, 0f);
                break;

            case TileData.Direction.Bottom:
                rotation = Quaternion.Euler(0f, 90f, 0f);
                break;

            case TileData.Direction.Right:
                rotation = Quaternion.Euler(0f, 180f, 0f);
                break;

            case TileData.Direction.Left:
                rotation = Quaternion.Euler(0f, 0f, 0f);
                break;

            default:
                rotation = Quaternion.identity;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.transform.rotation = rotation;
        }
    }
}
