using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class EnemyScript : MonoBehaviour
{
    public enum MonsterType
    {
        Goblin,
        Orc,
        Fly,
        Ant,
        Mystic,
        Crab
    }
    public MonsterType type;

    // 🔹 STATIC death counter for all enemy instances
    public static Dictionary<MonsterType, int> DeathCounts = new Dictionary<MonsterType, int>();


    public float speed = 5f;
    public int health = 100;
    public float pathPoints = 0f;
    private float originalSpeed;
    public float slowCoolDown = 1f;
    public bool isSlowed = false;
    public int moneyAward = 25;
    public int damage = 10;
    public string goldCounterName = "GoldCounter";
    private GameObject goldCounter;

    void Start()
    {
        originalSpeed = speed;
        goldCounter = GameObject.Find(goldCounterName);

        // 🔹 Ensure this monster type exists in the dictionary
        if (!DeathCounts.ContainsKey(type))
        {
            DeathCounts[type] = 0;
        }
    }
    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        pathPoints = (pathPoints + Time.deltaTime * speed / 10);
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        SlownesCheck();
    }

    public void ApplySlow(float slowMultiplier, float duration)
    {
        if (isSlowed) return;

        speed = originalSpeed * slowMultiplier;
        slowCoolDown = duration;
        isSlowed = true;
    }

    private void SlownesCheck()
    {
        if (isSlowed)
        {
            slowCoolDown -= Time.deltaTime;
            if (slowCoolDown <= 0f)
            {
                speed = originalSpeed;
                isSlowed = false;
            }
        }
    }

    void OnDestroy()
    {
         // 🔹 Track death by type
        DeathCounts[type]++;

        // Optional: Debug output
        Debug.Log($"{type} died. Total: {DeathCounts[type]}");

        UICanvasController.GoldCounter.IncrementCount(moneyAward);
        WaveManager.Instance.EnemyDestroyed();
    
    }
}