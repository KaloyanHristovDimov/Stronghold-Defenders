using UnityEngine;

public class KillBox : MonoBehaviour
{
    public int damage;

    private void OnCollisionEnter(Collision collision)
    {
        EnemyScript enemyS = collision.gameObject.GetComponent<EnemyScript>();
        Debug.Log("Collided with base");
        if (enemyS != null)
        {
            enemyS.health -= damage;
        }
    }
}
