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

    private void Awake()
    {
        Instance = this;
    }


    public void StartWave(int waveNumber)
    {
        listOfGroupsOfMonstersToSpawn.Clear();

        // First wave → only Group A (index 0 for example)
        if (waveNumber == 1)
        {
            listOfGroupsOfMonstersToSpawn.Add(listOfAllGroupsOfMonsters[0]);
        }
        else
        {
            // Pick X random groups, where X = wave number
            for (int i = 0; i < waveNumber; i++)
            {
                MonsterGroup randomGroup =
                    listOfGroupsToPickFrom[Random.Range(0, listOfGroupsToPickFrom.Count)];

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
                    Instantiate(group.MonsterPrefab, endpoint.transform, endpoint.transform);
                    yield return new WaitForSeconds(spawnDelay);
                }

                // Remove group after spawning
                listOfGroupsLeftToSpawn.RemoveAt(listOfGroupsLeftToSpawn.Count - 1);
            }
        }
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
}
