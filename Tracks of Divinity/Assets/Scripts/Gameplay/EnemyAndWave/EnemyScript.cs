using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
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
        GoldCounter moneyManager = goldCounter.GetComponent<GoldCounter>();
        moneyManager.IncrementCount(moneyAward);
        WaveManager.Instance.EnemyDestroyed();
    }
}