using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage;
    private Transform target;

    public void Initialize(Transform target, int damage)
    {
        this.target = target;
        this.damage = damage;
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
        EnemyScript enemy = target.GetComponent<EnemyScript>();
        if (enemy != null)
        {
            enemy.health -= damage;
        }

        Destroy(gameObject);
    }
}
