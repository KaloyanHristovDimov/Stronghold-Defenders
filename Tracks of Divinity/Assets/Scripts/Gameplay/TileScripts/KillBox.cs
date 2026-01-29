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
            UICanvasController.HealthCounter.DecrementCount(enemyS.damage);
            enemyS.health -= damage;
            
            if(UICanvasController.HealthCounter.Count <= 0) UICanvasController.LoseScreen.Open();
        }
    }
}