
using System.Collections;
using System.Net;
using UnityEngine;

public class EnemyNavigation : MonoBehaviour
{
    private TileData currentTile;
    private TileData.Direction DirectionToTurn;
    private Quaternion newRotation;
    public void SetupNavigation(TileData tileData)
    {
        DirectionToTurn = tileData.startDirection;
        
        switch (DirectionToTurn)
        {
            case TileData.Direction.Top:
                newRotation = Quaternion.Euler(0f, -90f, 0f);
                break;

            case TileData.Direction.Bottom:
                newRotation = Quaternion.Euler(0f, 90f, 0f);
                break;

            case TileData.Direction.Right:
                newRotation = Quaternion.Euler(0f, 0f, 0f);
                break;

            case TileData.Direction.Left:
                newRotation = Quaternion.Euler(0f, 180f, 0f);
                break;

            default:
                newRotation = Quaternion.identity;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Vector3 monsterPosition = other.transform.position;
            monsterPosition.x = transform.position.x;
            monsterPosition.z = transform.position.z;
            other.transform.position = monsterPosition;
            other.transform.rotation = newRotation;
        }
    }
}
