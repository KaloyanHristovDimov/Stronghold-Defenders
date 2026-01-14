using UnityEngine;

public class CheckPointScript : MonoBehaviour
{
    public int turnDirection=0; // 0 = left, 1 = right



    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy"))
        {
            GameObject enemy = other.gameObject;
            switch (turnDirection)
            {
                case 0:
                    enemy.transform.Rotate(0, -90, 0);
                    break;
                case 1:
                    enemy.transform.Rotate(0, 90, 0);
                    break;
                default:
                    break;
            }
        }
    }
}
