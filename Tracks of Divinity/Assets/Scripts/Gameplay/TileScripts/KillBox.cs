using UnityEngine;

public class KillBox : MonoBehaviour
{
    public int damage;
    public string healthCounterName = "HealthCounter";

    private void OnCollisionEnter(Collision collision)
    {
        EnemyScript enemyS = collision.gameObject.GetComponent<EnemyScript>();
        Debug.Log("Collided with base");
        if (enemyS != null)
        {
            UICanvasController.HealthCounter.DecrementCount(enemyS.health);
            enemyS.health -= damage;
            //Add death screen event activation
        }
    }
}