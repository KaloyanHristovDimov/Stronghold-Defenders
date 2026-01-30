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

    [Header("Wave Scaling")]
    [SerializeField] private int groupsPerWaveMultiplier = 1;

    [Header("Assignment Sync (delay between endpoint assignments)")]
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

    [SerializeField] private WaveIsStarting waveIsStarting;

    private bool waitingForPlayerTile = true;
    private bool nextWaveCountdownRunning = false;
    private Coroutine startNextWaveCoroutine;

    public bool CanPlaceTile => waitingForPlayerTile && !waveInProgress && !nextWaveCountdownRunning;

    // NEW: track group-spawn coroutines so we can say "all enemies have been spawned"
    private int activeGroupSpawnCoroutines = 0;
    private bool allGroupsAssigned = false;

    private Coroutine assignGroupsCoroutine;

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

            bool canUse = allowInteraction && !ep.IsBlocked();
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
        waveIsStarting.Activate();

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
        allGroupsAssigned = false;
        activeGroupSpawnCoroutines = 0;

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

            int groupsToSpawn = waveNumber * groupsPerWaveMultiplier;
            for (int i = 0; i < groupsToSpawn; i++)
            {
                MonsterGroup randomGroup = listOfGroupsToPickFrom[Random.Range(0, listOfGroupsToPickFrom.Count)];
                listOfGroupsOfMonstersToSpawn.Add(randomGroup);
            }
        }

        if (assignGroupsCoroutine != null)
            StopCoroutine(assignGroupsCoroutine);

        assignGroupsCoroutine = StartCoroutine(AssignGroupsAcrossEndpoints());
    }

    // EXACT behavior requested:
    // While groups remain -> go through active endpoints -> assign 1 group -> wait -> next endpoint
    // When end reached and groups remain -> start again from beginning.
    private IEnumerator AssignGroupsAcrossEndpoints()
    {
        // Build the groups-left stack/list
        List<SpawnGroupState> groupsLeft = new List<SpawnGroupState>(listOfGroupsOfMonstersToSpawn.Count);
        for (int i = 0; i < listOfGroupsOfMonstersToSpawn.Count; i++)
            groupsLeft.Add(new SpawnGroupState(listOfGroupsOfMonstersToSpawn[i]));

        while (groupsLeft.Count > 0)
        {
            // Snapshot valid endpoints for this pass
            List<EndPoint> validEndpoints = GetValidEndpointsSnapshot();
            if (validEndpoints.Count == 0)
            {
                Debug.LogError("[WaveManager] No valid endpoints during assignment. Aborting remaining assignments to avoid soft-lock.");
                allGroupsAssigned = true;
                UpdateFinishedSpawningFlag();
                TryCompleteWave();
                yield break;
            }

            // One pass over endpoints
            for (int i = 0; i < validEndpoints.Count && groupsLeft.Count > 0; i++)
            {
                EndPoint ep = validEndpoints[i];

                // assign one group to this endpoint (independent coroutine)
                SpawnGroupState group = groupsLeft[groupsLeft.Count - 1];
                groupsLeft.RemoveAt(groupsLeft.Count - 1);

                activeGroupSpawnCoroutines++;
                StartCoroutine(SpawnGroupOnEndpoint(ep, group)); // independent

                // wait only after assigning (does not interfere with spawnDelay)
                if (endpointAssignmentDelay > 0f)
                    yield return new WaitForSeconds(endpointAssignmentDelay);
                else
                    yield return null;
            }

            // if groupsLeft still > 0, loop and do another pass over endpoints
            yield return null;
        }

        // All groups assigned
        allGroupsAssigned = true;
        UpdateFinishedSpawningFlag();
        TryCompleteWave();
    }

    private List<EndPoint> GetValidEndpointsSnapshot()
    {
        List<EndPoint> valid = new List<EndPoint>();
        for (int i = 0; i < listOfActiveEndPoints.Count; i++)
        {
            EndPoint ep = listOfActiveEndPoints[i];
            if (ep != null && ep.gameObject.activeInHierarchy && !sealedEndpoints.Contains(ep))
                valid.Add(ep);
        }
        return valid;
    }

    // Independent per-group spawn coroutine
    private IEnumerator SpawnGroupOnEndpoint(EndPoint endpoint, SpawnGroupState group)
    {
        WaitForSeconds wait = new WaitForSeconds(spawnDelay);

        try
        {
            while (group.remaining > 0)
            {
                // If endpoint disappears or becomes sealed, stop this group (drop remaining to avoid soft-lock).
                if (endpoint == null || !endpoint.gameObject.activeInHierarchy || sealedEndpoints.Contains(endpoint))
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

                // only wait between each enemy
                yield return wait;
            }
        }
        finally
        {
            activeGroupSpawnCoroutines = Mathf.Max(0, activeGroupSpawnCoroutines - 1);
            UpdateFinishedSpawningFlag();
            TryCompleteWave();
        }
    }

    private void UpdateFinishedSpawningFlag()
    {
        // "All enemies have been spawned" means:
        // - assignment coroutine assigned every group
        // - and all group-spawn coroutines finished spawning their enemies
        finishedSpawning = allGroupsAssigned && activeGroupSpawnCoroutines == 0;
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

        // Wave ends only when ALL enemies have been spawned AND killed
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

        if (sealCostText != null)
            sealCostText.text = "Cost: " + currentSealCost;

        return true;
    }
}
