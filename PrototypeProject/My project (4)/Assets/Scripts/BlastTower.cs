using System.Collections.Generic;
using UnityEngine;

public class BlastTower : MonoBehaviour
{
    private float attackTimer = 0f;
    public float attackTime = 5f;
    public List<GameObject> enemiesInRange = new List<GameObject>();
    public int damage = 50;

    public ParticleSystem atackEffect;
    public AudioSource atackSFX;


    void Update()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackTime)
        {
            attackTimer = 0f;
            Atack();
        }
    }
    private void Atack()
    {
        for (int i = enemiesInRange.Count - 1; i >= 0; i--)
        {
            GameObject enemy = enemiesInRange[i];

            if (enemy == null)
            {
                enemiesInRange.RemoveAt(i);
                continue;
            }

            enemyScript script = enemy.GetComponent<enemyScript>();
            int newHp = script.health - damage;

            script.health = newHp;

            if (newHp <= 0)
            {
                enemiesInRange.RemoveAt(i);
            }
        }
        atackEffect.Play();
        atackSFX.Play();
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
