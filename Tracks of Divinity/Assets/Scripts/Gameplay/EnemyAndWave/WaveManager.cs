using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [Header("Endpoint Sealing")]
    public int startingSealCost = 50;
    [SerializeField] private TextMeshProUGUI sealCostText;
    private int currentSealCost;
    private readonly HashSet<EndPoint> sealedEndpoints = new();

    private bool waitingForPlayerTile = true;
    private bool nextWaveCountdownRunning = false;
    private Coroutine startNextWaveCoroutine;

    public bool CanPlaceTile => waitingForPlayerTile && !waveInProgress && !nextWaveCountdownRunning;

    private List<SpawnGroupState> groupsLeft = new();

    private readonly Dictionary<EndPoint, bool> endpointBusy = new();
    private readonly Dictionary<EndPoint, Coroutine> endpointSpawnRoutines = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentSealCost = startingSealCost;

        waitingForPlayerTile = true;
        nextWaveCountdownRunning = false;
        RefreshAllEndpoints();
    }

    private void SetEndpointsInteractable(bool allowInteraction)
    {
        for (int i = 0; i < listOfActiveEndPoints.Count; i++)
        {
            EndPoint ep = listOfActiveEndPoints[i];
            if (ep == null)
                continue;

            bool canUse =
                allowInteraction &&
                !ep.IsBlocked();

            ep.SetInteractable(canUse);
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
        RefreshAllEndpoints();
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

    private IEnumerator SpawnWaveRoutine()
    {
        finishedSpawning = false;

        var keys = new List<EndPoint>(endpointBusy.Keys);
        for (int i = 0; i < keys.Count; i++)
            endpointBusy[keys[i]] = false;

        while (groupsLeft.Count > 0)
        {
            List<EndPoint> validEndpoints = new List<EndPoint>();
            for (int i = 0; i < listOfActiveEndPoints.Count; i++)
            {
                EndPoint ep = listOfActiveEndPoints[i];
                if (ep != null && ep.gameObject.activeInHierarchy && !sealedEndpoints.Contains(ep))
                {
                    validEndpoints.Add(ep);
                }
            }

            if (validEndpoints.Count == 0)
            {
                Debug.LogError("[WaveManager] No valid endpoints during spawning. Aborting remaining spawns to avoid soft-lock.");
                finishedSpawning = true;
                TryCompleteWave();
                yield break;
            }

            bool assignedAtLeastOne = false;

            for (int e = 0; e < validEndpoints.Count && groupsLeft.Count > 0; e++)
            {
                EndPoint ep = validEndpoints[e];

                if (!endpointBusy.TryGetValue(ep, out bool busy))
                {
                    endpointBusy[ep] = false;
                    busy = false;
                }

                if (busy)
                    continue;

                SpawnGroupState group = groupsLeft[groupsLeft.Count - 1];
                groupsLeft.RemoveAt(groupsLeft.Count - 1);

                assignedAtLeastOne = true;

                endpointBusy[ep] = true;

                Coroutine c = StartCoroutine(SpawnGroupOnEndpoint(ep, group));
                endpointSpawnRoutines[ep] = c;

                if (endpointAssignmentDelay > 0f)
                    yield return new WaitForSeconds(endpointAssignmentDelay);
            }

            if (!assignedAtLeastOne)
                yield return null;
            else
                yield return null;
        }

        while (AnyEndpointBusy())
            yield return null;

        finishedSpawning = true;
        TryCompleteWave();
    }

    private bool AnyEndpointBusy()
    {
        foreach (var kv in endpointBusy)
        {
            if (kv.Key != null && kv.Key.gameObject.activeInHierarchy && kv.Value)
                return true;
        }
        return false;
    }

    private IEnumerator SpawnGroupOnEndpoint(EndPoint endpoint, SpawnGroupState group)
    {
        WaitForSeconds wait = new WaitForSeconds(spawnDelay);

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

        if (endpoint != null)
            endpointBusy[endpoint] = false;

        if (endpoint != null)
            endpointSpawnRoutines[endpoint] = null;

        TryCompleteWave();
    }

    public void RegisterEndpoint(EndPoint endpoint)
    {
        if (!listOfActiveEndPoints.Contains(endpoint))
            listOfActiveEndPoints.Add(endpoint);

        if (!endpointBusy.ContainsKey(endpoint))
            endpointBusy.Add(endpoint, false);

        endpoint.SetInteractable(CanPlaceTile);
    }

    public void UnregisterEndpoint(EndPoint endpoint)
    {
        listOfActiveEndPoints.Remove(endpoint);

        endpointBusy.Remove(endpoint);

        if (endpointSpawnRoutines.TryGetValue(endpoint, out var c) && c != null)
            StopCoroutine(c);

        endpointSpawnRoutines.Remove(endpoint);
    }

    public void RefreshAllEndpoints()
    {
        for (int i = 0; i < listOfActiveEndPoints.Count; i++)
        {
            EndPoint ep = listOfActiveEndPoints[i];
            if (ep != null)
                ep.SetInteractable(CanPlaceTile);
        }
    }

    private void UpdateGroupsToPickFrom()
    {
        if (wavesSinceLastGroupAdded < wavesBetweenAddNewGround)
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
        RefreshAllEndpoints();
    }

    public void EnemyDestroyed()
    {
        
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
        TryCompleteWave();
    }


    public bool TrySealEndpoint(EndPoint endpoint)
    {
        if (endpoint == null || sealedEndpoints.Contains(endpoint))
            return false;

        GoldCounter gold = FindObjectOfType<GoldCounter>();
        if (gold == null || !gold.CanAfford(currentSealCost))
            return false;

        gold.DecrementCount(currentSealCost);

        sealedEndpoints.Add(endpoint);
        currentSealCost *= 2;
        sealCostText.text = "Cost: " + currentSealCost;

        return true;
    }
}
