using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.Tilemaps;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class GameManager : MonoBehaviour
{
    public int energyPerSecond = 1;
    public GridManager gridManager;
    // private LevelManager levelManager; // Is singleton, static methods
    public UIManager uiManager;
    public WaveManager waveManager;

    private float maxEnergy = 0;
    private float currentEnergy = 0;
    private bool isPaused = false;

    private static TowersData towersData;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void LoadTowerData()
    {
        try
        {
            TextAsset levelDataString = Resources.Load<TextAsset>("Data/towers");
            towersData = JsonConvert.DeserializeObject<TowersData>(levelDataString.text);
        }
        catch (Exception e)
        {
            Debug.Log("Error loading tower data:" + e);
        }
    }

    void Start()
    {
        LevelManager.SetLevel("World01/01"); // TODO: This will be able to be set from the main menu
        LevelData levelData = LevelManager.GetLevelData();
        gridManager.Setup(levelData);
        currentEnergy = levelData.startEnergy;
        maxEnergy = levelData.maxEnergy;
        waveManager.Setup(levelData.waves);
    }

    void Update()
    {
        if (isPaused)
        {
            return;
        }

        currentEnergy += energyPerSecond * Time.deltaTime;
        if (currentEnergy > maxEnergy)
        {
            currentEnergy = maxEnergy;
        }
        uiManager.SetEnergy(currentEnergy);
    }

    public TowerData GetTowerData(string dataType)
    {
        return towersData.ContainsKey(dataType) ? towersData[dataType] : null;
    }

    public float GetCurrentEnergy()
    {
        return currentEnergy;
    }

    public bool SpendEnergy(float energy)
    {
        if (energy > currentEnergy)
        {
            return false;
        }

        currentEnergy -= energy;
        uiManager.SetEnergy(currentEnergy);

        return true;
    }
}
