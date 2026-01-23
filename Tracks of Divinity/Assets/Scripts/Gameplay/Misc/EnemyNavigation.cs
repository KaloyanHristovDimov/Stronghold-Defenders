
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
        StartCoroutine(DoThingAfterDelay(0.01f));

        IEnumerator DoThingAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Debug.Log("Colided");
            if (other.CompareTag("Enemy"))
            {

                other.transform.rotation = newRotation;
                Debug.Log("Rotated");
            }
        }
        
    }
}
