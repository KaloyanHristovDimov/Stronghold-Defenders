using UnityEngine;
using TMPro;

public class BaseHP : MonoBehaviour
{
    public int baseHealth = 100;
    private int currentHealth;

    public GameObject lossMenu;
    public TextMeshProUGUI healthText;
    void Start()
    {
        currentHealth = baseHealth;
        lossMenu.SetActive(false);
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyScript enemy = other.GetComponent<enemyScript>();
            currentHealth -= enemy.damage;
            Destroy(other.gameObject);
            healthText.text = "HP: " + currentHealth.ToString();
            if (currentHealth <= 0)
            {
                lossMenu.SetActive(true);
                global::PauseManager.GameIsPaused = true;
                Time.timeScale = 0f;
            }
        }
    }
}
