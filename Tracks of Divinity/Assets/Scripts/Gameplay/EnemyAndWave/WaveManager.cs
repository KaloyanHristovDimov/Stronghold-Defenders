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

    private bool waitingForPlayerTile = true;
    private bool nextWaveCountdownRunning = false;
    private Coroutine startNextWaveCoroutine;

    public bool CanPlaceTile => waitingForPlayerTile && !waveInProgress && !nextWaveCountdownRunning;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        waitingForPlayerTile = true;
        nextWaveCountdownRunning = false;
        SetEndpointsInteractable(true);
    }

    private void SetEndpointsInteractable(bool interactable)
    {
        for (int i = 0; i < listOfActiveEndPoints.Count; i++)
        {
            if (listOfActiveEndPoints[i] != null)
                listOfActiveEndPoints[i].SetInteractable(interactable);
        }
    }

    public void NotifyTilePlaced_StartNextWave()
    {
        if (!waitingForPlayerTile || waveInProgress || nextWaveCountdownRunning)
            return;

        waitingForPlayerTile = false;
        nextWaveCountdownRunning = true;

        SetEndpointsInteractable(false);

        if (startNextWaveCoroutine != null)
            StopCoroutine(startNextWaveCoroutine);

        startNextWaveCoroutine = StartCoroutine(StartNextWaveAfterDelay());
    }

    private IEnumerator StartNextWaveAfterDelay()
    {
        float timer = timeBetweenWaves;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        nextWaveCountdownRunning = false;
        StartWave(currentWave + 1);
    }

    public void StartWave(int waveNumber)
    {
        currentWave = waveNumber;
        waveInProgress = true;
        finishedSpawning = false;
        aliveEnemies = 0;

        SetEndpointsInteractable(false);

        UpdateGroupsToPickFrom();
        listOfGroupsOfMonstersToSpawn.Clear();

        if (waveNumber == 1)
        {
            listOfGroupsOfMonstersToSpawn.Add(listOfAllGroupsOfMonsters[0]);

            if (!listOfGroupsToPickFrom.Contains(listOfAllGroupsOfMonsters[0]))
                listOfGroupsToPickFrom.Add(listOfAllGroupsOfMonsters[0]);
        }
        else
        {
            for (int i = 0; i < waveNumber; i++)
            {
                MonsterGroup randomGroup =
                    listOfGroupsToPickFrom[Random.Range(0, listOfGroupsToPickFrom.Count)];

                listOfGroupsOfMonstersToSpawn.Add(randomGroup);
            }
        }

        listOfGroupsLeftToSpawn = new List<MonsterGroup>(listOfGroupsOfMonstersToSpawn);
        StartCoroutine(SpawnWaveRoutine());
    }

    private IEnumerator SpawnWaveRoutine()
    {
        // Small optimization: reuse the same wait object.
        WaitForSeconds wait = new WaitForSeconds(spawnDelay);

        while (listOfGroupsLeftToSpawn.Count > 0)
        {
            // Build a valid endpoint list every cycle (not just once).
            List<EndPoint> validEndpoints = new List<EndPoint>();
            for (int i = 0; i < listOfActiveEndPoints.Count; i++)
            {
                EndPoint ep = listOfActiveEndPoints[i];
                if (ep != null && ep.gameObject.activeInHierarchy)
                    validEndpoints.Add(ep);
            }

            if (validEndpoints.Count == 0)
            {
                Debug.LogError("[WaveManager] No valid endpoints during spawning. Aborting remaining spawns to avoid soft-lock.");
                finishedSpawning = true;
                yield break;
            }

            // Pick the next group (stack behaviour like your original).
            MonsterGroup group = listOfGroupsLeftToSpawn[listOfGroupsLeftToSpawn.Count - 1];

            // Choose an endpoint to spawn this group from.
            // If you only want ONE endpoint ever, just pick [0].
            EndPoint endpoint = validEndpoints[Random.Range(0, validEndpoints.Count)];

            // Spawn the whole group from that endpoint.
            for (int i = 0; i < group.amount; i++)
            {
                // If endpoint disappears mid-group, exit cleanly instead of crashing.
                if (endpoint == null || !endpoint.gameObject.activeInHierarchy)
                {
                    Debug.LogError("[WaveManager] Endpoint became invalid mid-group. Aborting remaining spawns to avoid stuck wave.");
                    finishedSpawning = true;
                    yield break;
                }

                Quaternion rotation = endpoint.direction switch
                {
                    TileData.Direction.Top => Quaternion.Euler(0f, 90f, 0f),
                    TileData.Direction.Bottom => Quaternion.Euler(0f, -90f, 0f),
                    TileData.Direction.Right => Quaternion.Euler(0f, 180f, 0f),
                    TileData.Direction.Left => Quaternion.Euler(0f, 0f, 0f),
                    _ => Quaternion.identity
                };

                Instantiate(group.MonsterPrefab, endpoint.transform.position, rotation);
                aliveEnemies++;

                yield return wait;
            }

            // Now we can safely remove the group.
            listOfGroupsLeftToSpawn.RemoveAt(listOfGroupsLeftToSpawn.Count - 1);
        }

        finishedSpawning = true;
    }


    public void RegisterEndpoint(EndPoint endpoint)
    {
        if (!listOfActiveEndPoints.Contains(endpoint))
            listOfActiveEndPoints.Add(endpoint);

        endpoint.SetInteractable(CanPlaceTile);
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

        if (listOfGroupsToPickFrom.Count == 0)
            return;

        if (listOfGroupsToPickFrom[^1] == listOfAllGroupsOfMonsters[^1])
            return;

        if (listOfGroupsToPickFrom.Count >= 6)
        {
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
                    addNext = true;
            }
        }
        else
        {
            int nextIndex = listOfGroupsToPickFrom.Count;
            if (nextIndex < listOfAllGroupsOfMonsters.Count)
                listOfGroupsToPickFrom.Add(listOfAllGroupsOfMonsters[nextIndex]);
        }
    }

    private void OnWaveCompleted()
    {
        wavesSinceLastGroupAdded++;

        waveInProgress = false;
        waitingForPlayerTile = true;
        nextWaveCountdownRunning = false;

        SetEndpointsInteractable(true);
    }

    public void EnemyDestroyed()
    {
        aliveEnemies--;
        if (finishedSpawning && aliveEnemies <= 0 && waveInProgress)
        {
            OnWaveCompleted();
        }
    }
}
