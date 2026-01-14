using UnityEngine;
using System.Collections.Generic;

public class SlowTower : MonoBehaviour
{
    public List<GameObject> enemiesInRange = new List<GameObject>();
    public float slowAmount = 0.4f;
    public float slowDuration = 3f;

    void Update()
    {
        SlowDown();
    }

    private void SlowDown()
    {
        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            GameObject enemy = enemiesInRange[i];
            if (enemy == null)
            {
                enemiesInRange.RemoveAt(i);
                continue;
            }
            enemyScript es = enemy.GetComponent<enemyScript>();  // Cache component

            if (!es.isSlowed)
            {
                // Reduce speed by 20%
                es.speed = es.speed * (1 - slowAmount);
                es.slowCoolDown = slowDuration;
                es.isSlowed = true;
            }
            


        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameObject enemy = other.gameObject;
            enemiesInRange.Add(enemy);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            RemoveFromList(other.gameObject);

        }
    }
    private void RemoveFromList(GameObject enemy)
    {
        
        enemiesInRange.Remove(enemy);
    }
}
