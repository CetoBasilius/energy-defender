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
    public UIManager uiManager;
    public WaveManager waveManager;

    private float maxEnergy = 0;
    private float currentEnergy = 0;
    private bool isPaused = false;

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
