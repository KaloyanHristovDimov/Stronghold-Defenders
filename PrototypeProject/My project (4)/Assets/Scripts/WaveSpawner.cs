using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{

    public GameObject enemyPrefab;
    public Transform spawnPoint;

    public float timeBetweenSpawns = 0.8f;
    public float timeBetweenWaves = 5f;
    public int startEnemyCount = 3;

    private int currentWave = 1;
    private int enemiesThisWave;

    public GameObject WinMenu;
 
    private void Start()
    {
        enemiesThisWave = startEnemyCount;
        StartCoroutine(SpawnWave());
        WinMenu.SetActive(false);
    }

    IEnumerator SpawnWave()
    {

        while (true)
        {
            if (currentWave > 4)
            {
                WinMenu.SetActive(true);
                global::PauseManager.GameIsPaused = true;
                Time.timeScale = 0f;
            }
            Debug.Log("Wave " + currentWave + " started");

            for (int i = 0; i < enemiesThisWave; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(timeBetweenSpawns);
            }

            Debug.Log("Wave " + currentWave + " finished");

            // Prepare next wave
            currentWave++;
            enemiesThisWave *= 2;

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    void SpawnEnemy()
    {
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
