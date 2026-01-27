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

    [System.Serializable]
    private class SpawnGroupState
    {
        public string Name;
        public GameObject MonsterPrefab;

        public int total;
        public int remaining;
        public int spawned;

        public SpawnGroupState(MonsterGroup src)
        {
            Name = src != null ? src.Name : "NULL";
            MonsterPrefab = src != null ? src.MonsterPrefab : null;
            total = src != null ? Mathf.Max(0, src.amount) : 0;
            remaining = total;
            spawned = 0;
        }
    }

    [Header("Groups")]
    public List<MonsterGroup> listOfAllGroupsOfMonsters;
    public List<MonsterGroup> listOfGroupsToPickFrom;

    [Header("Runtime (Planned This Wave)")]
    public List<MonsterGroup> listOfGroupsOfMonstersToSpawn = new();

    [Header("Spawn Points")]
    public List<EndPoint> listOfActiveEndPoints = new();

    [Header("Timing")]
    public float spawnDelay = 0.5f;
    public float timeBetweenWaves = 5f;
    public int wavesBetweenAddNewGround = 5;

    [Header("Spawn Sync")]
    [SerializeField] private float endpointAssignmentDelay = 0.1f;

    [Header("Progression")]
    public int wavesSinceLastGroupAdded = 0;
    public int currentWave = 0;

    [Header("Wave State")]
    public int aliveEnemies = 0;
    public bool waveInProgress = false;
    public bool finishedSpawning = false;

    private bool waitingForPlayerTile = true;
    private bool nextWaveCountdownRunning = false;
    private Coroutine startNextWaveCoroutine;

    public bool CanPlaceTile => waitingForPlayerTile && !waveInProgress && !nextWaveCountdownRunning;

    private List<SpawnGroupState> groupsLeft = new();

    // NEW: allow multiple spawn coroutines per endpoint; just count active jobs
    private int activeSpawnJobs = 0;

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

        UICanvasController.WaveCounter.IncrementCount();
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
            if (listOfAllGroupsOfMonsters != null && listOfAllGroupsOfMonsters.Count > 0)
            {
                listOfGroupsOfMonstersToSpawn.Add(listOfAllGroupsOfMonsters[0]);

                if (!listOfGroupsToPickFrom.Contains(listOfAllGroupsOfMonsters[0]))
                    listOfGroupsToPickFrom.Add(listOfAllGroupsOfMonsters[0]);
            }
            else
            {
                Debug.LogError("[WaveManager] listOfAllGroupsOfMonsters is empty. Cannot start wave 1.");
                finishedSpawning = true;
                waveInProgress = false;
                waitingForPlayerTile = true;
                SetEndpointsInteractable(true);
                return;
            }
        }
        else
        {
            if (listOfGroupsToPickFrom == null || listOfGroupsToPickFrom.Count == 0)
            {
                Debug.LogError("[WaveManager] listOfGroupsToPickFrom is empty. Cannot start wave.");
                finishedSpawning = true;
                waveInProgress = false;
                waitingForPlayerTile = true;
                SetEndpointsInteractable(true);
                return;
            }

            for (int i = 0; i < waveNumber; i++)
            {
                MonsterGroup randomGroup = listOfGroupsToPickFrom[Random.Range(0, listOfGroupsToPickFrom.Count)];
                listOfGroupsOfMonstersToSpawn.Add(randomGroup);
            }
        }

        groupsLeft.Clear();
        for (int i = 0; i < listOfGroupsOfMonstersToSpawn.Count; i++)
            groupsLeft.Add(new SpawnGroupState(listOfGroupsOfMonstersToSpawn[i]));

        StartCoroutine(SpawnWaveRoutine());
    }

    // UPDATED: assign groups immediately, even to the same endpoint (multiple concurrent coroutines)
    private IEnumerator SpawnWaveRoutine()
    {
        finishedSpawning = false;

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
            TryCompleteWave();
            yield break;
        }

        activeSpawnJobs = 0;

        int epIndex = 0;

        while (groupsLeft.Count > 0)
        {
            EndPoint ep = validEndpoints[epIndex % validEndpoints.Count];
            epIndex++;

            SpawnGroupState group = groupsLeft[groupsLeft.Count - 1];
            groupsLeft.RemoveAt(groupsLeft.Count - 1);

            activeSpawnJobs++;
            StartCoroutine(SpawnGroupOnEndpoint(ep, group));

            if (endpointAssignmentDelay > 0f)
                yield return new WaitForSeconds(endpointAssignmentDelay);
            else
                yield return null; // keeps frames responsive
        }

        while (activeSpawnJobs > 0)
            yield return null;

        finishedSpawning = true;
        TryCompleteWave();
    }

    // UPDATED: no endpoint busy flags; decrement active jobs when done
    private IEnumerator SpawnGroupOnEndpoint(EndPoint endpoint, SpawnGroupState group)
    {
        WaitForSeconds wait = new WaitForSeconds(spawnDelay);

        try
        {
            while (group.remaining > 0)
            {
                if (endpoint == null || !endpoint.gameObject.activeInHierarchy)
                    break;

                if (group.MonsterPrefab == null)
                    break;

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

                group.remaining--;
                group.spawned++;

                yield return wait;
            }
        }
        finally
        {
            activeSpawnJobs = Mathf.Max(0, activeSpawnJobs - 1);
            TryCompleteWave();
        }
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
        // Spawn coroutines will naturally stop spawning if the endpoint becomes null/inactive.
        // Avoid StopCoroutine here unless you also handle decrementing activeSpawnJobs.
    }

    private void UpdateGroupsToPickFrom()
    {
        if (wavesSinceLastGroupAdded < wavesBetweenAddNewGround)
            return;

        wavesSinceLastGroupAdded = 0;

        if (listOfGroupsToPickFrom.Count == 0)
            return;

        if (listOfAllGroupsOfMonsters == null || listOfAllGroupsOfMonsters.Count == 0)
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

    private void TryCompleteWave()
    {
        if (!waveInProgress)
            return;

        if (finishedSpawning && aliveEnemies <= 0)
            OnWaveCompleted();
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
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
        TryCompleteWave();
    }
}
