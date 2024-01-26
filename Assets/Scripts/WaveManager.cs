using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    private WaveData[] waves;
    private GridManager gridManager;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Setup(WaveData[] waves)
    {
        this.waves = waves;
    }
}
