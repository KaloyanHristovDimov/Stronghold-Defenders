using UnityEngine;

public class KillBox : MonoBehaviour
{
    public int damage;
    public string healthCounterName = "HealthCounter";
    private GameObject healthCounter;
    private CounterController counterController;
    private void Awake()
    {
        healthCounter = GameObject.Find(healthCounterName);
        counterController = healthCounter.GetComponent<CounterController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        EnemyScript enemyS = collision.gameObject.GetComponent<EnemyScript>();
        Debug.Log("Collided with base");
        if (enemyS != null)
        {
            counterController.DecrementCount(enemyS.health);
            enemyS.health -= damage;
        }
    }
}
