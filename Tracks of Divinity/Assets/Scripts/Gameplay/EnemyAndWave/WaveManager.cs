using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [System.Serializable]
    public class MonsterGroup
    {
        public string Name;
        public GameObject MonsterPrefab;
        public int amount;
    }
        
    [Header("Groups")]
    public List<MonsterGroup> listOfAllGroupsOfMonsters;
    public List<MonsterGroup> listOfGroupsToPickFrom;

    [Header("Runtime")]
    public List<MonsterGroup> listOfGroupsOfMonstersToSpawn = new();
    private List<MonsterGroup> listOfGroupsLeftToSpawn = new();

    [Header("Spawn Points")]
    public List<EndPoint> listOfActiveEndPoints = new();

    [Header("Timing")]
    public float spawnDelay = 0.5f;
    public int wavesSinceLastGroupAdded = 0;

    [Header("Wave State")]
    public int currentWave = 0;
    public float timeBetweenWaves = 5f;

    public int aliveEnemies = 0;
    public bool waveInProgress = false;
    public bool finishedSpawning = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Start first wave
        StartCoroutine(NextWaveCountdown());
    }


    public void StartWave(int waveNumber)
    {
        currentWave = waveNumber;
        waveInProgress = true;
        finishedSpawning = false;
        aliveEnemies = 0;
        UpdateGroupsToPickFrom();

        listOfGroupsOfMonstersToSpawn.Clear();

        // First wave → only Group A (index 0 for example)
        if (waveNumber == 1)
        {
            listOfGroupsOfMonstersToSpawn.Add(listOfAllGroupsOfMonsters[0]);
            listOfGroupsToPickFrom.Add(listOfAllGroupsOfMonsters[0]);
        }
        else
        {
            // Pick X random groups, where X = wave number
            for (int i = 0; i < waveNumber; i++)
            {
                MonsterGroup randomGroup =
                    listOfGroupsToPickFrom[Random.Range(0, listOfGroupsToPickFrom.Count-1)];

                listOfGroupsOfMonstersToSpawn.Add(randomGroup);
            }
        }

        // Copy list (important: NEW list, not reference)
        listOfGroupsLeftToSpawn = new List<MonsterGroup>(listOfGroupsOfMonstersToSpawn);

        StartCoroutine(SpawnWaveRoutine());
    }

    private IEnumerator SpawnWaveRoutine()
    {
        // While there are groups left
        while (listOfGroupsLeftToSpawn.Count > 0)
        {
            foreach (EndPoint endpoint in listOfActiveEndPoints)
            {
                if (listOfGroupsLeftToSpawn.Count == 0)
                    yield break;

                // Always take the last group
                MonsterGroup group = listOfGroupsLeftToSpawn[listOfGroupsLeftToSpawn.Count - 1];
                    

                // Spawn all monsters in that group
                for (int i = 0; i < group.amount; i++)
                {
                    Quaternion rotation;
                    switch (endpoint.direction)
                    {
                        case TileData.Direction.Top:
                            rotation = Quaternion.Euler(0f, 90f, 0f);
                            break;

                        case TileData.Direction.Bottom:
                            rotation = Quaternion.Euler(0f, -90f, 0f);
                            break;

                        case TileData.Direction.Right:
                            rotation = Quaternion.Euler(0f, 180f, 0f);
                            break;

                        case TileData.Direction.Left:
                            rotation = Quaternion.Euler(0f, 0f, 0f);
                            break;

                        default:
                            rotation = Quaternion.identity;
                            break;
                    }
                    Instantiate(group.MonsterPrefab, endpoint.transform.position, rotation);
                    aliveEnemies++;
                    yield return new WaitForSeconds(spawnDelay);
                }

                // Remove group after spawning
                listOfGroupsLeftToSpawn.RemoveAt(listOfGroupsLeftToSpawn.Count - 1);
            }
        }
        finishedSpawning = true;

    }

    public void RegisterEndpoint(EndPoint endpoint)
    {
        if (!listOfActiveEndPoints.Contains(endpoint))
            listOfActiveEndPoints.Add(endpoint);
    }

    public void UnregisterEndpoint(EndPoint endpoint)
    {
        listOfActiveEndPoints.Remove(endpoint);
    }

    private void UpdateGroupsToPickFrom()
    {
        if (wavesSinceLastGroupAdded < 5)
            return;

        wavesSinceLastGroupAdded = 0;

        // If last pickable group is already the last overall group → stop
        if (listOfGroupsToPickFrom[^1] == listOfAllGroupsOfMonsters[^1])
            return;

        // Case 1: Sliding window (max 6 groups)
        if (listOfGroupsToPickFrom.Count >= 6)
        {
            // Remove oldest
            listOfGroupsToPickFrom.RemoveAt(0);

            bool addNext = false;

            foreach (var group in listOfAllGroupsOfMonsters)
            {
                if (addNext)
                {
                    listOfGroupsToPickFrom.Add(group);
                    break;
                }

                if (group == listOfGroupsToPickFrom[^1])
                {
                    addNext = true;
                }
            }
        }
        // Case 2: Still growing pool
        else
        {
            int nextIndex = listOfGroupsToPickFrom.Count;

            if (nextIndex < listOfAllGroupsOfMonsters.Count)
            {
                listOfGroupsToPickFrom.Add(listOfAllGroupsOfMonsters[nextIndex]);
            }
        }
    }

    private void OnWaveCompleted()
    {
        wavesSinceLastGroupAdded++;

        StartCoroutine(NextWaveCountdown());
    }

    private IEnumerator NextWaveCountdown()
    {
        float timer = timeBetweenWaves;

        while (timer > 0f)
        {
            // UI hook here if needed
            yield return null;
            timer -= Time.deltaTime;
        }

        StartWave(currentWave + 1);
    }

    public void EnemyDestroyed()
    {
        aliveEnemies--;
        if (finishedSpawning && aliveEnemies <= 0 && waveInProgress)
        {
            waveInProgress = false;
            OnWaveCompleted();
        }
    }
}
