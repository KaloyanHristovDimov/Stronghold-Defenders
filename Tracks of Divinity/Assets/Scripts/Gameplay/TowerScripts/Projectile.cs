using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 100f;
    private int damage;
    private Transform target;

    private bool aoe = false;
    private float aoeRange = 0f;

    private bool appliesSlow = false;
    private float slowMultiplier = 1f;
    private float slowDuration = 0f;

    public void Initialize(Transform target, int damage, bool aoe, float aoeRange, bool appliesSlow, float slowMultiplier, float slowDuration)
    {
        this.target = target;
        this.damage = damage;
        this.aoe = aoe;
        this.aoeRange = aoeRange;

        this.appliesSlow = appliesSlow;
        this.slowMultiplier = slowMultiplier;
        this.slowDuration = slowDuration;
    }
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
        transform.LookAt(target);
    }

    void HitTarget()
    {
        if (aoe)
        {
            ApplyAOEDamage();
        }
        else
        {
            DamageSingleTarget(target);
        }

        Destroy(gameObject);
    }

    private void DamageSingleTarget(Transform enemyTransform)
    {
        EnemyScript enemy = enemyTransform.GetComponent<EnemyScript>();

        if (enemy != null)
        {
            enemy.health -= damage;
            if (appliesSlow)
            {
                enemy.ApplySlow(slowMultiplier, slowDuration);
                Debug.Log("Applied slow to enemy.");
            }
        }
    }

    private void ApplyAOEDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, aoeRange);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyScript enemy = hit.GetComponent<EnemyScript>();
                if (enemy != null)
                {
                    enemy.health -= damage;
                    if (appliesSlow)
                    {
                        enemy.ApplySlow(slowMultiplier, slowDuration);
                        Debug.Log("Applied slow to enemy in AOE.");
                    }
                }
            }
        }
    }
}
