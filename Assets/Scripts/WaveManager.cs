using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEditor.iOS;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GridManager gridManager;
    public UIManager uiManager;

    private WaveData[] waves;
    private int currentWaveIndex = 0;
    private float currentWaveTime = 0;
    private float currentWaveEnemyDelay = 0;
    private Queue<EnemyWaveData> currentWaveQueue;
    private EnemyWaveData currentEnemyWaveData;

    void Update()
    {
        if (waves == null)
        {
            return;
        }

        currentWaveTime += Time.deltaTime;
        currentWaveEnemyDelay += Time.deltaTime;

        if (currentWaveEnemyDelay >= currentEnemyWaveData.delay)
        {
            SpawnEnemies();
            currentWaveEnemyDelay = 0;
            if (this.currentWaveQueue.Count > 0)
            {
                this.currentEnemyWaveData = this.currentWaveQueue.Dequeue();
            }
            else
            {
                waves = null; // TODO: finish this
            }
        }
    }

    public void Setup(WaveData[] waves)
    {
        this.waves = waves;

        this.currentWaveTime = 0;
        this.currentWaveEnemyDelay = 0;
        this.currentWaveIndex = 0;

        // Wave Start
        this.currentWaveQueue = new Queue<EnemyWaveData>(this.waves[this.currentWaveIndex].enemies);
        this.currentEnemyWaveData = this.currentWaveQueue.Dequeue();
        uiManager.SetWave(currentWaveIndex + 1, this.waves.Length);
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < currentEnemyWaveData.count; i++)
        {
            gridManager.SpawnEnemy(currentEnemyWaveData.name);
        }
    }
}