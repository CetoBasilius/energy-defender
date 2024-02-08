
using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Newtonsoft.Json;
using UnityEngine;

public class UnitFactory : MonoBehaviour
{
    private static Dictionary<string, TowerData> towersData;
    private static Dictionary<string, EnemyData> enemiesData;

    private Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        LoadTowerData();
        LoadEnemyData();
    }

    private void Start()
    {
        CreatePrefabs(enemiesData.ToDictionary(kvp => kvp.Key, kvp => (UnitData)kvp.Value));
        CreatePrefabs(towersData.ToDictionary(kvp => kvp.Key, kvp => (UnitData)kvp.Value));
    }

    public TowerData GetTowerData(string dataType)
    {
        return towersData.ContainsKey(dataType) ? towersData[dataType] : null;
    }

    private static void LoadTowerData()
    {
        try
        {
            TextAsset towerDataString = Resources.Load<TextAsset>("Data/towers");
            towersData = JsonConvert.DeserializeObject<Dictionary<string, TowerData>>(towerDataString.text);
        }
        catch (Exception e)
        {
            Debug.Log("Error loading tower data:" + e);
        }
    }

    private static void LoadEnemyData()
    {
        try
        {
            TextAsset levelDataString = Resources.Load<TextAsset>("Data/enemies");
            enemiesData = JsonConvert.DeserializeObject<Dictionary<string, EnemyData>>(levelDataString.text);
        }
        catch (Exception e)
        {
            Debug.Log("Error loading enemy data:" + e);
        }
    }

    public GameObject CreateEnemy(string dataType)
    {
        GameObject enemyPrefab = Resources.Load<GameObject>(enemiesData[dataType].prefabPath);
        EnemyData enemyData = enemiesData[dataType];
        GameObject enemy = Instantiate(enemyPrefab);
        enemy.GetComponent<Enemy>().Setup(enemyData);

        return enemy;
    }

    private void CreatePrefabs(Dictionary<string, UnitData> dataDictionary)
    {
        foreach (var data in dataDictionary.Values)
        {
            GameObject prefab = Resources.Load<GameObject>(data.prefabPath);
            if (prefab == null)
            {
                Debug.LogError("Prefab not found: " + data.prefabPath);
                continue;
            }
            if (prefabs.ContainsKey(data.name))
            {
                Debug.LogError("Prefab already exists: " + data.name);
                continue;
            }
            prefabs.Add(data.name, prefab);
        }
    }
    public int GetTowerEnergyCost(string dataType)
    {
        return towersData.ContainsKey(dataType) ? towersData[dataType].energyCost : 0;
    }
}