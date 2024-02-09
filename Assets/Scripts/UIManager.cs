using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public ScrollableCamera scrollableCamera;
    public Counter energyPanel;
    public Counter wavePanel;
    public GameObject rangeCircle;

    public void SetEnergy(float currentEnergy)
    {
        energyPanel.SetInt(currentEnergy);
    }

    public void SetWave(int currentWave, int totalWaves)
    {
        wavePanel.Set("Wave " + currentWave + '/' + totalWaves);
    }

    public void LockCamera()
    {
        scrollableCamera.SetLocked(false);
    }

    public void UnlockCamera()
    {
        scrollableCamera.SetLocked(true);
    }

    public bool IsCameraLocked()
    {
        return !scrollableCamera.IsLocked();
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void ShowRangeCircle(Vector3 mousePosition, int towerRange)
    {
        rangeCircle.transform.position = mousePosition;
        rangeCircle.transform.localScale = new Vector3(towerRange * 2, towerRange * 2, 1);
        rangeCircle.SetActive(true);
    }

    public void HideRangeCircle()
    {
        rangeCircle.SetActive(false);
    }
}
