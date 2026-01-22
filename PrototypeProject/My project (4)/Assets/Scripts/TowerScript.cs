using System.Collections.Generic;
using UnityEngine;

public class TowereScript : MonoBehaviour
{
    private float attackTimer = 0f;
    public float attackTime = 5f;
    public List<GameObject> enemiesInRange = new List<GameObject>();
    public int damage = 100;
    public bool aoe = false;
    public float aoeRange = 5;
    private GameObject enemyToDamage;

    public GameObject projectilePrefab;
    public Transform firePoint;

    //public AudioSource atackSFX;


    void Update()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackTime)
        {
            attackTimer = 0f;
            Atack();
        }

        if (enemyToDamage != null)
        {
            RotateTowardsEnemy(enemyToDamage.transform);
        }
    }
    private void Atack()
    {
        float maxPathPoints = -1f;
        enemyToDamage = null;

        if (enemiesInRange.Count > 0)
        {
            for (int i = enemiesInRange.Count - 1; i >= 0; i--)
            {
                GameObject enemy = enemiesInRange[i];
                if (enemy == null)
                {
                    enemiesInRange.RemoveAt(i);
                    continue;
                }

                float pathPoints = enemy.GetComponent<EnemyScript>().pathPoints;
                if (pathPoints > maxPathPoints)
                {
                    maxPathPoints = pathPoints;
                    enemyToDamage = enemy;
                }
            }

            if (enemyToDamage != null)
            {
                Shoot(enemyToDamage.transform);
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

    private void Shoot(Transform target)
    {
        GameObject projectileGO = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        Projectile projectile = projectileGO.GetComponent<Projectile>();
        projectile.Initialize(target, damage, aoe, aoeRange);

        //atackSFX.Play();
    }

    private void RotateTowardsEnemy(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0f; // Y-axis only

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                Time.deltaTime * 5f
            );
        }
    }

}