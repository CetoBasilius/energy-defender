
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
    private static Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        LoadData(ref towersData, "Data/towers");
        LoadData(ref enemiesData, "Data/enemies");
        CreatePrefabs(enemiesData.ToDictionary(kvp => kvp.Key, kvp => (UnitData)kvp.Value));
        CreatePrefabs(towersData.ToDictionary(kvp => kvp.Key, kvp => (UnitData)kvp.Value));
    }

    private static void LoadData<T>(ref Dictionary<string, T> dataDictionary, string resourcePath) where T : UnitData
    {
        try
        {
            TextAsset dataString = Resources.Load<TextAsset>(resourcePath);
            dataDictionary = JsonConvert.DeserializeObject<Dictionary<string, T>>(dataString.text);
            foreach (var kvp in dataDictionary)
            {
                kvp.Value.id = kvp.Key;
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Error loading {typeof(T).Name} data: {e}");
        }
    }

    private static void CreatePrefabs(Dictionary<string, UnitData> dataDictionary)
    {
        foreach (var data in dataDictionary.Values)
        {
            GameObject prefab = Resources.Load<GameObject>(data.prefabPath);
            if (prefab == null)
            {
                Debug.LogError("Prefab not found: " + data.id);
                continue;
            }
            if (prefabs.ContainsKey(data.id))
            {
                Debug.LogError("Prefab key already exists: " + data.id + " - " + data.name);
                continue;
            }
            prefabs.Add(data.id, prefab);
        }
    }

    protected GameObject CreateEnemy(string id)
    {
        GameObject enemyPrefab = Resources.Load<GameObject>(enemiesData[id].prefabPath);
        GameObject enemy = Instantiate(enemyPrefab);
        enemy.GetComponent<Enemy>().Setup(enemiesData[id]);

        return enemy;
    }

    protected GameObject CreateTower(string id)
    {
        GameObject towerPrefab = prefabs[id];
        GameObject tower = Instantiate(towerPrefab);
        tower.GetComponent<Tower>().Setup(towersData[id]);

        return tower;
    }
    public int GetTowerEnergyCost(string id)
    {
        return towersData.ContainsKey(id) ? towersData[id].energyCost : 0;
    }
}