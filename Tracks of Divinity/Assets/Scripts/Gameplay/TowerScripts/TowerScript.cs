using System.Collections.Generic;
using UnityEngine;

public class TowereScript : MonoBehaviour
{
    private float attackTimer = 0f;
    public float attackTime = 1f;
    public List<GameObject> enemiesInRange = new List<GameObject>();
    public int damage = 50;
    public float range = 5f;
    [Header("AOE Settings")]
    public bool aoe = false;
    public float aoeRange = 4;

    [Header("Slow Effect")]
    public bool appliesSlow = false;
    [Range(0.1f, 1f)] public float slowMultiplier = 1f;
    public float slowDuration = 2f;

    public TowerType towerType;
    public Biome biome;
    public bool damageType=false;
    public int price=100;
    public Tower Data;

    private GameObject enemyToDamage;

    public GameObject projectilePrefab;
    public Transform firePoint;

    //public AudioSource atackSFX;

    private void Start()
    {
        attackTime = Data.speed;
        damage = Data.damage;
        range = Data.range;
        price = Data.price;
        towerType = Data.towerType;
        biome = Data.biome;
        damageType = Data.damageType;
        aoe = Data.aoeRange > 0;
        aoeRange = Data.aoeRange;

        appliesSlow = Data.appliesSlow;
        slowMultiplier = Data.slowMultiplier;
        slowDuration = Data.slowDuration;


        GetComponent<SphereCollider>().radius = range;
    }

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
        projectile.Initialize(target, damage, aoe, aoeRange, appliesSlow, slowMultiplier, slowDuration);

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


    public Tower CreateTowerData()
    {
        return new Tower(
            damageType: damageType,
            damage: damage,
            range: Mathf.RoundToInt(range),
            speed: attackTime, // attack speed derived from cooldown
            price: price,
            towerType: towerType,
            biome: biome,
            aoeRange: aoe ? Mathf.RoundToInt(aoeRange) : 0
        );
    }
}